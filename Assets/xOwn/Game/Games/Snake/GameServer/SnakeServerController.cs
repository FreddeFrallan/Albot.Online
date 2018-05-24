using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snake{

	public class SnakeServerController : Game.ServerController {
		
		public SnakeGameStateUpdater updater;
		private SnakeGameMaster controller = new SnakeGameMaster();

		public override Game.GameMaster getController (){return controller;}
		public override void initController (System.Action<string> printFunc, System.Action<object, int, short> sendMsgFunc, 
			System.Action shutdownGameServer, GameWrapper wrapper, List<string> preGamePlayers){

			controller.updater = updater;
			controller.init (printFunc, sendMsgFunc, shutdownGameServer, wrapper, preGamePlayers);
		}

		void Awake(){
			controller.updater = updater;
			updater.gameMaster = controller;
		}

	}


	public class SnakeGameMaster: Game.GameMaster {

		public SnakeGameStateUpdater updater;
		public SnakeProtocol protocol = new SnakeProtocol();

		public override Game.GameType getGameType (){return Game.GameType.Snake;}
		public override Game.CommProtocol getProtocol (){return protocol;}
		public override bool isRealtime (){return true;}
		protected override void initProtocol (System.Action<object, int, short> sendMsgFunc){
			protocol.init (sendMsgFunc);
			colorOrder = new List<Game.PlayerColor> (){ Game.PlayerColor.Blue, Game.PlayerColor.Red };
		}
		public override int maxNbrPlayers (){return 2;}



		public override ConnectedPlayer onPlayerJoined (ConnectedPlayer newPlayer){
			newPlayer = base.onPlayerJoined (newPlayer);

			GameInfo gi = new GameInfo(newPlayer.username, newPlayer.color, new int[0][]);
			protocol.sendPlayerInit (newPlayer.client.peerID, gi);

            protocol.subscribeHandler(newPlayer.client, SnakeGameLogic.moveHandler, (short)SnakeProtocol.MsgType.playerCommands, wrapper, typeof(GameCommand));
			if (nbrPlayers() == maxNbrPlayers())
				startGame();
			return newPlayer;
		}




		public override void startGame (){
			gameLog.addInitLogMsg (players);
			updater.startGame (players);
		}

		public override void onPlayerLeft (ConnectedPlayer oldPlayer){
			Debug.LogError ("Player left: " + oldPlayer.username);
			int leftIndex = colorOrder.FindIndex (x => x == oldPlayer.color);
			int indexOfNotLeavingPlayer = (leftIndex + 1) % colorOrder.Count;
			updater.setGameOver (colorOrder[indexOfNotLeavingPlayer], new int[0][]);
		}


		public Game.PlayerColor getIndexColor(int index){return colorOrder [index];}
		public void startShutdownServer(){base.shutdownServer ();}


		public void submitToGameLog(string logMsg){
			gameLog.pushToRawLog (logMsg);
		}
	}
}