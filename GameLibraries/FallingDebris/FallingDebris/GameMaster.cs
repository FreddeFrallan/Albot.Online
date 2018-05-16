using System;
using System.Diagnostics;
using FallingDebris;

namespace Main
{
	public class GameMaster : Game.RealtimeGame
	{
		//Contructor needed for loading DLL file later.
		public GameMaster (){}

		public int playerId = 0;
		public override bool isRealtime (){return true;}
		public static CommProtocol protocol = new CommProtocol();
		public override Game.GameType getGameType (){return Game.GameType.FallingDebris;}
		public override Game.CommProtocol getProtocol (){return protocol;}
		protected override void initProtocol (Action<object, int, short> sendMsgFunc){protocol.init (sendMsgFunc);}
		public override int maxNbrPlayers (){return 1;}
		public GameLogic logic = new GameLogic();
		private Stopwatch gameTimer = new Stopwatch ();
		public static Action<string> localPrint;


		public void onPlayerMove(object msg, ConnectedPlayer p){
			string moveString = Convert.ToString (msg).Trim();
			try{
				int target = int.Parse(moveString);
				logic.setTargetpos(target);
			}
			catch(Exception e){}
		}

		public override void startGame (){
			localPrint = print;
			logic.thePlayerID = players [0].peerID;
			logic.print = print;
			print ("Starting Game");
			gameTimer.Start ();
			logic.startGameLogic ();
		}

		public override void Update (){
			double[] playerState = logic.getPlayerState ();

			GameState msg = new GameState ((float)playerState[0], (int)playerState[1], gameTimer.ElapsedMilliseconds);
			foreach (ConnectedPlayer p in players)
				protocol.sendGameState (p.peerID, msg);
		}


		public override ConnectedPlayer onPlayerJoined (ConnectedPlayer newPlayer){
			players.Add (newPlayer);
			newPlayer.color = Game.PlayerColor.White;
			var gi = new GameInfo(newPlayer.peerID, newPlayer.color, false, Game.PlayerColor.None);
			protocol.sendPlayerInit (newPlayer.peerID, gi);
			playerId = newPlayer.peerID;

			protocol.subscribeHandler (newPlayer, onPlayerMove, (short)CommProtocol.MsgType.move, wrapper);
			startGame ();
			return newPlayer;
		}


		public override void onPlayerLeft (ConnectedPlayer newPlayer){
			print ("Player left");
			onGameOver ();
			logic.gameOver ();
		}


		public void sendGameState(GameState msg){
			foreach (ConnectedPlayer p in players)
				protocol.sendGameState (p.peerID, msg);
		}
	}
}

