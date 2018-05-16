using System.Collections;
using System.Collections.Generic;
using System;
using Game;

namespace Breakthrough{

	public class BrakethroughProtocol : Game.CommProtocol{
		public enum MsgType : short {
			move = 800,
			boardUpdate = 801,
			playerInit = 802,
			moveRequest = 803,
			gameInfo = 804,
		}

		public override void init (Action<Object, int, short> sendMsgFunc){
			sendMsg = sendMsgFunc;
			currentProtocol.Add(new AlbotMessage(typeof(StringMessage), (short)MsgType.move));
			currentProtocol.Add(new AlbotMessage(typeof(BoardMsg), (short)MsgType.boardUpdate));
			currentProtocol.Add(new AlbotMessage(typeof(GameInfo), (short)MsgType.playerInit));
			currentProtocol.Add(new AlbotMessage(typeof(GameInfo), (short)MsgType.gameInfo));
		}
			

		//Server
		public void sendMove(int targetID, string move, Game.PlayerColor color) { sendString(targetID, MsgType.move, move, color); }
		public void requestMove(int targetID, BoardMsg b) { sendMsg(b, targetID, (short)MsgType.moveRequest ); }
		public void updateBoard(int targetID, BoardMsg b) { sendMsg(b, targetID, (short)MsgType.boardUpdate ); }

		public void sendGameInfo(int targetID, GameInfo gi) {sendMsg(gi, targetID, (short)MsgType.gameInfo);}
		public void sendPlayerInit(int targetID, GameInfo gi) {sendMsg(gi, targetID, (short)MsgType.playerInit);}

		private void sendString(int targetID, MsgType type, string str, Game.PlayerColor color) {
			sendMsg(new StringMessage(str, color), targetID, (short)type);
		}
	}


	[Serializable]
	public class BoardMsg{ 
		public int updateNumber;
		public int[,] board;
		public PlayerColor requestMoveColor;
		public BoardMsg(int[,] board, PlayerColor color, int updateNumber){this.board = board; this.requestMoveColor = color; this.updateNumber = updateNumber;}
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