using System.Collections;
using System.Collections.Generic;
using Game;
using BumpTag;
using System.Threading;

namespace Main{

	public class GameMaster : Game.TurnbasedGame {
		//Contructor needed for loading DLL file later.
		public GameMaster(){}


		private List<PlayerColor> colorOrder = new List<PlayerColor>(){PlayerColor.Red, PlayerColor.Green, PlayerColor.Blue, PlayerColor.Yellow}; 
		private BumpTag.CommProtocol protocol = new BumpTag.CommProtocol();
		private BoardLogic board;
		private bool gameRunning = false;
		private DiceRoller dice = new DiceRoller (1);

		public override int maxNbrPlayers (){return 2;}
		public override Game.GameType getGameType (){return GameType.BumtTag;}
		public override Game.CommProtocol getProtocol (){return protocol;}
		protected override void initProtocol (System.Action<object, int, short> sendMsgFunc){protocol.init (sendMsgFunc); }
			


		public override void startGame (){
			Thread.Sleep (1000);
			board = new BoardLogic(players.Count);
			print ("Starting Game: ");
			requstMove ();
		}


		public void onReceviedMove(object msg, Player p){
			print ("Msg from: " + p.username);
			if (p != currentPlayer ()) // Not the excpected player
				return;


			PlayerMove moveMsg = (PlayerMove)msg;
			if (moveMsg.steps != dice.lastValue) { // The amount of steps is somehow corrupt?! resending msg
				requstMove ();
				return;
			}
			BoardLogic.Piece piece = board.getPiece (p.color, moveMsg.pieceID);
			if(board.isValidMove(piece, moveMsg.steps) == false){ // The move is somehow corrupt?! resending msg
				requstMove ();
				return;
			}
			board.movePiece (piece, moveMsg.steps);
			print ("Sending RPC move");
			foreach(Player pl in players)
				protocol.sendRPCMove (pl.id, new PlayerMove(p.color, moveMsg.pieceID, piece.pos));

			Thread.Sleep (200);
			nextPlayer ();
			requstMove ();
		}


		private void requstMove(){
			Player p = currentPlayer ();
			int moveSteps = dice.roll ().totalSum;

			//Skip player if he cannot move!***  Add wait here later so we have time to display some animations
			if (board.hasValidMove (p.color, moveSteps) == false) {
				nextPlayer ();
				requstMove ();
			}

			string reqMsg = moveSteps + " " + board.boardAsString (p.color);
			protocol.requestMove (p.id, reqMsg);
			print ("Requsting move from: " + p.username);
		}

		public override void onPlayerJoined (Player newPlayer){
			if (players.Count >= maxNbrPlayers ())return;//This should never happen? Or should we allow spectators perhaps
			newPlayer.color = colorOrder[players.Count];
			players.Add (newPlayer);
			protocol.subscribeHandler (newPlayer, onReceviedMove, (short)BumpTag.CommProtocol.MsgType.move, wrapper);
			sendInitMsg (newPlayer);

			if (players.Count == maxNbrPlayers ())
				startGame ();
		}
		private void sendInitMsg(Player p){
			GameInfo info = new GameInfo (p.color, false, PlayerColor.None);
			protocol.sendPlayerInit (p.id, info);
		}

		public override void onPlayerLeft (Player newPlayer){
			players.Remove (newPlayer);
			if(gameRunning == false){
				resetPlayerColors ();
				foreach (Player p in players)
					sendInitMsg (p);
			}
		}
		//If player leaves, and game has not started we give everyone an appropriate new color
		private void resetPlayerColors(){
			for (int i = 0; i < players.Count; i++)
				players [i].color = colorOrder [i];
		}



	}

} // namespace BumpTag
