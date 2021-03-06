﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using Game;

namespace Snake{

	public class SnakeProtocol : Game.CommProtocol {


		public enum MsgType : short {
			boardStart = 800,
			boardUpdate = 801,
			playerInit = 802,
			playerCommands = 803,
			gameInfo = 804,
		}

		public override void init (System.Action<object, int, short> sendMsgFunc){
			sendMsg = sendMsgFunc;
			currentProtocol.Add(new AlbotMessage(typeof(BoardUpdate), (short)MsgType.boardUpdate));
			currentProtocol.Add(new AlbotMessage(typeof(GameInfo), (short)MsgType.playerInit));
			currentProtocol.Add(new AlbotMessage(typeof(GameInfo), (short)MsgType.gameInfo));
		}

		public void sendGameInfo(int targetID, GameInfo gi) {sendMsg(gi, targetID, (short)MsgType.gameInfo);}
		public void sendPlayerInit(int targetID, GameInfo gi) {sendMsg(gi, targetID, (short)MsgType.playerInit);}
		public void sendBoard(int targetID, BoardUpdate board){sendMsg(board, targetID, (short)MsgType.boardUpdate);}

	}

	[Serializable]
	public class RPCMove{ 
		public int[] move;
		public PlayerColor color;
		public RPCMove(int[] move, PlayerColor color){this.move = move; this.color = color;}
	}
	[Serializable]
	public class GameInfo{
		public string username;
		public PlayerColor myColor;
		public bool gameOver;
		public int[][] crashPos;
		public PlayerColor winnerColor;
		public GameInfo(string username, PlayerColor myColor, int[][] crashPos, bool gameOver = false, PlayerColor winnerColor = PlayerColor.None){
			this.crashPos = crashPos;
			this.username = username;
			this.myColor = myColor;
			this.gameOver = gameOver;
			this.winnerColor = winnerColor;
		}
	}

		
	[Serializable]
	public class BoardUpdate{
		public Position2D[] redCoords, blueCoords;
		public int redDir, blueDir;
		public uint updateNumber;
		public bool gameOver;
		public PlayerColor winnerColor, myColor;
		public BoardUpdate (){}
		public BoardUpdate(uint updateNumber, Position2D[] redC, Position2D[] blueC, int blueDir, int redDir, PlayerColor myColor = PlayerColor.None, bool gameOver = false, PlayerColor winnerColor = PlayerColor.None){
			this.updateNumber = updateNumber;
			this.redCoords = redC;
			this.blueCoords = blueC;
			this.redDir = redDir;
			this.blueDir = blueDir;
			this.myColor = myColor;
			this.gameOver = gameOver;
			this.winnerColor = winnerColor;
		}
	}
		

	[Serializable]
	public class GameCommand{
		public int dir;
		public PlayerColor myColor;
		public GameCommand(PlayerColor myColor, int dir){
			this.dir = dir;
			this.myColor = myColor;
		}
	}

}