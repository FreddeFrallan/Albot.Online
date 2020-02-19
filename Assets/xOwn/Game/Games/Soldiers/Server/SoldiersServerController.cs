using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soldiers{

	public class SoldiersServerController : Game.ServerController {

		public SoldiersGameStateUpdater updater;
		private SoldiersGameMaster controller = new SoldiersGameMaster();

		public override Game.GameMaster getController (){return controller;}
		public override void initController (System.Action<string> printFunc, System.Action<object, int, short> sendMsgFunc, System.Action shutdownGameServer, GameWrapper wrapper, List<string> preGamePlayers){
			controller.updater = updater;
			controller.init (printFunc, sendMsgFunc, shutdownGameServer, wrapper, preGamePlayers);
		}

		void Awake(){
			controller.updater = updater;
			updater.gameMaster = controller;
		}

	}




	public class SoldiersGameMaster: Game.GameMaster {

		public SoldiersGameStateUpdater updater;
		public SoldiersProtocol protocol = new SoldiersProtocol();

		public override Game.GameType getGameType (){return Game.GameType.Soldiers;}
		public override Game.CommProtocol getProtocol (){return protocol;}
		public override bool isRealtime (){return true;}
		protected override void initProtocol (System.Action<object, int, short> sendMsgFunc){
			protocol.init (sendMsgFunc);
			colorOrder = new List<Game.PlayerColor> (){ Game.PlayerColor.Blue, Game.PlayerColor.Red };
		}
		public override int maxNbrPlayers (){return 2;}



		public override ConnectedPlayer onPlayerJoined (ConnectedPlayer newPlayer){
			newPlayer = base.onPlayerJoined (newPlayer);

			GameInfo gi = new GameInfo(newPlayer.username, newPlayer.color);
			protocol.sendPlayerInit (newPlayer.client.peerID, gi);

			protocol.subscribeHandler(newPlayer.client, SoldierServerCommandHandler.moveHandler, (short)SoldiersProtocol.MsgType.playerCommands, wrapper, typeof(PlayerCommands));
			if (nbrPlayers() == maxNbrPlayers())
				startGame();
			return newPlayer;
		}




		public override void startGame (){
			updater.startGame (players);
		}

		public override void onPlayerLeft (ConnectedPlayer oldPlayer){
			Debug.LogError ("Player left: " + oldPlayer.username);
			int leftIndex = colorOrder.FindIndex (x => x == oldPlayer.color);
			int indexOfNotLeavingPlayer = (leftIndex + 1) % colorOrder.Count;
			updater.setGameOver (colorOrder[indexOfNotLeavingPlayer]);
		}


		public void startShutdownServer(){base.shutdownServer ();}
	}

}