using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

namespace FallingDebris{
	
	public class FallingGameMaster : Game.RealtimeGame {

		public int playerId = 0;
		public override bool isRealtime (){return true;}
		public static CommProtocol protocol = new CommProtocol();
		public override Game.GameType getGameType (){return Game.GameType.FallingDebris;}
		public override Game.CommProtocol getProtocol (){return protocol;}
		protected override void initProtocol (Action<object, int, short> sendMsgFunc){protocol.init (sendMsgFunc);}
		public override int maxNbrPlayers (){return 1;}
		public FallingGameLogic logic = new FallingGameLogic();
		private Stopwatch gameTimer = new Stopwatch ();
		public static Action<string> localPrint;


		public void onPlayerMove(object msg, ConnectedClient c){
			string moveString = Convert.ToString (msg).Trim();
			try{
				int target = int.Parse(moveString);
				logic.setTargetpos(target);
			}
			catch{}
		}

		public override void startGame (){
			localPrint = print;
			logic.thePlayerID = players [0].client.peerID;
			logic.print = print;
			print ("Starting Game");
			gameTimer.Start ();
			logic.startGameLogic ();
		}

		public override void Update (){
			double[] playerState = logic.getPlayerState ();

			GameState msg = new GameState ((float)playerState[0], (int)playerState[1], gameTimer.ElapsedMilliseconds);
			foreach (ConnectedPlayer p in players)
				protocol.sendGameState (p.client.peerID, msg);
		}


		public override ConnectedPlayer onPlayerJoined (ConnectedPlayer newPlayer){
			players.Add (newPlayer);
			newPlayer.color = Game.PlayerColor.White;
			var gi = new GameInfo(newPlayer.client.peerID, newPlayer.color, false, Game.PlayerColor.None);
			protocol.sendPlayerInit (newPlayer.client.peerID, gi);
			playerId = newPlayer.client.peerID;

		//	protocol.subscribeHandler (newPlayer.client, onPlayerMove, (short)CommProtocol.MsgType.move, wrapper);
			startGame ();
			return newPlayer;
		}


		public override void onPlayerLeft (ConnectedPlayer newPlayer){
			print ("Player left");
			base.shutdownGameServer ();
			logic.gameOver ();
		}


		public void sendGameState(GameState msg){
			foreach (ConnectedPlayer p in players)
				protocol.sendGameState (p.client.peerID, msg);
		}
	}

}