using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using System;

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
			
		public void sendBoardUpdate(int[,] grid, Position2D[] playerPos, int[] dir){
			List<Position2D> rCoord = new List<Position2D> (), bCoord = new List<Position2D> ();
			int boardSize = grid.GetLength (0);

			bCoord.AddRange (bodies [0].getRecentPlayerBody ());
			rCoord.AddRange (bodies [1].getRecentPlayerBody ());
            
			bCoord.Add (playerPos[0]);
			rCoord.Add (playerPos[1]);

			BoardUpdate update = new BoardUpdate (updateCounter++, rCoord.ToArray(), bCoord.ToArray(), dir[0], dir[1]);
			broadcastBoard (update);
		}


		private void broadcastBoard(BoardUpdate b){
			foreach (ConnectedPlayer p in players) {
				b.myColor = p.color;
				try{
					protocol.sendBoard (gameMaster.getMatchingPlayer(p.color).client.peerID, b);
				}catch(Exception e){
                    Debug.LogError(e.StackTrace);
                    Debug.LogError(e.Message);
                    Debug.LogError ("Error Send");
                }
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


		public void addNewPlayerBody(Position2D coord, int playerIndex){
			bodies [playerIndex].enqueue (coord);
		}


		private class PlayerBody{
			private const int maxSize = 4;
			private List<Position2D> blocked = new List<Position2D> ();

			public void enqueue(Position2D coord){
				blocked.Insert (0, coord);
				if (blocked.Count > maxSize)
					blocked.RemoveRange (maxSize, blocked.Count - maxSize);
			}

			public List<Position2D> getRecentPlayerBody(){return blocked;}
		}
	}
}