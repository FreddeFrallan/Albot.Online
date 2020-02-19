using System;
using Game;

namespace Othello{


	public class PlayerMoveHandler{
		public CommProtocol protocol;
		private bool gameOver = false;
		private BoardLogic board;
		private Main.GameMaster mainGame;

		public PlayerMoveHandler(Main.GameMaster mainGame, CommProtocol prot, BoardLogic board){
			this.mainGame = mainGame;
			this.protocol = prot;
			this.board = board;
		}

		public void onPlayerMove(object stringMsg, ConnectedClient c) {
			CommProtocol.StringMessage msg = (CommProtocol.StringMessage)stringMsg;
			mainGame.debugPrint (msg.color + " sent: " + msg.msg);
			ConnectedPlayer p = mainGame.getCurrentPlayer ();
			if (msg.color != p.color || c.peerID != p.client.peerID || gameOver)
				return;			
			//Parse incoming msg
			string moveString = msg.msg;
			int[] move = new int[2];
			try{
				int chosenMove = int.Parse(moveString);
				move = new int[]{chosenMove %8, chosenMove / 8};
			}
			catch(Exception e){
				protocol.requestMove (mainGame.getCurrentPlayer().client.peerID, board.ToString (), mainGame.getCurrentPlayer().color);
				return;
			}

			//See if the move is a legit move
			if (board.getAvailabeMoves (p.color == Game.PlayerColor.White).Find(x => x[0] == move[0] && x[1] == move[1]) == null) {
				protocol.requestMove (mainGame.getCurrentPlayer().client.peerID, board.ToString (),  mainGame.getCurrentPlayer().color);
				return;
			}
				
			board.playMove (move [0], move [1], p.color == Game.PlayerColor.White);
			RPCMove RPCmsg = new RPCMove(move, p.color);
			mainGame.broadcastRPCMove (RPCmsg);

			mainGame.switchToNextPlayer();
			if (gameIsOver ()) {
				gameOver = true;
				mainGame.setWinner (getWinColor ());
				return;
			}

			mainGame.startPlayerTimer ();
			protocol.requestMove (mainGame.getCurrentPlayer().client.peerID, board.ToString (),  mainGame.getCurrentPlayer().color);
		}


		//Check if the current player has any available moves. If not we switch to the other player.
		//If this player also has no moves then we declare that the game is over.
		private bool gameIsOver(){
			if (board.getAvailabeMoves (mainGame.getCurrentPlayer ().color == Game.PlayerColor.White).Count == 0) {
				mainGame.switchToNextPlayer();
				if (board.getAvailabeMoves (mainGame.getCurrentPlayer ().color == Game.PlayerColor.White).Count == 0)
					return true;
			}
			return false;
		}


		private Game.PlayerColor getWinColor(){
			int[] score = board.getScore ();
			if (score [0] == score [1])
				return Game.PlayerColor.None;
			else
				return score [0] > score [1] ? Game.PlayerColor.White : Game.PlayerColor.Black;
		}

	}
}

