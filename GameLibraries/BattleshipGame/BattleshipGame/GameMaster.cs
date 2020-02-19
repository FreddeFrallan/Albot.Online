using System;
using Battleship;
using BattleshipGame;

namespace Main{

	public class GameMaster : Game.TurnbasedGame {
		private string initString = "GameStarted please send your startboard";

		public override int maxNbrPlayers() { return 2; }
		public override bool isRealtime (){return false;}
		public override Game.GameType getGameType (){return Game.GameType.Battleship;}
	
		private GameState currentGameState = GameState.waitingForPlayers;
		private GameLogic theGameLogic;
		public static GameMaster singleton;


		public BattleshipProtocol protocol = new BattleshipProtocol();
		protected override void initProtocol (Action<object, int, short> sendMsgFunc){protocol.init (sendMsgFunc);}
		public override Game.CommProtocol getProtocol (){return protocol;}


		//Contructor needed for loading DLL file later.
		public GameMaster(){
			singleton = this;
			colorOrder = new System.Collections.Generic.List<Game.PlayerColor> (){ Game.PlayerColor.Blue, Game.PlayerColor.Red };
		}

		public void onPlayerMove(object stringMsg, ConnectedClient c) {
			#region init stage
			Game.CommProtocol.StringMessage msg = (Game.CommProtocol.StringMessage)stringMsg;
			ConnectedPlayer p = getMatchingPlayer(msg.color);
			if (currentGameState == GameState.initStage) {
				theGameLogic.handleInitMsg (msg.msg, p);
				return;
			}
			#endregion
			if (currentGameState != GameState.running || p != currentPlayer())
				return;			

			string moveString = msg.msg;
			int moveTarget;
			bool validMove = false;

			if (int.TryParse (moveString, out moveTarget))
				validMove = theGameLogic.playMove (moveTarget, p);
			if(currentGameState == GameState.gameOver) //If somebody played a winning move we quit
				return;


			if (validMove) {
				nextPlayer();
				broadcastStartTimer (currentPlayer ().color, maxTimePerMove (), (Game.TurnBasedCommProtocol)protocol);
			}
			sendRequestMove(currentPlayer());
		}


		public override void startGame() {
			theGameLogic = new GameLogic(this);
			currentGameState = GameState.initStage;

			sendRequstInitBoard (currentPlayer());
		}
		public void sendPlayerTwoInitMsg(){
			nextPlayer ();
			sendRequstInitBoard (currentPlayer ());
		}
		public void startToRunGame(){
			print ("Starting to run game");
			nextPlayer ();
			currentGameState = GameState.running;
			startTimer ();
			broadcastStartTimer (currentPlayer ().color, maxTimePerMove (), (Game.TurnBasedCommProtocol)protocol);

			//Broadcast initBoards 
			foreach (ConnectedPlayer p in players) {
				Player gamePlayer = theGameLogic.getMatchingBattleshipPlayer (p);
				BoardMsg initMsg = new BoardMsg (gamePlayer.getUnformatedBoard (), p.color);
				protocol.sendBoardInit (p.client.peerID, initMsg);
			}
				
			sendRequestMove (currentPlayer ());
		}


		public void broadcastRPCMove(FireResult result, ConnectedPlayer playingPlayer){
			RPCMove move;
			if(result.statusType == FireType.sunk)
				move = new RPCMove (result.target, playingPlayer.color, Player.fireTypeToChar(result.statusType),result.ship.type, result.ship.startPos, result.ship.horizontal);
			else
				move = new RPCMove (result.target, playingPlayer.color, Player.fireTypeToChar(result.statusType), ShipType.battle); //Send Battleship if not sunk, because we don't wanna send all that sweet info just yet.

			foreach (ConnectedClient c in clients)
				protocol.sendRPCMove (c.peerID, move);
		}

		public void sendRequstInitBoard(ConnectedPlayer p){protocol.sendBoardInitRequest (p.client.peerID, initString, p.color);}
		public void sendRequestMove(ConnectedPlayer p){
			Player gamePlayer = theGameLogic.getMatchingBattleshipPlayer (p.color);
			BoardMsg requestMsg = new BoardMsg (gamePlayer.getUnformatedTargetBoard(), p.color);
			protocol.requestMove (p.client.peerID, requestMsg);
		}

		public override ConnectedPlayer onPlayerJoined(ConnectedPlayer p) {
			p = base.onPlayerJoined (p);

			var inSet = new InitSettings(p.username, p.color);
			protocol.sendPlayerInit (p.client.peerID, inSet);
			initPlayerTimer (maxTimePerMove (), p.color, (Game.TurnBasedCommProtocol)protocol);
			protocol.subscribeHandler(p.client, onPlayerMove, (short)BattleshipProtocol.MsgType.move, wrapper);

			if (nbrPlayers() == maxNbrPlayers())
				startGame();
			return p;
		}

		public override void onPlayerLeft (ConnectedPlayer newPlayer){
			ConnectedPlayer p = players.Find (x => x.client.peerID == newPlayer.client.peerID);
			players.Remove (p);

			//Broadcast GameOver state
			if (currentGameState == GameState.running)
				setGameOver (players [0]);
		}

		public void setGameOver(ConnectedPlayer winningPlayer){
			currentGameState = GameState.gameOver;

			//Broadcast GameOver state
			foreach (ConnectedPlayer p in players)
				protocol.sendGameInfo (p.client.peerID,  new GameInfo (true, winningPlayer.color));
			base.shutdownGameServer ();
		}
			
		public override float maxTimePerMove (){return 10;}
		public override void onTimesOut (){
			nextPlayer ();
			setGameOver(currentPlayer());
		}
		public void debugPrint(string msg){
			print (msg);
		}

	}
}


public enum GameState{
	running,
	waitingForPlayers,
	gameOver,
	initStage,
}
