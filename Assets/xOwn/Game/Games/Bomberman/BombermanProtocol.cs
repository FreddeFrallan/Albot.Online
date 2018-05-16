using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Game;

namespace Bomberman{

	public class BombermanProtocol : CommProtocol {

		public enum MsgType : short {
			boardInit = 800,
			boardUpdate = 801,
			playerInit = 802,
			playerCommands = 803,
			gameInfo = 804,
		}

		public override void init (System.Action<object, int, short> sendMsgFunc){
			sendMsg = sendMsgFunc;
			currentProtocol.Add(new AlbotMessage(typeof(GameInfo), (short)MsgType.playerInit));
			currentProtocol.Add(new AlbotMessage(typeof(GameInfo), (short)MsgType.gameInfo));
			currentProtocol.Add(new AlbotMessage(typeof(BoardUpdate), (short)MsgType.boardInit));
			currentProtocol.Add(new AlbotMessage(typeof(BoardUpdate), (short)MsgType.boardUpdate));
		}

		public void sendGameInfo(int targetID, GameInfo gi) {sendMsg(gi, targetID, (short)MsgType.gameInfo);}
		public void sendPlayerInit(int targetID, GameInfo gi) {sendMsg(gi, targetID, (short)MsgType.playerInit);}
		public void sendBoardInit(int targetID, BoardUpdate b){sendMsg(b, targetID, (short)MsgType.boardInit);}
		public void sendBoardUpdate(int targetID, BoardUpdate b){sendMsg(b, targetID, (short)MsgType.boardUpdate);}


		private void sendString(int targetID, MsgType type, string str, Game.PlayerColor color) {
			sendMsg(new StringMessage(str, color), targetID, (short)type);
		}
	}

	[Serializable]
	public class PlayerCommand{
		public PlayerColor color;
		public bool dropBomb;
		public int moveDir;
		public PlayerCommand(PlayerColor color, int moveDir, bool dropBomb = false){this.color = color; this.dropBomb = dropBomb; this.moveDir = moveDir;}
	}
		
	[Serializable]
	public class GameInfo{
		public string username;
		public PlayerColor myColor;
		public bool gameOver;
		public PlayerColor winnerColor;
		public GameInfo(string username, PlayerColor myColor, bool gameOver = false, PlayerColor winnerColor = PlayerColor.None){
			this.username = username;
			this.myColor = myColor;
			this.gameOver = gameOver;
			this.winnerColor = winnerColor;
		}
	}



	#region BoardUpdate
	[Serializable]
	public class BoardUpdate{
		public PlayerColor color;
		public int updateID;
		public List<MapObj> currentMap;
		public 	List<BoardAction> currentActions;
		public BoardUpdate(List<MapObj> map, List<BoardAction> actions, PlayerColor color, int updateID){currentMap = map; this.currentActions = actions; this.color = color; this.updateID = updateID;}
	}

	#region BoardActions
	[Serializable]
	public class BoardAction{
		public int targetID;
		public ActionType type;
		public float[] pos;
		public BoardAction(ActionType type, int id, float[] pos){this.type = type; this.targetID = id; this.pos = pos;}
	}
	public enum ActionType{
		PlayerDeath,
		BombExplosion,
		BombSpawn
	}
	#endregion

	#region MapObjects
	[Serializable]
	public class MapObj{
		public int gameID, bombsLeft;
		public BombermanObjType type;
		public float[] pos, targetPos;
		public PlayerColor color;
		public float explodeDuration;
		public MapObj(int ID,  BombermanObjType type, float[] pos, int bombsLeft = 0){
			this.gameID = ID;
			this.type = type;
			this.pos = pos;
			this.bombsLeft = bombsLeft;
		}
	}
	[Serializable]
	public enum BombermanObjType{
		Player,
		Bomb,
		Pickup
	}
	#endregion

	#endregion
}