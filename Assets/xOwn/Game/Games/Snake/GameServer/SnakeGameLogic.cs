using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using System;

namespace Snake{

	public class SnakeGameLogic : MonoBehaviour {
		public static readonly float refreshRate = 0.5f;

        private System.Random rand = new System.Random();
		private static SnakeGameLogic singleton;
		public SnakeRenderer renderer;
		public SnakeGameStateUpdater updater;
		public SnakeGameMaster master;

		private int gridSize = 20;
		private int[,] gameGrid;
		private bool gameRunning = true;

		private Vector2[] playerPos = new Vector2[2];
		private Vector2[] crashPos = new Vector2[2];
		private int[] dir = new int[]{ 0, 2 };


		#region init
		public void startGame (SnakeGameMaster master) {
			this.master = master;
			singleton = this;
			initGameGrid ();
			updater.sendBoardUpdate (gameGrid, playerPos, dir);
			StartCoroutine (gameLoop ());
		}

		private void initGameGrid(){
			gameGrid = new int[gridSize, gridSize];
			for (int x = 0; x < gridSize; x++)
				for (int y = 0; y < gridSize; y++)
					gameGrid [x, y] = 0;
		
			generateRandomStartPos ();
		}

		private void generateRandomStartPos(){
			int x = rand.Next (2, gridSize/2-3);
			int y = rand.Next (2, gridSize/2-3);

			playerPos [0] = new Vector2 (x, y);
			gameGrid [(int)playerPos[0].x, (int)playerPos[0].y] = 1;
			submitPlayerBody (playerPos[0], 0);

			playerPos [1] = new Vector2 (gridSize - x-1, gridSize - y-1);
			gameGrid [(int)playerPos[1].x, (int)playerPos[1].y] = 2;
			submitPlayerBody (playerPos[1], 1);

			if (rand.Next (0, 100) >= 50)
				dir = new int[]{ 0, 2 };
			else
				dir = new int[]{ 1, 3};
			
			submitToGameLog ();
		}

		#endregion
			

		private IEnumerator gameLoop(){
			while (gameRunning) {
				yield return new WaitForSeconds (refreshRate);
				gameTick ();
				if (gameRunning)
					updater.sendBoardUpdate (gameGrid, playerPos, dir);
			}
		}


		private void gameTick(){
			Vector2 tPos1 = Vector2.zero, tPos2 = Vector2.zero;
			bool crash1 = movePlayer (0, ref tPos1);
			bool crash2 = movePlayer (1, ref tPos2);

			if (crash1 || crash2) {
				setPlayerCrash (crash1, crash2);
				return;
			}
			if (compPos (tPos1, tPos2)) {
				setPlayerCrash (true, true);
				return;
			}

			//Update game board
			gameGrid [(int)tPos1.x, (int)tPos1.y] = 1;
			gameGrid [(int)tPos2.x, (int)tPos2.y] = 2;
			playerPos [0] = tPos1;
			playerPos [1] = tPos2;

			submitPlayerBody (playerPos[0], 0);
			submitPlayerBody (playerPos[1], 1);
			submitToGameLog ();
		}

		private bool movePlayer(int playerIndex, ref Vector2 targetPos){
			Vector2 currentPos = playerPos [playerIndex];
			targetPos = new Vector2 (currentPos.x, currentPos.y);

			switch (dir [playerIndex]) {
				case 0: targetPos.x++; break;
				case 1: targetPos.y++; break;
				case 2: targetPos.x--; break;
				case 3: targetPos.y--; break;
			}
			crashPos[playerIndex] = new Vector2(targetPos.x, targetPos.y);

			if (targetPos.x < 0 || targetPos.x >= gridSize || targetPos.y < 0 || targetPos.y >= gridSize) //Out of map
				return true;
			if (gameGrid [(int)targetPos.x, (int)targetPos.y] != 0)//Check crash
				return true;

			return false;
		}


		#region Crash
		private void setPlayerCrash(bool p1, bool p2){
			List<int[]> newCrash = new List<int[]> ();
			PlayerColor winColor;
			string crashMsg = "";

			if (p1 && p2) {
				winColor = PlayerColor.None;
				crashMsg += "B Crash " + getCrashPosInString (0) + "#" + "R Crash " + getCrashPosInString (1);
				newCrash.Add (new int[]{(int)crashPos[0].x, (int)crashPos[0].y});
				newCrash.Add (new int[]{(int)crashPos[1].x, (int)crashPos[1].y});
			} else {
				int crashPlayerIndex = p1 ? 0 : 1;
				crashMsg += (p1 ? "B" : "R") + " Crash " + getCrashPosInString (crashPlayerIndex) + "#";
				winColor = master.getIndexColor (p1 ? 1 : 0);
				newCrash.Add (new int[]{(int)crashPos[crashPlayerIndex].x, (int)crashPos[crashPlayerIndex].y});
			}

			int[][] crashes = new int[newCrash.Count] [];
			for (int i = 0; i < newCrash.Count; i++)
				crashes [i] = newCrash [i];

			updater.setGameOver (winColor, crashes);
			master.submitToGameLog (crashMsg);
			gameRunning = false;
		}


		private string getCrashPosInString(int playerIndex){return (int)crashPos[playerIndex].x + " " + (int)crashPos[playerIndex].y;}
		public void gameOver(){	gameRunning = false;}
		#endregion



		public void activatePlayerMove(GameCommand moveMsg){
			if (moveMsg.myColor != PlayerColor.Blue && moveMsg.myColor != PlayerColor.Red)
				return;

			int oldDir = dir [convertColorToTeam(moveMsg.myColor)];
			if ((moveMsg.dir + 2) % 4 == oldDir)
				return;

			dir [convertColorToTeam(moveMsg.myColor)] = (int)moveMsg.dir;
		}


		public static void moveHandler(object msg, ConnectedClient c){
			GameCommand cMsg;
			try{cMsg = (GameCommand)msg;
			}catch{return;}
				
			singleton.activatePlayerMove (cMsg);
		}



		#region Util
		private bool compPos(Vector2 v1, Vector2 v2){return v1.x == v2.x && v1.y == v2.y;}
		public static int convertColorToTeam(Game.PlayerColor color){	return color == Game.PlayerColor.Blue ? 0 : 1;}
		private void submitPlayerBody(Vector2 pos, int playerIndex){updater.addNewPlayerBody ((int)pos.x + (int)pos.y * gridSize, playerIndex);}

		private void submitToGameLog(){
			string updateMsg = ("B " + (int)playerPos [0].x + " " + (int)playerPos [0].y + " " + dir[0]);
			updateMsg += "#";
			updateMsg +=  ("R " + (int)playerPos [1].x + " " + (int)playerPos [1].y + " " + dir[1]);
			master.submitToGameLog(updateMsg);
		}
		#endregion
	}

}