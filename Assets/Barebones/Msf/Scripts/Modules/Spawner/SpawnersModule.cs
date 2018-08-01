using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Barebones.Logging;
using Barebones.Networking;
using UnityEngine;
using AlbotServer;

namespace Barebones.MasterServer
{
    public class SpawnersModule : ServerModuleBehaviour
    {
        public delegate void SpawnedProcessRegistrationHandler(SpawnTask task, IPeer peer);
		public static SpawnersModule singleton;

        #region Unity Inspector

        [Header("Permissions")]
        [Tooltip("Minimal permission level, necessary to register a spanwer")]
        public int CreateSpawnerPermissionLevel = 0;

        [Header("General")]
        public LogLevel LogLevel = LogLevel.Warn;

        [Tooltip("How often spawner queues are updated")]
        public float QueueUpdateFrequency = 0.1f;

        [Tooltip("If true, clients will be able to request spawns")]
        public bool EnableClientSpawnRequests = true;

        #endregion


        public event Action<RegisteredSpawner> SpawnerRegistered; 
        public event Action<RegisteredSpawner> SpawnerDestroyed;
        public event SpawnedProcessRegistrationHandler SpawnedProcessRegistered;

        protected Dictionary<string, RegisteredSpawner> Spawners;

        protected Dictionary<string, SpawnTask> SpawnTasks;

        public BmLogger Logger = Msf.Create.Logger(typeof(SpawnersModule).Name);

        protected virtual void Awake(){
            Logger.LogLevel = LogLevel;
        }

        public override void Initialize(IServer server){
            base.Initialize(server);
			singleton = this;

            Spawners = new Dictionary<string, RegisteredSpawner>();
            SpawnTasks = new Dictionary<string, SpawnTask>();

            // Add handlers
            server.SetHandler((short) MsfOpCodes.RegisterSpawner, HandlerRegisterSpawner);
            server.SetHandler((short) MsfOpCodes.ClientsSpawnRequest, HandleClientsSpawnRequest);
            server.SetHandler((short) MsfOpCodes.RegisterSpawnedProcess, HandleRegisterSpawnedProcess);
            server.SetHandler((short) MsfOpCodes.CompleteSpawnProcess, HandleCompleteSpawnProcess);
            server.SetHandler((short) MsfOpCodes.ProcessStarted, HandleProcessStarted);
            server.SetHandler((short) MsfOpCodes.ProcessKilled, HandleProcessKilled);
            server.SetHandler((short) MsfOpCodes.AbortSpawnRequest, HandleAbortSpawnRequest);
            server.SetHandler((short) MsfOpCodes.GetSpawnFinalizationData, HandleGetCompletionData);
            server.SetHandler((short) MsfOpCodes.UpdateSpawnerProcessesCount, HandleSpawnedProcessesCount);

            // Coroutines
            StartCoroutine(StartQueueUpdater());
        }

        public virtual RegisteredSpawner CreateSpawner(IPeer peer, SpawnerOptions options){
            var spawner = new RegisteredSpawner(GenerateSpawnerId(), peer, options);

            var peerSpawners =
                peer.GetProperty((int) MsfPropCodes.RegisteredSpawners) as Dictionary<string, RegisteredSpawner>;

            if (peerSpawners == null)
            {
                // If this is the first time registering a spawners

                // Save the dictionary
                peerSpawners = new Dictionary<string, RegisteredSpawner>();
                peer.SetProperty((int) MsfPropCodes.RegisteredSpawners, peerSpawners);

                peer.Disconnected += OnRegisteredPeerDisconnect;
            }

            // Add a new spawner
            peerSpawners[spawner.SpawnerId] = spawner;

            // Add the spawner to a list of all spawners
            Spawners[spawner.SpawnerId] = spawner;

            // Invoke the event
            if (SpawnerRegistered != null)
                SpawnerRegistered.Invoke(spawner);

            return spawner;
        }

