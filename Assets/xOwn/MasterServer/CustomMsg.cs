using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/*
public class CustomNetMsg{
	
	public static void sendBoardUpdate(NetworkConnection c, string message){
		if (c.isConnected) {
			boardUpdateMsg msg = new boardUpdateMsg ();
			msg.rawBoard = message;
			c.Send ((short)CMsgType.boardUpdate, msg);
		}
		else
			Debug.LogError ("Tried to send boardUpdate, but not connected to client");
	}
		
	public static void sendPlayedMove(NetworkConnection c, string message){
		if (c.isConnected) {
			playedMoveMsg msg = new playedMoveMsg ();
			msg.choosenMove = message;
			c.Send ((short)CMsgType.movePlayed, msg);
		}
		else
			Debug.LogError ("Tried to send playedMove, but not connected to server");
	}

	public static void sendRPCMove(NetworkConnection c, string message){
		if (c.isConnected) {
			playedMoveMsg msg = new playedMoveMsg ();
			msg.choosenMove = message;
			c.Send ((short)CMsgType.RPCMove, msg);
		}
		else
			Debug.LogError ("Tried to send RPCMove, but not connected to client");
	}

	public static void sendPlayerInitSettings(NetworkConnection c, bool message){
		if (c.isConnected) {
			initPlayerMsg msg = new initPlayerMsg ();
			msg.isYellow = message;
			c.Send ((short)CMsgType.playerInit, msg);
		}
		else
			Debug.LogError ("Tried to send playedMove, but not connected to client");
	}

	public static void sendGameStatus(NetworkConnection c, string message){
		if (c.isConnected) {
			gameStatusMsg msg = new gameStatusMsg ();
			msg.currentGameStatus = message;
			c.Send ((short)CMsgType.gameStatus, msg);
		}
		else
			Debug.LogError ("Tried to send gameStatus, but not connected to client");
	}
}

#region blueprints
public class boardUpdateMsg : MessageBase{
	public string rawBoard;
}
public class gameStatusMsg : MessageBase{
	public string currentGameStatus;
}
public class playedMoveMsg : MessageBase{
	public string choosenMove;
}
public class initPlayerMsg : MessageBase{
	public bool isYellow;
}

public enum CMsgType : short{
	movePlayed = 800,
	boardUpdate = 801,
	playerInit = 802,
	RPCMove = 803,
	gameStatus = 804
}
#endregion
*/