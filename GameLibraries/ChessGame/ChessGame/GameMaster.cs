using System;
using System.Collections.Generic;
using ChessBot;
using Chess;

namespace Main
{
	class GameMaster : Game.TurnbasedGame{
		
		public override Game.GameType getGameType (){return Game.GameType.Chess;}
		public override bool isRealtime (){return false;}
		public override Game.CommProtocol getProtocol (){return protocol;}
		protected override void initProtocol (Action<object, int, short> sendMsgFunc){protocol.init (sendMsgFunc);}
		public override int maxNbrPlayers (){return 2;}
		private CommProtocol protocol = new CommProtocol ();
		private Board theGame;
		private bool gameRunning = false;
		private bool gameOver = false;


		public void onPlayerMove(object msg, ConnectedPlayer p) {
			if (p != currentPlayer() || gameOver)
				return;			

			//Parse msg
			string words = Convert.ToString (msg);
			int[] move = new int[4];
			int[] start = new int[2];
			int[] target = new int[2];
			try{
				start = new int[]{int.Parse(words[0].ToString()), int.Parse(words [1].ToString())};
				target = new int[]{int.Parse(words[2].ToString()), int.Parse(words [3].ToString())};
			}catch(Exception e){
				print ("Corrupt incoming msg: " +  Convert.ToString (msg));
				protocol.requestMove (currentPlayer().peerID, theGame.ToString ());
				return;
			}

			//Check if Valid move
			Move playedMove = MoveHandler.isValidMove (theGame, p.color == Game.PlayerColor.White, start, target);
			if (playedMove == null) {
				print ("Not valid Move: " +  Convert.ToString (msg));
				protocol.requestMove (currentPlayer().peerID, theGame.ToString ());
				return;
			}
			//Play move & Broadcast
			theGame = theGame.playMove(playedMove);
			RPCMove rpcMove = new RPCMove (start, target, p.color);
			foreach (ConnectedPlayer pl in players)
				protocol.sendRPCMove (pl.peerID, rpcMove);


			//Check if gameOver
			nextPlayer();
			BoardStatus status = MoveHandler.getGameState (theGame, currentPlayer ().color == Game.PlayerColor.White);
			if (status != BoardStatus.normal) {
				gameOver = true;
				Game.PlayerColor winColor;
				winColor = status == BoardStatus.draw ? Game.PlayerColor.None : p.color;
				broadcastGameOver (winColor);
			}
			else
				protocol.requestMove (currentPlayer().peerID, theGame.ToString ());
		}

		private void broadcastGameOver(Game.PlayerColor winColor){
			GameInfo status = new GameInfo (0, Game.PlayerColor.None, true, winColor);
			foreach (ConnectedPlayer p in players)
				protocol.sendGameInfo (p.peerID, status);
		}

		public override void startGame (){
			print ("Starting Game");
			theGame = new Board ();
			gameRunning = true;
			protocol.requestMove (currentPlayer().peerID, theGame.ToString ());
		}

		public override ConnectedPlayer onPlayerJoined (ConnectedPlayer newPlayer){
			players.Add (newPlayer);
			newPlayer.color = nbrPlayers() == 1 ? Game.PlayerColor.White : Game.PlayerColor.Black;
			newPlayer.UISlotID = newPlayer.color == Game.PlayerColor.White ? 0 : 1;

			print (newPlayer.UISlotID.ToString());
			var gi = new GameInfo(newPlayer.peerID, newPlayer.color);
			protocol.sendPlayerInit (newPlayer.peerID, gi);

			protocol.subscribeHandler(newPlayer, onPlayerMove, (short)CommProtocol.MsgType.move, wrapper);
			if (nbrPlayers() == maxNbrPlayers())
				startGame();
			return newPlayer;
		}

		public override void onPlayerLeft (ConnectedPlayer newPlayer){
			ConnectedPlayer p = players.Find (x => x.peerID == newPlayer.peerID);
			players.Remove (p);
			if (gameRunning)
				foreach (ConnectedPlayer pl in players) {
					var gi = new GameInfo(pl.peerID, pl.color, true, players[0].color);
					protocol.sendGameInfo(pl.peerID, gi);
				}
		}

	}
}
