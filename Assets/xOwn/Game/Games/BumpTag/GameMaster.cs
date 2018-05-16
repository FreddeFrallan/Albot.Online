using System.Collections;
using System.Collections.Generic;
using Game;
using System.Threading;

namespace BumpTag { 

	public class GameMaster : Game.TurnbasedGame{
		#region implemented abstract members of TurnbasedGame

		public override float maxTimePerMove ()
		{
			throw new System.NotImplementedException ();
		}

		public override void onTimesOut ()
		{
			throw new System.NotImplementedException ();
		}

		#endregion

		//Contructor needed for loading DLL file later.
		public GameMaster(){}
//		private List<PlayerColor> colorOrder = new List<PlayerColor>(){PlayerColor.Red, PlayerColor.Green, PlayerColor.Blue, PlayerColor.Yellow}; 
		private BumpTag.CommProtocol protocol = new CommProtocol();
		private BoardLogic board;
		private bool gameRunning = false;
		private DiceRoller dice = new DiceRoller (1);
			
		public override int maxNbrPlayers (){return 4;}
		public override Game.GameType getGameType (){return GameType.BumtTag;}
		public override bool isRealtime (){return false;}
		public override Game.CommProtocol getProtocol (){return protocol;}
		protected override void initProtocol (System.Action<object, int, short> sendMsgFunc){
			protocol.init (sendMsgFunc); 
			wrapper.init (protocol, this);
		}


		public override void startGame (){
			board = new BoardLogic(players.Count);
			print ("Starting Game: " + players.Count);
			requstMove ();
		}


		//Not finished********
		public void onReceviedMove(object msg, ConnectedPlayer p){
			print ("Msg from: " + p.username);
			if (p != currentPlayer ()) // Not the excpected player
				return;

			
			PlayerMove moveMsg = (PlayerMove)msg;
			print (p.username + " sent MoveMsg: " + moveMsg.steps);
			if (moveMsg.steps != dice.lastValue) { // The amount of steps is somehow corrupt?! resending msg
				requstMove ();
				return;
			}
			BoardLogic.Piece pi = board.getPiece (p.color, moveMsg.pieceID);
			if(board.isValidMove(pi, moveMsg.steps) == false){ // The move is somehow corrupt?! resending msg
				requstMove ();
				return;
			}
				
			board.movePiece (pi, moveMsg.steps);
			nextPlayer ();
			requstMove ();
		}


		private void requstMove(){
			ConnectedPlayer p = currentPlayer ();
			int moveSteps = dice.roll ().totalSum;

			//Skip player if he cannot move!***  Add wait here later so we have time to display some animations
			if (board.hasValidMove (p.color, moveSteps) == false) {
				nextPlayer ();
				requstMove ();
			}

			string reqMsg = moveSteps + " " + board.boardAsString (p.color);
			protocol.requestMove (p.client.peerID, reqMsg);
		}

		public override ConnectedPlayer onPlayerJoined (ConnectedPlayer newPlayer){
			if (players.Count >= maxNbrPlayers ())return newPlayer;//This should never happen? Or should we allow spectators perhaps
			newPlayer.color = colorOrder[players.Count];
			players.Add (newPlayer);
			sendInitMsg (newPlayer);

			if (players.Count == maxNbrPlayers ())
				startGame ();

			return newPlayer;
		}
		private void sendInitMsg(ConnectedPlayer p){
			GameInfo info = new GameInfo (p.color);
			protocol.sendPlayerInit (p.client.peerID, info);
		}

		public override void onPlayerLeft (ConnectedPlayer newPlayer){
			players.Remove (newPlayer);
			if (gameRunning == false) {
				resetPlayerColors ();
				foreach (ConnectedPlayer p in players)
					sendInitMsg (p);
			} else {
				
			}
		}
		//If player leaves, and game has not started we give everyone an appropriate new color
		private void resetPlayerColors(){
			for (int i = 0; i < players.Count; i++)
				players [i].color = colorOrder [i];
		}



	}

} // namespace BumpTag
