using System;
using BreakthroughGame;
using Breakthrough;

namespace Main{
	
	public class GameMaster : Game.TurnbasedGame{
		public override Game.GameType getGameType (){return Game.GameType.Breakthrough;}
		public override bool isRealtime (){return false;}
		public override int maxNbrPlayers (){return 2;}
		public override float maxTimePerMove (){return 10;}
		public override Game.CommProtocol getProtocol (){return protocol;}
		protected override void initProtocol (Action<object, int, short> sendMsgFunc){protocol.init (sendMsgFunc);}
		private int playerColorToInt(Game.PlayerColor c){return c == Game.PlayerColor.White ? 1 : -1;}
		private int currentMoveNumber = 0;


		public static void sPrint(string s){singleton.print (s);}
		private static GameMaster singleton;
		private GameLogic logic;
		private MoveHandler moveHandler;
		private BrakethroughProtocol protocol = new BrakethroughProtocol();
		private bool gameOver = false;

		//Contructor needed for loading DLL file later.
		public GameMaster(){
			colorOrder = new System.Collections.Generic.List<Game.PlayerColor> (){ Game.PlayerColor.White, Game.PlayerColor.Black };
			logic = new GameLogic (8, 8);
			moveHandler = new MoveHandler (logic);
			singleton = this;
		}



		public void onPlayerMove(object obj, ConnectedClient c){
			Game.CommProtocol.StringMessage moveMsg = (Game.CommProtocol.StringMessage)obj;
			if (gameOver || moveMsg.color != currentPlayer ().color)
				return;

			int[] start = new int[2], target = new int[2];
			if(moveHandler.checkIfValidMove(moveMsg.msg, ref start, ref target, playerColorToInt(moveMsg.color))){
				logic.makeMove (start, target, playerColorToInt (moveMsg.color));
				currentMoveNumber++;
				broadcastBoard ();
				gameLog.pushToRawLog (moveMsg.msg);

				if (logic.winCheck ()) {
					victory ();
					return;
				}
				nextPlayer();
			}
				
			requestMove ();
		}

		private void broadcastBoard(){
			BoardMsg msg = new BoardMsg (logic.getRawBoard (), currentPlayer().color, currentMoveNumber);
			foreach (ConnectedPlayer p in players) 
				protocol.updateBoard(p.client.peerID, msg);
		}

		private void requestMove(){
			startTimer ();
			BoardMsg msg = new BoardMsg (logic.getRawBoard (), currentPlayer().color, currentMoveNumber);
			protocol.requestMove(getMatchingPlayer (currentPlayer ().color).client.peerID, msg);
		}

		private void victory(){
			if (gameOver)
				return;
			gameOver = true;

			GameInfo gi = new GameInfo ("", Game.PlayerColor.None, true, currentPlayer ().color);
			foreach (ConnectedPlayer p in players) 
				protocol.sendGameInfo (p.client.peerID, gi);

			shutdownGameServer ();
		}


		public override void startGame (){
			//startTimer ();
			print("Starting game");
			broadcastBoard();
			requestMove ();
		}


			
		public override ConnectedPlayer onPlayerJoined (ConnectedPlayer newPlayer){
			newPlayer = base.onPlayerJoined (newPlayer);

			GameInfo gi = new GameInfo(newPlayer.username, newPlayer.color);
			protocol.sendPlayerInit (newPlayer.client.peerID, gi);
			//initPlayerTimer (maxTimePerMove(), newPlayer.color, (Game.CommProtocol)protocol);

			protocol.subscribeHandler(newPlayer.client, onPlayerMove, (short)BrakethroughProtocol.MsgType.move, wrapper);
			if (nbrPlayers() == maxNbrPlayers())
				startGame();
			return newPlayer;
		}


		public override void onPlayerLeft (ConnectedPlayer newPlayer){
			if (newPlayer.color == currentPlayer ().color)
				nextPlayer ();
			victory ();
		}

		public override void onTimesOut (){
			nextPlayer ();
			victory ();
		}
	}
}
