using System;
using Barebones.Networking;
using UnityEngine;

namespace Barebones.MasterServer
{
    public class SpawnRequestController
    {
        private readonly IClientSocket _connection;
        public string SpawnId { get; set; }
        public string spawnCode { get; private set; }

        public event Action<SpawnStatus> StatusChanged;

        public SpawnStatus Status { get; private set; }

        public SpawnRequestController(string spawnId, IClientSocket connection, string spawnCode)
        {
            _connection = connection;
            SpawnId = spawnId;
            this.spawnCode = spawnCode;

            // Set handlers
            connection.SetHandler((short) MsfOpCodes.SpawnRequestStatusChange, HandleStatusUpdate);
        }

        public void Abort()
        {
            Msf.Client.Spawners.AbortSpawn(SpawnId);
        }

        public void Abort(MsfSpawnersClient.AbortSpawnHandler handler)
        {
            Msf.Client.Spawners.AbortSpawn(SpawnId, handler);
        }

        [Obsolete("Use GetFinalizationData")]
        public void GetCompletionData(MsfSpawnersClient.FinalizationDataHandler handler)
        {
            Msf.Client.Spawners.GetFinalizationData(SpawnId, handler, _connection);
        }

        public void GetFinalizationData(MsfSpawnersClient.FinalizationDataHandler handler)
        {
            Msf.Client.Spawners.GetFinalizationData(spawnCode, handler, _connection);
        }

        private static void HandleStatusUpdate(IIncommingMessage message)
        {
            var data = message.Deserialize(new SpawnStatusUpdatePacket());
            //Debug.LogError(data.Status + "  " + data.SpawnId);

            var controller = Msf.Client.Spawners.GetRequestController(data.SpawnId);

            if (controller == null)
                return;

            controller.Status = data.Status;

            if (controller.StatusChanged != null)
                controller.StatusChanged.Invoke(data.Status);
        }
    }
}