﻿using System;
using System.Collections;
using System.Collections.Generic;
using Barebones.Logging;
using Barebones.MasterServer;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;
using AlbotServer;
using Game;

public class AlbotGameRoom : NetworkBehaviour{
    public static SpawnTaskController SpawnTaskController;

    /// <summary>
    /// Unet msg type 
    /// </summary>
    public static short AccessMsgType = 3000;
    

    [Header("General")]
    public LogLevel LogLevel = LogLevel.Warn;

    [Tooltip("This address will be sent to clients with an access token")]
    public string PublicIp = "xxx.xxx.xxx.xxx";
    public string Name = "Room Name";
    public int MaxPlayers = 5;
    public bool IsPublic = true;
    public string Password = "";
    public bool AllowUsersRequestAccess = true;

    [Header("Room properties")]
    public string MapName = "Amazing Map";

    [Header("Other")]
    public bool QuitAppIfDisconnected = true;
	public BmLogger Logger = Msf.Create.Logger(typeof(AlbotGameRoom).Name);

    protected Dictionary<int, UnetMsfPlayer> PlayersByPeerId;
    protected Dictionary<string, UnetMsfPlayer> PlayersByUsername;
    protected Dictionary<int, UnetMsfPlayer> PlayersByConnectionId;

    public NetworkManager NetworkManager;
    public RoomController Controller;
	public List<string> preGamesPlayers = new List<string>();


	private GameType gameType;

    protected virtual void Awake(){
        NetworkManager = NetworkManager ?? FindObjectOfType<NetworkManager>();

        Logger.LogLevel = LogLevel;

        PlayersByPeerId = new Dictionary<int, UnetMsfPlayer>();
        PlayersByUsername = new Dictionary<string, UnetMsfPlayer>();
        PlayersByConnectionId = new Dictionary<int, UnetMsfPlayer>();

        NetworkServer.RegisterHandler(AccessMsgType, HandleReceivedAccess);
        Msf.Server.Rooms.Connection.Disconnected += OnDisconnectedFromMaster;
    }

    public bool IsRoomRegistered { get; protected set; }

    /// <summary>
    /// This will be called, when game server starts
    /// </summary>
    public override void OnStartServer(){
        // Find the manager, in case it was inaccessible on awake
        NetworkManager = NetworkManager ?? FindObjectOfType<NetworkManager>();

		MsfArgs.runAfterArgsInit (BeforeRegisteringRoom);
		MsfArgs.runAfterArgsInit (RegisterRoom);
    }

    /// <summary>
    /// This method is called before creating a room. It can be used to
    /// extract some parameters from cmd args or from span task properties
    /// </summary>
    protected virtual void BeforeRegisteringRoom(){
        if (SpawnTaskController != null){
            // If this server was spawned, try to read some of the properties
            var prop = SpawnTaskController.Properties;

            // Room name
            if (prop.ContainsKey(MsfDictKeys.RoomName))
                Name = prop[MsfDictKeys.RoomName];

            if (prop.ContainsKey(MsfDictKeys.MaxPlayers))
                MaxPlayers = int.Parse(prop[MsfDictKeys.MaxPlayers]);

            if (prop.ContainsKey(MsfDictKeys.RoomPassword))
                Password = prop[MsfDictKeys.RoomPassword];

            if (prop.ContainsKey(MsfDictKeys.MapName))
                MapName = prop[MsfDictKeys.MapName];

			gameType = Msf.Args.GameType;
        }

        // Override the public address
        if (Msf.Args.IsProvided(Msf.Args.Names.MachineIp) && NetworkManager != null){
            PublicIp = Msf.Args.MachineIp;
            
        }
    }

