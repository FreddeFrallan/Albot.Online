﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using System;

namespace Snake{

	public class SnakeGameLogic : MonoBehaviour {
		public static readonly float refreshRate = 3f;
        public const int GRID_SIZE = 10;

        private System.Random rand = new System.Random();
		private static SnakeGameLogic singleton;
		public SnakeGameStateUpdater updater;
		public SnakeGameMaster master;

		private int[,] gameGrid;
		private bool gameRunning = true;

		private Position2D[] playerPos = new Position2D[2];
		private Vector2[] crashPos = new Vector2[2];
		private int[] dir = new int[]{ 0, 2 };
        private int[] oldDir = new int[] { 0, 2 };


		#region init
		public void startGame (SnakeGameMaster master) {
			this.master = master;
			singleton = this;
			initGameGrid ();
			updater.sendBoardUpdate (gameGrid, playerPos, dir);

            submitToGameLog();
            StartCoroutine (gameLoop ());
		}

		private void initGameGrid(){
			gameGrid = new int[GRID_SIZE, GRID_SIZE];
			for (int x = 0; x < GRID_SIZE; x++)
				for (int y = 0; y < GRID_SIZE; y++)
					gameGrid [x, y] = 0;
		
			generateRandomStartPos ();
            setOldDirs();
        }

		private void generateRandomStartPos(){
			int x = rand.Next (2, GRID_SIZE / 2-3);
			int y = rand.Next (2, GRID_SIZE / 2-3);

			playerPos [0] = new Position2D() {x = x, y = y};
			gameGrid [(int)playerPos[0].x, (int)playerPos[0].y] = 1;
			submitPlayerBody (playerPos[0], 0);

            playerPos[1] = new Position2D() { x = GRID_SIZE - x - 1, y = GRID_SIZE - y - 1 };
            gameGrid [(int)playerPos[1].x, (int)playerPos[1].y] = 2;
			submitPlayerBody (playerPos[1], 1);

			if (rand.Next (0, 100) >= 50)
				dir = new int[]{ 0, 2 };
			else
				dir = new int[]{ 3, 1};
		}

		#endregion
			

		private IEnumerator gameLoop(){
			while (gameRunning) {
				yield return new WaitForSeconds (refreshRate);
				gameTick ();
                submitToGameLog();
                if (gameRunning)
					updater.sendBoardUpdate (gameGrid, playerPos, dir);
			}
		}


		private void gameTick(){
			Vector2 tPos1 = Vector2.zero, tPos2 = Vector2.zero;
			bool crash1 = movePlayer (0, ref tPos1);
			bool crash2 = movePlayer (1, ref tPos2);
            setOldDirs();

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
            playerPos[0] = new Position2D() { x = (int)tPos1.x, y = (int)tPos1.y };
            playerPos[1] = new Position2D() { x = (int)tPos2.x, y = (int)tPos2.y };

            submitPlayerBody (playerPos[0], 0);
			submitPlayerBody (playerPos[1], 1);
		}

		private bool movePlayer(int playerIndex, ref Vector2 targetPos){
			Position2D currentPos = playerPos [playerIndex];
			targetPos = new Vector2 (currentPos.x, currentPos.y);

			switch (dir [playerIndex]) {
				case 0: targetPos.x++; break;
				case 1: targetPos.y--; break;
				case 2: targetPos.x--; break;
				case 3: targetPos.y++; break;
			}
			crashPos[playerIndex] = new Vector2(targetPos.x, targetPos.y);

			if (targetPos.x < 0 || targetPos.x >= GRID_SIZE || targetPos.y < 0 || targetPos.y >= GRID_SIZE) //Out of map
				return true;
			if (gameGrid [(int)targetPos.x, (int)targetPos.y] != 0)//Check crash
				return true;

			return false;
		}
        private void setOldDirs() {
            oldDir[0] = dir[0];
            oldDir[1] = dir[1];
        }


		#region Crash
		private void setPlayerCrash(bool p1, bool p2){
			List<int[]> newCrash = new List<int[]> ();
			PlayerColor winColor;
            GameOverState winState;

			if (p1 && p2) {
                winState = GameOverState.draw;
				winColor = PlayerColor.None;
				newCrash.Add (new int[]{(int)crashPos[0].x, (int)crashPos[0].y});
				newCrash.Add (new int[]{(int)crashPos[1].x, (int)crashPos[1].y});

            } else {
                winState = GameOverState.playerWon;
                int crashPlayerIndex = p1 ? 0 : 1;
				winColor = master.getIndexColor (p1 ? 1 : 0);
				newCrash.Add (new int[]{(int)crashPos[crashPlayerIndex].x, (int)crashPos[crashPlayerIndex].y});
			}

			int[][] crashes = new int[newCrash.Count] [];
			for (int i = 0; i < newCrash.Count; i++)
				crashes [i] = newCrash [i];

			master.submitToGameLog (SnakeGameLogProtocol.generateCrash(p1, p2, crashPos));
            master.onGameOver(winState, winColor, crashes);
		}

		public void gameOver(){	gameRunning = false;}
		#endregion



		public void activatePlayerMove(GameCommand moveMsg){
			if (moveMsg.myColor != PlayerColor.Blue && moveMsg.myColor != PlayerColor.Red)
				return;

			int postDir = oldDir [convertColorToTeam(moveMsg.myColor)];
			if ((moveMsg.dir + 2) % 4 == postDir)
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
		public static int convertColorToTeam(PlayerColor color){	return color == PlayerColor.Blue ? 0 : 1;}
		private void submitPlayerBody(Position2D pos, int playerIndex){updater.addNewPlayerBody (pos, playerIndex);}
		private void submitToGameLog(){master.submitToGameLog(SnakeGameLogProtocol.generateState(playerPos, dir));}
		#endregion
	}

}