        private void OnRegisteredPeerDisconnect(IPeer peer){
            var peerSpawners = peer.GetProperty((int)MsfPropCodes.RegisteredSpawners) as Dictionary<string, RegisteredSpawner>;

            if (peerSpawners == null)
                return;

            // Create a copy so that we can iterate safely
            var registeredSpawners = peerSpawners.Values.ToList();

            foreach (var registeredSpawner in registeredSpawners)
            {
                DestroySpawner(registeredSpawner);
            }
        }

        public void DestroySpawner(RegisteredSpawner spawner){
            var peer = spawner.Peer;

            if (peer != null)
            {
                var peerRooms = peer.GetProperty((int)MsfPropCodes.RegisteredSpawners) as Dictionary<string, RegisteredSpawner>;

                // Remove the spawner from peer
                if (peerRooms != null)
                    peerRooms.Remove(spawner.SpawnerId);
            }

            // Remove the spawner from all spawners
            Spawners.Remove(spawner.SpawnerId);

            // Invoke the event
            if (SpawnerDestroyed != null)
                SpawnerDestroyed.Invoke(spawner);
        }

        public string GenerateSpawnerId(){
            return Msf.Helper.CreateRandomStringMatch(MasterServerConstants.KEY_LENGTH, (key) => { return Spawners.ContainsKey(key); });
        }

        public string GenerateSpawnTaskId(){
            return Msf.Helper.CreateRandomStringMatch(MasterServerConstants.KEY_LENGTH, (key) => { return SpawnTasks.ContainsKey(key); });
        }

        public SpawnTask Spawn(Dictionary<string, string> properties) {
            return Spawn(properties, "", "");
        }

        public SpawnTask Spawn(Dictionary<string, string> properties, string region){
            return Spawn(properties, region, "");
        }

        public virtual SpawnTask Spawn(Dictionary<string, string> properties, string region, string customArgs, string spawnID = "") {
            var spawners = GetFilteredSpawners(properties, region);

            if (spawners.Count <= 0){
                Logger.Warn("No spawner was returned after filtering. " + 
                    (string.IsNullOrEmpty(region) ? "" : "Region: " + region));
                return null;
            }

            // Order from least busy server
			List<RegisteredSpawner> orderedSpawners = spawners.OrderByDescending(s => s.CalculateFreeSlotsCount()).ToList();

            var availableSpawner = orderedSpawners.FirstOrDefault(s => s.CanSpawnAnotherProcess());

            // Ignore, if all of the spawners are busy
			if (availableSpawner == null) {
				Debug.LogError ("No available spawners");
				return null;
			}
            return Spawn(properties, customArgs, availableSpawner, spawnID);
        }

        /// <summary>
        /// Requests a specific spawner to spawn a process
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="customArgs"></param>
        /// <param name="spawner"></param>
        /// <returns></returns>
        public virtual SpawnTask Spawn(Dictionary<string, string> properties, string customArgs, RegisteredSpawner spawner, string spawnID){
            var task = new SpawnTask(spawnID, spawner, properties, customArgs);

            if (SpawnTasks.ContainsKey(task.UniqueCode)) {
                Debug.LogError("Already have Spawner: " + task.UniqueCode);
                SpawnTasks.Remove(task.UniqueCode);
            }

            SpawnTasks.Add(task.UniqueCode, task);
            spawner.AddTaskToQueue(task);

            return task;
        }

        /// <summary>
        /// Retrieves a list of spawner that can be used with given properties and region name
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public virtual List<RegisteredSpawner> GetFilteredSpawners(Dictionary<string, string> properties, string region){
            return GetSpawners(region);
        }

        public virtual List<RegisteredSpawner> GetSpawners(){
            return GetSpawners(null);
        }

        public virtual List<RegisteredSpawner> GetSpawners(string region){
            // If region is not provided, retrieve all spawners
            if (string.IsNullOrEmpty(region))
                return Spawners.Values.ToList();

            return GetSpawnersInRegion(region);
        }

        public virtual List<RegisteredSpawner> GetSpawnersInRegion(string region){
            return Spawners.Values
                .Where(s => s.Options.Region == region)
                .ToList();
        }

