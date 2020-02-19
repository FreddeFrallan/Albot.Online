using System.Collections;
using System.Collections.Generic;
using System;

namespace FallingDebris{

	public class CommProtocol : Game.CommProtocol {
		public enum MsgType : short {
			move = 800,
			boardUpdate,
			playerInit,
		}

		public override void init (Action<Object, int, short> sendMsgFunc){
			sendMsg = sendMsgFunc;
			currentProtocol.Add(new AlbotMessage(typeof(StringMessage), (short)MsgType.move));
			currentProtocol.Add(new AlbotMessage(typeof(GameInfo), (short)MsgType.playerInit));
			currentProtocol.Add(new AlbotMessage(typeof(StringMessage), (short)MsgType.boardUpdate));
		}


		//Server
		public void sendMove(int targetID, string move) { sendString(targetID, MsgType.move, move); }
		public void sendGameState(int targetID, GameState gi) {sendMsg(gi, targetID, (short)MsgType.boardUpdate);}
		public void sendPlayerInit(int targetID, GameInfo gi) {sendMsg(gi, targetID, (short)MsgType.playerInit);}

		private void sendString(int targetID, MsgType type, string str) {
			sendMsg(new StringMessage(str), targetID, (short)type);
		}

	}

	[Serializable]
	public class GameState{
		public float currentPos;
		public double time;
		public int currentTarget;
		public GameState(float pos, int target, double time){currentTarget = target; currentPos = pos; this.time = time;}
	}

	[Serializable]
	public class GameInfo{
		public int myId;
		public Game.PlayerColor myColor;
		public bool gameOver;
		public Game.PlayerColor winnerColor;
		public GameInfo(int myId, Game.PlayerColor myColor, bool gameOver = false, Game.PlayerColor winnerColor = Game.PlayerColor.None){
			this.myId = myId;
			this.myColor = myColor;
			this.gameOver = gameOver;
			this.winnerColor = winnerColor;
		}
	}
}
