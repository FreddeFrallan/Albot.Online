using System.Collections;
using System.Collections.Generic;
using System;

namespace Battleship{

	public class BattleshipProtocol : Game.TurnBasedCommProtocol {

		public enum MsgType : short {
			move = 800,
			targetBoardUpdate,
			playerInit,
			RPCMove,
			gameInfo,
			boardInit,
			boardInitRequest,
		}
		public override void init (Action<Object, int, short> sendMsgFunc){
			sendMsg = sendMsgFunc;
			currentProtocol.Add(new AlbotMessage(typeof(StringMessage), (short)MsgType.move));
			currentProtocol.Add(new AlbotMessage(typeof(BoardMsg), (short)MsgType.targetBoardUpdate));
			currentProtocol.Add(new AlbotMessage(typeof(GameInfo), (short)MsgType.playerInit));
			currentProtocol.Add(new AlbotMessage(typeof(StringMessage), (short)MsgType.RPCMove));
			currentProtocol.Add(new AlbotMessage(typeof(GameInfo), (short)MsgType.gameInfo));
			currentProtocol.Add (new AlbotMessage (typeof(BoardMsg), (short)MsgType.boardInit));
		}


		public void sendMove(int targetID, string move, Game.PlayerColor color) { sendString(targetID, MsgType.move, move, color); }
		//Server
		public void requestMove(int targetID, BoardMsg bi) { sendMsg(bi, targetID, (short)MsgType.targetBoardUpdate); }
		public void sendRPCMove(int targetID, RPCMove move) { sendMsg(move, targetID, (short)MsgType.RPCMove); }
		public void sendGameInfo(int targetID, GameInfo gi) {sendMsg(gi, targetID, (short)MsgType.gameInfo);}
		public void sendPlayerInit(int targetID, InitSettings inSet) {sendMsg(inSet, targetID, (short)MsgType.playerInit);}
		public void sendBoardInit(int targetID,  BoardMsg bi) {sendMsg(bi, targetID, (short)MsgType.boardInit);}
		public void sendBoardInitRequest(int targetID, string msg, Game.PlayerColor color) {sendString(targetID, MsgType.boardInitRequest, msg, color);}

		private void sendString(int targetID, MsgType type, string str, Game.PlayerColor color) {
			sendMsg(new StringMessage(str, color), targetID, (short)type);
		}

	}

	[Serializable]
	public class BoardMsg{ 
		public char[,] board;
		public Game.PlayerColor color;
		public BoardMsg(char[,] board, Game.PlayerColor color){this.board = board; this.color = color;}
	}
	[Serializable]
	public class RPCMove{ 
		public int move;
		public Game.PlayerColor color;
		public char targetStatus;
		public ShipType targetType;
		public int[] startPos;
		public bool horizontal;
		public RPCMove(int move, Game.PlayerColor color, char targetStatus, ShipType targetType, int[] startPos = null, bool horizontal = false){
			this.move = move; this.color = color; 
			this.targetStatus = targetStatus; this.targetType = targetType;
			this.startPos = startPos; this.horizontal = horizontal;
		}
	}
	[Serializable]
	public class GameInfo{
		public bool gameOver;
		public Game.PlayerColor winnerColor;
		public GameInfo(bool gameOver = false, Game.PlayerColor winnerColor = Game.PlayerColor.None){

			this.gameOver = gameOver;
			this.winnerColor = winnerColor;
		}
	}
	[Serializable]
	public class InitSettings{
		public string username;
		public Game.PlayerColor myColor;
		public InitSettings(string username, Game.PlayerColor myColor){ 
			this.username = username;
			this.myColor = myColor;
		}
	}
}