        /// <summary>
        /// Returns true, if peer has permissions to register a spawner
        /// </summary>
        /// <param name="peer"></param>
        /// <returns></returns>
        protected virtual bool HasCreationPermissions(IPeer peer){
            var extension = peer.GetExtension<PeerSecurityExtension>();
            return extension.PermissionLevel >= CreateSpawnerPermissionLevel;
        }

        protected virtual bool CanClientSpawn(IPeer peer, ClientsSpawnRequestPacket data){
            return EnableClientSpawnRequests;
        }

        protected virtual IEnumerator StartQueueUpdater(){
            while (true) {
                yield return new WaitForSeconds(QueueUpdateFrequency);

                foreach (var spawner in Spawners.Values)
                    try{
                        spawner.UpdateQueue();
                    }
                    catch (Exception e){
                        Logger.Error(e);
                    }
            }
        }

        #region Message Handlers

        protected virtual void HandleClientsSpawnRequest(IIncommingMessage message){
            /*
            var data = message.Deserialize(new ClientsSpawnRequestPacket());
            var peer = message.Peer;

            if (!CanClientSpawn(peer, data)){
                message.Respond("Unauthorized", ResponseStatus.Unauthorized);
                return;
            }

            var prevRequest = peer.GetProperty((int) MsfPropCodes.ClientSpawnRequest) as SpawnTask;

            if (prevRequest != null && !prevRequest.IsDoneStartingProcess){
                message.Respond("You already have an active request", ResponseStatus.Failed);
                return;
            }

            // Get the spawn task
            var task = Spawn(data.Options, data.Region, "");
            if (task == null){
                message.Respond("All the servers are busy. Try again later".ToBytes(), ResponseStatus.Failed);
                return;
            }
            task.Requester = message.Peer;

            // Save the task
            peer.SetProperty((int)MsfPropCodes.ClientSpawnRequest, task);

            // Listen to status changes
            task.StatusChanged += (status) =>{
                // Send status update
                var msg = Msf.Create.Message((short) MsfOpCodes.SpawnRequestStatusChange, new SpawnStatusUpdatePacket(){
                    SpawnId = task.SpawnId,
                    Status = status
                });
                message.Peer.SendMessage(msg);
            };

            message.Respond(task.SpawnId, ResponseStatus.Success);
            */
         }



        //			The Current function used in Albot
        // Here we pass some command line Args to the new Game Server, such as GameType, Realtime & hasSpectators
        // This is done in the "DefaultSpawnRequestHandler" located in "SpawnerController"
        // New Data can be sent by adding it to the Dict "Options" and then later adding it to the Command line arg
        // These values will automaticlly be read by the Msf.Args module, and be available after startup
        /************************************************/
        
		public string createNewRoomFromPreGame(List<IPeer> peersInRoom, Dictionary<string, string> options, string spawnID){
			var task = Spawn(options, "", "", spawnID);
			if (task == null) //All the servers are busy. Try again later"
				return "";
			
            Debug.LogError("Task: " + task.SpawnId);
			task.Requester = peersInRoom[0];
            Debug.LogError("Creating game: " + options[MsfDictKeys.GameType]);
            task.albotHack(peersInRoom);


            return task.UniqueCode;
		}

        private void HandleAbortSpawnRequest(IIncommingMessage message){
            var prevRequest = message.Peer.GetProperty((int)MsfPropCodes.ClientSpawnRequest) as SpawnTask;

            if (prevRequest == null){
                message.Respond("There's nothing to abort", ResponseStatus.Failed);
                return;
            }

            if (prevRequest.Status >= SpawnStatus.Finalized){
                message.Respond("You can't abort a completed request", ResponseStatus.Failed);
                return;
            }

            if (prevRequest.Status <= SpawnStatus.None){
                message.Respond("Already aborting", ResponseStatus.Success);
                return;
            }

            prevRequest.Abort();
        }

