using System;
using Game;

namespace BumpTag{
	public class CommProtocol : Game.CommProtocol {
		public enum MsgType : short {
			move = 800,
			boardUpdate,
			playerInit,
			RPCMove,
			gameInfo,
		}

		public override void init (System.Action<object, int, short> sendMsgFunc){
			sendMsg = sendMsgFunc;
			currentProtocol.Add(new AlbotMessage(typeof(StringMessage), (short)MsgType.move));
			currentProtocol.Add(new AlbotMessage(typeof(StringMessage), (short)MsgType.boardUpdate));
			currentProtocol.Add(new AlbotMessage(typeof(GameInfo), (short)MsgType.playerInit));
			currentProtocol.Add(new AlbotMessage(typeof(StringMessage), (short)MsgType.RPCMove));
			currentProtocol.Add(new AlbotMessage(typeof(GameInfo), (short)MsgType.gameInfo));
		}

		//Server
		public void sendMove(int targetID, string move) { sendString(targetID, MsgType.move, move); }
		public void requestMove(int targetID, string board) { sendString(targetID, MsgType.boardUpdate, board); }
		public void sendRPCMove(int targetID, PlayerMove move) { sendMsg(move, targetID, (short)MsgType.RPCMove); }
		public void sendGameInfo(int targetID, GameInfo gi) {sendMsg(gi, targetID, (short)MsgType.gameInfo);}
		public void sendPlayerInit(int targetID, GameInfo gi) {sendMsg(gi, targetID, (short)MsgType.playerInit);}

		private void sendString(int targetID, MsgType type, string str) {
			sendMsg(new StringMessage(str), targetID, (short)type);
		}

	}
	[Serializable]
	public class GameInfo{
		public PlayerColor myColor;
		public bool gameOver;
		public PlayerColor winnerColor;
		public GameInfo(PlayerColor myColor, bool gameOver = false, PlayerColor winnerColor = PlayerColor.None){
			this.myColor = myColor;
			this.gameOver = gameOver;
			this.winnerColor = winnerColor;
		}
	}
	[Serializable]
	public class PlayerMove{
		public PlayerColor color;
		public int pieceID;
		public int steps;
		public PlayerMove(PlayerColor color, int pieceID, int steps){
			this.color = color;
			this.pieceID = pieceID;
			this.steps = steps;
		}
	}
}