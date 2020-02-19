using System;
using Othello;

namespace Main{
	public class GameMaster : Game.TurnbasedGame {
		//Contructor needed for loading DLL file later.
		public GameMaster (){
			moveHandler = new PlayerMoveHandler (this, protocol, board);
			colorOrder = new System.Collections.Generic.List<Game.PlayerColor> (){ Game.PlayerColor.White, Game.PlayerColor.Black };
		}

		public override bool isRealtime (){return false;}
		public CommProtocol protocol = new CommProtocol();
		public override Game.GameType getGameType (){return Game.GameType.Othello;}
		public override Game.CommProtocol getProtocol (){return protocol;}
		protected override void initProtocol (Action<object, int, short> sendMsgFunc){protocol.init (sendMsgFunc);}
		public override int maxNbrPlayers (){return 2;}

		private PlayerMoveHandler moveHandler;
		private Othello.BoardLogic board = new Othello.BoardLogic();
		private bool gameRunning = false;

		public override void startGame (){
			print("Starting game");
			gameRunning = true;
			startTimer ();
			startPlayerTimer ();
			protocol.requestMove (currentPlayer().client.peerID, board.ToString (), currentPlayer().color);
		}

		public override ConnectedPlayer onPlayerJoined (ConnectedPlayer newPlayer){
			newPlayer = base.onPlayerJoined (newPlayer);

			GameInfo gi = new GameInfo(newPlayer.username, newPlayer.color);
			protocol.sendPlayerInit (newPlayer.client.peerID, gi);
			initPlayerTimer (maxTimePerMove(), newPlayer.color, (Game.TurnBasedCommProtocol)protocol);

			protocol.subscribeHandler(newPlayer.client, moveHandler.onPlayerMove, (short)CommProtocol.MsgType.move, wrapper);
			if (nbrPlayers() == maxNbrPlayers())
				startGame();
			return newPlayer;
		}

		public override void onPlayerLeft (ConnectedPlayer newPlayer){
			ConnectedPlayer p = players.Find (x => x.client.peerID == newPlayer.client.peerID);

			players.Remove (p);
			if (gameRunning)
				setWinner (players [0].color);
			else if(players.Count == 0)
				base.shutdownGameServer ();
		}


		#region Timers
		public override float maxTimePerMove (){return 10;}
		public override void onTimesOut (){
			nextPlayer ();
			setWinner (currentPlayer ().color);
		}
		#endregion

		#region Internal getters & setters
		public ConnectedPlayer getCurrentPlayer(){return currentPlayer();}
		public void switchToNextPlayer(){nextPlayer();}
		public void startPlayerTimer(){broadcastStartTimer (currentPlayer ().color, maxTimePerMove (), (Game.TurnBasedCommProtocol)protocol);}

		public void broadcastRPCMove(RPCMove move){
			foreach (ConnectedClient c in clients)
				protocol.sendRPCMove (c.peerID, move);
		}
		public void setWinner(Game.PlayerColor winColor){
			foreach (ConnectedClient c in clients)
				protocol.sendGameInfo (c.peerID, new GameInfo ("", Game.PlayerColor.None, true, winColor));
			base.shutdownGameServer ();
		}
		#endregion


		public void debugPrint(string msg){print (msg);}
	}
}

