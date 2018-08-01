using System.Collections.Generic;
using System;
using Connect4;
using System.Threading;

namespace Main{

	public class GameMaster : Game.TurnbasedGame {
		public override int maxNbrPlayers() { return 2; }
		public override bool isRealtime (){return false;}
		public override Game.GameType getGameType (){return Game.GameType.Connect4;}
		private bool gameRunning = false;



		public CommProtocol protocol = new CommProtocol();
		protected override void initProtocol (Action<object, int, short> sendMsgFunc){protocol.init (sendMsgFunc);}
		public override Game.CommProtocol getProtocol (){return protocol;}

		private Connect4.BoardLogic mBoard = new Connect4.BoardLogic();
		private MoveHandler moveHandler;

		//Contructor needed for loading DLL file later.
		public GameMaster(){
			moveHandler = new MoveHandler (this, protocol, mBoard);
			colorOrder = new List<Game.PlayerColor> (){ Game.PlayerColor.Yellow, Game.PlayerColor.Red };
		}

	
		public override void startGame() {
			print("Starting game");
			gameRunning = true;
			broadcastStartTimer (currentPlayer ().color, maxTimePerMove(), (Game.TurnBasedCommProtocol)protocol);
			protocol.requestMove (currentPlayer().client.peerID, mBoard.ToString (), currentPlayer ().color);
			startTimer ();
		}

		public override ConnectedPlayer onPlayerJoined(ConnectedPlayer p) {
			p = base.onPlayerJoined (p);

			protocol.sendPlayerInit(p.client.peerID, new GameInfo(p.username, p.color));
			initPlayerTimer (maxTimePerMove(), p.color, (Game.TurnBasedCommProtocol)protocol);
			protocol.subscribeHandler(p.client, moveHandler.onPlayerMove, (short)CommProtocol.MsgType.move, wrapper);
		
			if (nbrPlayers() == maxNbrPlayers())
				startGame();
			return p;
		}

		public override void onPlayerLeft (ConnectedPlayer newPlayer){
			ConnectedPlayer p = players.Find (x => x.client.peerID == newPlayer.client.peerID);

			players.Remove (p);
			if (gameRunning)
				setGameOver (players [0].color);
		}

		public void setGameOver(Game.PlayerColor winColor){
			foreach (ConnectedClient c in clients) {
				var gi = new GameInfo ("", Game.PlayerColor.None, true, winColor);
				protocol.sendGameInfo (c.peerID, gi);
			}
			base.shutdownGameServer ();
		}


		public override float maxTimePerMove (){return 10;}
		public override void onTimesOut (){
			nextPlayer ();
			setGameOver (currentPlayer ().color);
		}



		#region Internal getters & setters
		public ConnectedPlayer getCurrentPlayer(){return currentPlayer();}
		public void switchToNextPlayer(){nextPlayer();}
		public void startPlayerTimer(){broadcastStartTimer (currentPlayer ().color, maxTimePerMove (), (Game.TurnBasedCommProtocol)protocol);}

		public void broadcastRPCMove(RPCMove move){
			foreach (ConnectedClient c in clients)
				protocol.sendRPCMove (c.peerID, move);
		}
		public void debugPrint(string msg){print (msg);}
		public void pushToRawLog(string msg){gameLog.pushToRawLog (msg);}
		#endregion
	}



} // namespace Connect4
