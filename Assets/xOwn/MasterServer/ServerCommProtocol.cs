using UnityEngine.Networking;
using System.Collections.Generic;
using Barebones.MasterServer;

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
        Ping = 611,

		//Pre games
		CreatePreGame = 612,
		RequestJoinPreGame = 613,
		UpdatePreGame = 614,
		ReadyUpdate = 615,
		StartPreGame = 616,
		RestartTrainingGame = 617,
		PlayerLeftPreGame = 618,
		SlotTypeChanged = 619,
		PreGameKick = 620,
        StartSinglePlayerGame = 621,
        GameRoomInvite = 622,
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
        public string errorMsg, roomID;
    }
	public class PreGameSlotSTypeMsg : MessageBase{
        public string roomID;
        public PreGameSlotInfo slot;
	}
    public class PreGameStartedMsg : MessageBase {
        public PreGameSpecs specs;
        public PreGameSlotInfo[] slots;
    }
	public class PreGameReadyUpdate : MessageBase{
		public string roomID;
		public bool isReady;
	}
	public class PreGameJoinRequest : MessageBase{
        public string errorMsg, roomID;
        public GameInfoType roomType;
        public PlayerInfo joiningPlayer;
	}
	public class PreGameRoomMsg : MessageBase{
        public PreGameSpecs specs;
        public PreGameSlotInfo[] players;
	}
	public class PreGameCreateMSg : MessageBase{
		public PlayerInfo mainPlayer;
		public Game.GameType type;
        public int maxPlayers;
		public bool isTraining;
	}

    public class RunningGameInfoMsg : MessageBase {
        public PlayerInfo[] players;
        public Game.GameType gameType;
        public PreGameState status;
        public string gameID;
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