        protected virtual void HandleGetCompletionData(IIncommingMessage message)
        {
            var spawnId = message.AsString();
            SpawnTask task;
            SpawnTasks.TryGetValue(spawnId, out task);

            if (task == null){
                message.Respond("Invalid request", ResponseStatus.Failed);
                return;
            }

			/*
            if (task.Requester != message.Peer){
                message.Respond("You're not the requester", ResponseStatus.Unauthorized);
                return;
            }
            */

            if (task.FinalizationPacket == null){
                message.Respond("Task has no completion data", ResponseStatus.Failed);
                return;
            }

            // Respond with data (dictionary of strings)
            message.Respond(task.FinalizationPacket.FinalizationData.ToBytes(), ResponseStatus.Success);
        }

        protected virtual void HandlerRegisterSpawner(IIncommingMessage message){
            if (!HasCreationPermissions(message.Peer)){
                message.Respond("Insufficient permissions", ResponseStatus.Unauthorized);
                return;
            }

            var options = message.Deserialize(new SpawnerOptions());

            var spawner = CreateSpawner(message.Peer, options);

            // Respond with spawner id
            message.Respond(spawner.SpawnerId, ResponseStatus.Success);
        }

        /// <summary>
        /// Handles a message from spawned process. Spawned process send this message
        /// to notify server that it was started
        /// </summary>
        /// <param name="message"></param>
        protected virtual void HandleRegisterSpawnedProcess(IIncommingMessage message)
        {
            var data = message.Deserialize(new RegisterSpawnedProcessPacket());

            SpawnTask task;
            Debug.LogError("Register");
            Debug.LogError("Register spawncode: " + data.SpawnCode);
            SpawnTasks.TryGetValue(data.SpawnCode, out task);

            if (task == null){
                message.Respond("Invalid spawn task", ResponseStatus.Failed);
                Logger.Error("Process tried to register to an unknown task");
                return;
            }

            if (task.UniqueCode != data.SpawnCode){
                message.Respond("Unauthorized", ResponseStatus.Unauthorized);
                Logger.Error("Spawned process tried to register, but failed due to mismaching unique code");
                return;
            }

            task.OnRegistered(message.Peer);

            if (SpawnedProcessRegistered != null)
                SpawnedProcessRegistered.Invoke(task, message.Peer);

            message.Respond(task.Properties.ToBytes(), ResponseStatus.Success);
        }

        protected virtual void HandleCompleteSpawnProcess(IIncommingMessage message){
            var data = message.Deserialize(new SpawnFinalizationPacket());

            SpawnTask task;
            SpawnTasks.TryGetValue(data.SpawnId, out task);

            if (task == null){
                message.Respond("Invalid spawn task", ResponseStatus.Failed);
                Logger.Error("Process tried to complete to an unknown task");
                return;
            }

            if (task.RegisteredPeer != message.Peer){
                message.Respond("Unauthorized", ResponseStatus.Unauthorized);
                Logger.Error("Spawned process tried to complete spawn task, but it's not the same peer who registered to the task");
                return;
            }

            task.OnFinalized(data);
            message.Respond(ResponseStatus.Success);
        }

        protected virtual void HandleProcessKilled(IIncommingMessage message){
            var spawnId = message.AsString();

            SpawnTask task;
            SpawnTasks.TryGetValue(spawnId, out task);

            if (task == null)
                return;

            task.OnProcessKilled();
            task.Spawner.OnProcessKilled();
        }

        protected virtual void HandleProcessStarted(IIncommingMessage message){
            var spawnId = message.AsString();

            SpawnTask task;
            SpawnTasks.TryGetValue(spawnId, out task);

            if (task == null)
                return;

            task.OnProcessStarted();
            task.Spawner.OnProcessStarted();
        }

        private void HandleSpawnedProcessesCount(IIncommingMessage message){
            var packet = message.Deserialize(new IntPairPacket());

            RegisteredSpawner spawner;
            Spawners.TryGetValue(packet.A, out spawner);

            if (spawner == null)
                return;

            spawner.UpdateProcessesCount(packet.B);
        }

        #endregion


    }
}