using System.Collections;
using System.Collections.Generic;
using AlbotServer;
using UnityEngine;
using Game;

namespace Snake{

	public class SnakeServerController : ServerController {
		
		public SnakeGameStateUpdater updater;
		private SnakeGameMaster controller = new SnakeGameMaster();

		public override GameMaster getController (){return controller;}
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


	public class SnakeGameMaster: GameMaster {

		public SnakeGameStateUpdater updater;
		public SnakeProtocol protocol = new SnakeProtocol();
        private bool gameOver = false;

		public override GameType getGameType (){return GameType.Snake;}
		public override CommProtocol getProtocol (){return protocol;}
		public override bool isRealtime (){return true;}
		protected override void initProtocol (System.Action<object, int, short> sendMsgFunc){
			protocol.init (sendMsgFunc);
			colorOrder = new List<PlayerColor> (){ PlayerColor.Blue, PlayerColor.Red };
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
			updater.startGame (players);
            spectatorModule.onGameStarted(players);
		}

		public override void onPlayerLeft (ConnectedPlayer oldPlayer){
            if (gameOver)
                return;

			Debug.LogError ("Player left: " + oldPlayer.username);
			int leftIndex = colorOrder.FindIndex (x => x == oldPlayer.color);
			int indexOfNotLeavingPlayer = (leftIndex + 1) % colorOrder.Count;
            onGameOver(GameOverState.playerLeft, colorOrder[indexOfNotLeavingPlayer], new int[0][]);
		}

        public void onGameOver(GameOverState state, PlayerColor winColor, int[][] crashPos) {
            if (gameOver)
                return;
            gameOver = true;

            Debug.LogError("Sending gameOver");
            updater.setGameOver(winColor, crashPos);

            if (state == GameOverState.draw) {
                reportGameOver(state);
                return;
            }

            ConnectedPlayer winPlayer = players.Find(p => p.color == winColor);
            ConnectedPlayer losingPlayer = players.Find(p => p.color != winColor);
            reportGameOver(state, new string[] {winPlayer.username, losingPlayer.username });
        }

        public PlayerColor getIndexColor(int index){return colorOrder [index];}
		public void startShutdownServer(){base.shutdownServer ();}
        public void submitToGameLog(string logMsg) { gameLog.pushToRawLog(logMsg); } 
	}
}