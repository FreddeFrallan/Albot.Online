﻿using UnityEngine.Networking;
using System.Collections.Generic;

namespace AlbotServer{

	public enum ServerCommProtocl : short{
		//Master & Clients
		CheckCurrentVersion = 599,

		//Chat & Lobby
		LobbyInit = 600,
		LobbyPlayerEnter = 601,
		LobbyPlayerLeft =  602,
		LobbyChatMsg = 603,
		LobbyGameStats = 604,

		//Game Server
		PlayerLeftGameRoom = 605,
		PlayerJoinedGameRoom = 606,
		ClientReadyChannel = 607,
		GameRoomInitMsgChannel = 608,
		PlayerTimerInit = 609,
		PlayerTimerCommand = 610,

		//Pre games
		CreatePreGame = 611,
		RequestJoinPreGame = 612,
		UpdatePreGame = 613,
		ReadyUpdate = 614,
		StartPreGame = 615,
		RestartTrainingGame = 616,
		PlayerLeftPreGame = 617,
		SlotTypeChanged = 618,
		PreGameKick = 619,
	}


	/****** INHERITING does not work
		example: 
		Class A : MessageBase
		Class B : A
	*/
	public class StringMsg : MessageBase{
		public string msg;
	}

	#region PreGame
	public class PreGameMsg : MessageBase {
		public int roomID;
		public string errorMsg;
	}
	public class PreGameSlotSTypeMsg : MessageBase{
		public int roomID;
		public string errorMsg;
		public int slotID;
		public PreGameSlotType type;
		public PlayerInfo newPlayerInfo;
	}
	public class PreGameKickMsg : MessageBase{
		public int roomID;
		public string errorMsg;
		public int peerID;
	}
	public class PreGameStartMsg : MessageBase{		
		public int roomID, trainingRoomID;
		public string errorMsg;
		public bool isTraining, isSinglePlayer;
	}
	public class PreGameReadyUpdate : MessageBase{
		public int roomID;
		public string errorMsg;
		public bool isReady;
	}
	public class PreGameJoinRequest : MessageBase{
		public int roomID;
		public string errorMsg;
		public PlayerInfo joiningPlayer;
	}
	public class PreGameRoomMsg : MessageBase{
		public int roomID;
		public string errorMsg;
		public PreGamePlayer[] players;
		public Game.GameType type;
		public bool isTraining;
	}
	public class PreGameCreateMSg : MessageBase{
		public PlayerInfo mainPlayer;
		public Game.GameType type;
		public bool isTraining;
	}
	#endregion

	public class GameRoomInitMsg : MessageBase{
		public Game.GameType type;
	}

	//Msg we use to tell the GameServer that we are ready, and while we do that we send oer player info
	public class ClientReadyMsg : MessageBase{
		public PlayerInfo[] players;
	}	

	public class PlayerInfoMsg : MessageBase{
		public PlayerInfo player;
	}	

	public struct PlayerInfo{
		public string username;
		public int iconNumber;
		public Game.PlayerColor color;
	}

	#region Lobby & Chat
	public class LobbyGameStatsMsg : MessageBase{
		public int currentActiveGames, totalGamesPlayed;
	}
	#endregion
}