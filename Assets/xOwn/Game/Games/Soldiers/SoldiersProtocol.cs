using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Soldiers{


	public class SoldiersProtocol : Game.CommProtocol {

		public enum MsgType : short {
			boardStart = 800,
			boardUpdate = 801,
			playerInit = 802,
			playerCommands = 803,
			gameInfo = 804,
		}

		public override void init (System.Action<object, int, short> sendMsgFunc){
			sendMsg = sendMsgFunc;
			currentProtocol.Add(new AlbotMessage(typeof(BoardUpdate), (short)MsgType.boardStart));
			currentProtocol.Add(new AlbotMessage(typeof(BoardUpdate), (short)MsgType.boardUpdate));
			currentProtocol.Add(new AlbotMessage(typeof(GameInfo), (short)MsgType.playerInit));
			currentProtocol.Add(new AlbotMessage(typeof(PlayerCommands), (short)MsgType.playerCommands));
			currentProtocol.Add(new AlbotMessage(typeof(GameInfo), (short)MsgType.gameInfo));
		}
			
		public void sendGameInfo(int targetID, GameInfo gi) {sendMsg(gi, targetID, (short)MsgType.gameInfo);}
		public void sendPlayerInit(int targetID, GameInfo gi) {sendMsg(gi, targetID, (short)MsgType.playerInit);}
		public void sendBoard(int targetID, BoardUpdate board){sendMsg(board, targetID, (short)MsgType.boardUpdate);}
		public void sendStartBoard(int targetID, BoardUpdate board){sendMsg(board, targetID, (short)MsgType.boardStart);}


		private void sendString(int targetID, MsgType type, string str, Game.PlayerColor color) {
			sendMsg(new StringMessage(str, color), targetID, (short)type);
		}

	}

	[Serializable]
	public class RPCMove{ 
		public int[] move;
		public Game.PlayerColor color;
		public RPCMove(int[] move, Game.PlayerColor color){this.move = move; this.color = color;}
	}
	[Serializable]
	public class GameInfo{
		public string username;
		public Game.PlayerColor myColor;
		public bool gameOver;
		public Game.PlayerColor winnerColor;
		public GameInfo(string username, Game.PlayerColor myColor, bool gameOver = false, Game.PlayerColor winnerColor = Game.PlayerColor.None){
			this.username = username;
			this.myColor = myColor;
			this.gameOver = gameOver;
			this.winnerColor = winnerColor;
		}
	}

	[Serializable]
	public class BoardUpdate{
		public Game.PlayerColor color;
		public float[,] units;
		public bool[] teams, canAttack;
		public float time;
		public uint updateId;
		public int[] deaths, healths, ids;
		public SoldierState[] states;

		public BoardUpdate(List<Soldier> soldiers, List<Soldier> deaths, uint updateId, float time, Game.PlayerColor color = Game.PlayerColor.None){
			units = new float[soldiers.Count, 2];
			teams = new bool[soldiers.Count];
			ids = new int[soldiers.Count];
			healths = new int[soldiers.Count];
			canAttack = new bool[soldiers.Count];
			states = new SoldierState[soldiers.Count];
			this.deaths = new int[deaths.Count];
			this.updateId = updateId;
			this.time = time;
			this.color = color;
			for (int i = 0; i < soldiers.Count; i++) {
				units [i, 0] = soldiers [i].transform.position.x;
				units [i, 1] = soldiers [i].transform.position.z;
				teams [i] = soldiers [i].team == 1;
				ids [i] = soldiers [i].id;
				healths [i] = soldiers [i].HP;
				canAttack [i] = soldiers [i].canDmg;
				states [i] = soldiers [i].intention;
			}

			for (int i = 0; i < deaths.Count; i++)
				this.deaths [i] = deaths [i].id;
		}
	}

	[Serializable]
	public class PlayerCommands{
		public Game.PlayerColor color;
		public TCPCommand[] commands;

		public PlayerCommands(Game.PlayerColor color, List<TCPCommand> commands){
			this.color = color;
			this.commands = commands.ToArray ();
		}
	}

}