    public virtual void RegisterRoom(){
        var isUsingLobby = Msf.Args.IsProvided(Msf.Args.Names.LobbyId);

        var properties = SpawnTaskController != null 
            ? SpawnTaskController.Properties 
            : new Dictionary<string, string>();

        if (!properties.ContainsKey(MsfDictKeys.MapName))
            properties[MsfDictKeys.MapName] = MapName;

		properties[MsfDictKeys.SceneName] = Msf.Args.LoadScene;
        // 1. Create options object
        var options = new RoomOptions(){
            RoomIp = PublicIp,
            RoomPort = NetworkManager.networkPort,
            Name = Name,
            MaxPlayers = MaxPlayers,

            // Lobby rooms should be private, because they are accessed differently
            IsPublic = isUsingLobby ? false : IsPublic,
            AllowUsersRequestAccess = isUsingLobby ? false : AllowUsersRequestAccess,
            Password = "",
            Properties = new Dictionary<string, string>(){
                {MsfDictKeys.MapName, MapName }, // Show the name of the map
				{MsfDictKeys.SceneName, Msf.Args.LoadScene}, // Add the scene name
				{MsfDictKeys.GameType, gameType.ToString()},
				{MsfDictKeys.IsPreGame, false.ToString()},
            }
        };

        BeforeSendingRegistrationOptions(options);

        // 2. Send a request to create a room
        Msf.Server.Rooms.RegisterRoom(options, (controller, error) =>{
            if (controller == null){
                Logger.Error("Failed to create a room: " + error);
                return;
            }

            // Save the controller
            Controller = controller;
            OnRoomRegistered(controller);
        });
    }

    /// <summary>
    /// Override this method, if you want to make some changes to registration options
    /// </summary>
    /// <param name="options"></param>
    protected virtual void BeforeSendingRegistrationOptions(RoomOptions options){
    }

    /// <summary>
    /// Called when room is registered to the "master server"
    /// </summary>
    /// <param name="roomController"></param>
    public void OnRoomRegistered(RoomController roomController){
        IsRoomRegistered = true;

        // Set access provider (Optional)
        roomController.SetAccessProvider(CreateAccess);
        // If this room was spawned
        if (SpawnTaskController != null)
            SpawnTaskController.FinalizeTask(CreateSpawnFinalizationData());
    }

    /// <summary>
    /// Override, if you want to manually handle creation of access'es
    /// </summary>
    /// <param name="callback"></param>
    public virtual void CreateAccess(UsernameAndPeerIdPacket requester, RoomAccessProviderCallback callback){
        callback.Invoke(new RoomAccessPacket(){
            RoomIp = Controller.Options.RoomIp,
            RoomPort = Controller.Options.RoomPort,
            Properties = Controller.Options.Properties,
            RoomId = Controller.RoomId,
			SceneName = Msf.Args.LoadScene,
            Token = Guid.NewGuid().ToString()
        }, null);
    }

    /// <summary>
    /// This dictionary will be sent to "master server" when we want 
    /// notify "master" server that Spawn Process is completed
    /// </summary>
    /// <returns></returns>
    public virtual Dictionary<string, string> CreateSpawnFinalizationData(){
        return new Dictionary<string, string>(){
            // Add room id, so that whoever requested to spawn this game server,
            // knows which rooms access to request
            {MsfDictKeys.RoomId, Controller.RoomId.ToString()} 
        };
    }


    protected virtual void HandleReceivedAccess(NetworkMessage netmsg){
        var token = netmsg.ReadMessage<StringMessage>().value;

        Controller.ValidateAccess(token, (validatedAccess, error) =>{
            if (validatedAccess == null){
                //Logger.Error("Failed to confirm access token:" + error);
                // Confirmation failed, disconnect the user
                netmsg.conn.Disconnect();
                return;
            }

			//Debug.LogError("Confirmed token access for peer: " + validatedAccess);

            // Get account info
	            Msf.Server.Auth.GetPeerAccountInfo(validatedAccess.PeerId, (info, errorMsg) =>{
	                if (info == null)
	                {
	                    Logger.Error("Failed to get account info of peer " + validatedAccess.PeerId + "" +
	                                 ". Error: " + errorMsg);
	                    return;
	                }

					//Debug.LogError("Got peer account info: " + info);

					
					ConnectedClient newC = new ConnectedClient(netmsg.conn, info.PeerId);
					GameRoomClients.clientConnected(newC);
	            });
        });
    }

    private void OnDisconnectedFromMaster(){
        if (QuitAppIfDisconnected)
            Application.Quit();
    }

    protected virtual void OnDestroy(){
        Msf.Server.Rooms.Connection.Disconnected -= OnDisconnectedFromMaster;
    }
}