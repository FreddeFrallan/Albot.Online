using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace Bomberman{

	public class BombermanServerController : ServerController {

		//public BombermanGameMaster updater;
		public BombermanGameStateUpdater updater;
		public BombermanOverlord overlord;
		private BombermanGameMaster controller = new BombermanGameMaster();

		public override GameMaster getController (){return controller;}
		public override void initController (System.Action<string> printFunc, System.Action<object, int, short> sendMsgFunc, System.Action shutdownGameServer, GameWrapper wrapper, List<string> preGamePlayers){
			controller.updater = updater;
			controller.init (printFunc, sendMsgFunc, shutdownGameServer, wrapper, preGamePlayers);
		}

		void Awake(){
			controller.updater = updater;
			updater.gameMaster = controller;
			overlord.gameMaster = controller;
		}
	}




	public class BombermanGameMaster : GameMaster{

		public BombermanGameStateUpdater updater;
		public BombermanProtocol protocol = new BombermanProtocol();

		public override GameType getGameType (){ return Game.GameType.Bomberman;}
		public override CommProtocol getProtocol (){return protocol;}
		public override bool isRealtime (){return true;}
		public override int maxNbrPlayers (){return 2;}
		protected override void initProtocol (System.Action<object, int, short> sendMsgFunc){
			protocol.init (sendMsgFunc);
			colorOrder = new List<PlayerColor> (){ PlayerColor.Blue, PlayerColor.Red};
		}


		public override ConnectedPlayer onPlayerJoined (ConnectedPlayer newPlayer){
			newPlayer = base.onPlayerJoined (newPlayer);

			GameInfo gi = new GameInfo(newPlayer.username, newPlayer.color);
			protocol.sendPlayerInit (newPlayer.client.peerID, gi);
			protocol.subscribeHandler(newPlayer.client, BombermanOverlord.moveHandler, (short)BombermanProtocol.MsgType.playerCommands, wrapper, typeof(PlayerCommand));

			if (nbrPlayers() == maxNbrPlayers())
				startGame();
			return newPlayer;
		}

		public void onGameOver(PlayerColor winColor){
			updater.sendFinalBoard ();
			GameInfo gi = new GameInfo ("", PlayerColor.None, true, winColor);
			foreach (ConnectedPlayer p in players) {
				try{
					gi.myColor = p.color;
					protocol.sendGameInfo(getMatchingPlayer(p.color).client.peerID, gi);
				}catch{}
			}
			shutdownServer ();
		}



		public override void startGame (){
			Debug.LogError ("Starting Game");
			updater.startGame (players);
		}
		public override void onPlayerLeft (ConnectedPlayer oldPlayer){
			Debug.LogError ("Player left: " + oldPlayer.username);
			int playerGameID = BombermanOverlord.convertColorToTeam (oldPlayer.color);
			PlayerController playerObj = BombermanGameStateUpdater.getMatchingPlayerObj (playerGameID);
			if (playerObj != null)
				playerObj.takeDamage ();
		}
	
	
	}
}