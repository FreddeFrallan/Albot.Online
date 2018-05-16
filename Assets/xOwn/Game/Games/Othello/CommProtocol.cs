using System.Collections;
using System.Collections.Generic;
using System;

namespace Othello{

	public class CommProtocol : Game.TurnBasedCommProtocol {
		public enum MsgType : short {
			move = 800,
			boardUpdate,
			playerInit,
			RPCMove,
			gameInfo,
		}

		public override void init (Action<Object, int, short> sendMsgFunc){
			sendMsg = sendMsgFunc;
			currentProtocol.Add(new AlbotMessage(typeof(StringMessage), (short)MsgType.move));
			currentProtocol.Add(new AlbotMessage(typeof(StringMessage), (short)MsgType.boardUpdate));
			currentProtocol.Add(new AlbotMessage(typeof(GameInfo), (short)MsgType.playerInit));
			currentProtocol.Add(new AlbotMessage(typeof(StringMessage), (short)MsgType.RPCMove));
			currentProtocol.Add(new AlbotMessage(typeof(GameInfo), (short)MsgType.gameInfo));
		}


		//Server
		public void sendMove(int targetID, string move, Game.PlayerColor color) { sendString(targetID, MsgType.move, move, color); }
		public void requestMove(int targetID, string board, Game.PlayerColor color) { sendString(targetID, MsgType.boardUpdate, board, color); }
		public void sendRPCMove(int targetID, RPCMove move) { sendMsg(move, targetID, (short)MsgType.RPCMove); }
		public void sendGameInfo(int targetID, GameInfo gi) {sendMsg(gi, targetID, (short)MsgType.gameInfo);}
		public void sendPlayerInit(int targetID, GameInfo gi) {sendMsg(gi, targetID, (short)MsgType.playerInit);}

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

}
