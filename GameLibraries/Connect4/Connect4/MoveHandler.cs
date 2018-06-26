using System;

namespace Connect4{
	public class MoveHandler{

		public CommProtocol protocol;
		private bool gameOver = false;
		private BoardLogic board;
		private Main.GameMaster mainGame;
		private int moveCounter = 0;

		public MoveHandler(Main.GameMaster mainGame, CommProtocol prot, BoardLogic board){
			this.mainGame = mainGame;
			this.protocol = prot;
			this.board = board;
		}
			
		public void onPlayerMove(object stringMsg, ConnectedClient c) {
			CommProtocol.StringMessage msg = (CommProtocol.StringMessage)stringMsg;
			ConnectedPlayer p = mainGame.getCurrentPlayer ();
			mainGame.debugPrint ("Incoming from: " + msg.color);
			if (msg.color != p.color || c.peerID != p.client.peerID|| gameOver)
				return;			

			string move = msg.msg;
			int col;
			if (!int.TryParse(move, out col) || !board.isAllowedMove(col)) {
				protocol.requestMove(p.client.peerID, board.ToString(), mainGame.getCurrentPlayer().color);
				return;
			}

			//Valid move was made
			mainGame.pushToRawLog(move);
			moveCounter++;
			board.dropPiece(p.color == Game.PlayerColor.Yellow ? BoardLogic.Piece.Yellow : BoardLogic.Piece.Red, col);
			RPCMove RPCmsg = new RPCMove(int.Parse(move), p.color);
			mainGame.broadcastRPCMove (RPCmsg);

			Game.PlayerColor winColor = board.hasWinner();
			if (winColor != Game.PlayerColor.None || moveCounter == 42) {
				mainGame.setGameOver (winColor);
				return;
			}

			mainGame.switchToNextPlayer();
			mainGame.startPlayerTimer ();
			protocol.requestMove (mainGame.getCurrentPlayer().client.peerID, board.ToString (), mainGame.getCurrentPlayer().color);
		}


        private void foo() {
            bar((int x) => { return x * x; });
        }

        private void bar(Func<int, int> a) {
            int result = a(10);
        }

	}
}

