using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snake{

	public class SnakeGameStateUpdater : MonoBehaviour {

		private List<ConnectedPlayer> players;

		public SnakeProtocol protocol;
		public SnakeGameMaster gameMaster;
		public SnakeGameLogic gameLogic;
		private uint updateCounter = 1;
		private PlayerBody[] bodies = new PlayerBody[2];

		public void startGame(List<ConnectedPlayer> players){
			protocol = gameMaster.protocol;
			this.players = players;
			bodies [0] = new PlayerBody ();
			bodies [1] = new PlayerBody ();
			gameLogic.startGame (gameMaster);
		}
			
		public void sendBoardUpdate(int[,] grid, Vector2[] playerPos, int[] dir){
			List<int> rCoord = new List<int> (), bCoord = new List<int> ();
			int boardSize = grid.GetLength (0);


			bCoord.AddRange (bodies [0].getRecentPlayerBody ());
			rCoord.AddRange (bodies [1].getRecentPlayerBody ());

			Vector2 p1 = playerPos [0], p2 = playerPos [1];
			bCoord.Add ((int)(p1.y * boardSize + p1.x));
			rCoord.Add ((int)(p2.y * boardSize + p2.x));

			BoardUpdate update = new BoardUpdate (updateCounter++, rCoord.ToArray(), bCoord.ToArray(), dir[0], dir[1]);
			broadcastBoard (update);
		}


		private void broadcastBoard(BoardUpdate b){
			foreach (ConnectedPlayer p in players) {
				b.myColor = p.color;
				try{
					protocol.sendBoard (gameMaster.getMatchingPlayer(p.color).client.peerID, b);
				}catch{Debug.LogError ("Error Send");}
			}
		}
			

		public void setGameOver(Game.PlayerColor winColor, int[][] crashPos){
			foreach (ConnectedPlayer p in players) {
				GameInfo infoMsg = new GameInfo (p.username, p.color, crashPos, true, winColor);
				try{protocol.sendGameInfo (gameMaster.getMatchingPlayer(p.color).client.peerID, infoMsg);}
				catch{}
			}
				
			gameLogic.gameOver ();
			gameMaster.startShutdownServer ();
		}


		public void addNewPlayerBody(int coord, int playerIndex){
			bodies [playerIndex].enqueue (coord);
		}


		private class PlayerBody{
			private const int maxSize = 4;
			private List<int> blocked = new List<int> ();

			public void enqueue(int coord){
				blocked.Insert (0, coord);
				if (blocked.Count > maxSize)
					blocked.RemoveRange (maxSize, blocked.Count - maxSize);
			}

			public List<int> getRecentPlayerBody(){return blocked;}
		}
	}
}