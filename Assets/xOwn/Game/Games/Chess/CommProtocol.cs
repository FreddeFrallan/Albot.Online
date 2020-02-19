using System;
namespace Chess{

	public class CommProtocol : Game.CommProtocol {
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
		public void sendMove(int targetID, string move) { sendString(targetID, MsgType.move, move); }
		public void requestMove(int targetID, string board) { sendString(targetID, MsgType.boardUpdate, board); }
		public void sendRPCMove(int targetID, RPCMove move) { sendMsg(move, targetID, (short)MsgType.RPCMove); }
		public void sendGameInfo(int targetID, GameInfo gi) {sendMsg(gi, targetID, (short)MsgType.gameInfo);}
		public void sendPlayerInit(int targetID, GameInfo gi) {sendMsg(gi, targetID, (short)MsgType.playerInit);}

		private void sendString(int targetID, MsgType type, string str) {
			sendMsg(new StringMessage(str), targetID, (short)type);
		}

	}

	[Serializable]
	public class RPCMove{ 
		public int[] start, target;
		public Game.PlayerColor color;
		public RPCMove(int[] start, int[] target, Game.PlayerColor color){this.start = start;this.target = target;  this.color = color;}
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

} // namespace Connect4
