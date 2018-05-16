using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardLogic{

	public static GameLogic2048 overlord;

	public static void spawnRandom(int[,] gameBoard, int roundCounter){
		List<int[]> emptySquares = new List<int[]> ();
		for (int y = 0; y < 4; y++)
			for (int x = 0; x < 4; x++)
				if (gameBoard [x, y] == 0)
					emptySquares.Add (new int[]{ x, y });

		int[] target = emptySquares [Random.Range (0, emptySquares.Count)];
		if (roundCounter >= 5)
			gameBoard [target [0], target [1]] = Random.value < 0.9 ? 2 : 4;
		else
			gameBoard [target [0], target [1]] = 2;

		overlord.spawnNewTile (target [0], target [1], gameBoard [target [0], target [1]]);
	}


	public static bool moveInXDirection(int xDir, int[,] gameBoard){
		bool playedMove = false;
		int startX = xDir > 0 ? 3 : 0;
		for (int x = startX; x >= 0 && x < 4; x -= xDir)
			for (int y = 0; y < 4; y++)
				for (int i = 1; i < 5; i++) {
					if (gameBoard [x, y] == 0)
						break;

					int newX = x + i * xDir;
					if (newX > 3 || newX < 0) {
						int temp = gameBoard [x, y];
						gameBoard [x, y] = 0;
						gameBoard [newX-xDir, y] = temp;
						if(i != 1)
							playedMove = true;
						break;
					}
					if (gameBoard [newX, y] != 0) {
						if (gameBoard [newX, y] == gameBoard [x, y]) {
							gameBoard [newX, y] *= 2;
							gameBoard [x, y] = 0;
							playedMove = true;
						}
						else {
							int temp = gameBoard [x, y];
							gameBoard [x, y] = 0;
							gameBoard [newX-xDir, y] = temp;
							if(i != 1)
								playedMove = true;
						}
						break;	
					}
				}
		return playedMove;
	}

	public static bool moveInYDirection(int yDir, int[,] gameBoard){
		bool playedMove = false;
		int startY = yDir > 0 ? 3 : 0;
		for (int y = startY; y >= 0 && y < 4; y -= yDir)
			for (int x = 0; x < 4; x++)
				for (int i = 1; i < 5; i++) {
					if (gameBoard [x, y] == 0)
						break;

					int newY = y + i * yDir;
					if (newY > 3 || newY < 0) {
						int temp = gameBoard [x, y];
						gameBoard [x, y] = 0;
						gameBoard [x, newY-yDir] = temp;
						if(i != 1)
							playedMove = true;
						break;
					}
					if (gameBoard [x, newY] != 0) {
						if (gameBoard [x, newY] == gameBoard [x, y]) {
							gameBoard [x, newY] *= 2;
							gameBoard [x, y] = 0;
							playedMove = true;
						} else {
							int temp = gameBoard [x, y];
							gameBoard [x, y] = 0;
							gameBoard [x, newY-yDir] = temp;
							if(i != 1)
								playedMove = true;
						}

						break;	
					}
				}
		return playedMove;
	}


	public static bool checkIfGameOver(int[,] gameBoard){
		bool canMove = false;

		for (int y = 0; y < 4; y++)
			for (int x = 0; x < 4; x++)
				if (gameBoard [x, y] == 0)
					canMove = true;

		if (canMove)
			return true;

		for (int y = 0; y < 4; y++)
			for (int x = 0; x < 4; x++) {
				if (x - 1 >= 0 && gameBoard [x - 1, y] == gameBoard [x, y])
					canMove = true;
				if (x + 1 < 4 && gameBoard [x + 1, y] == gameBoard [x, y]) 
					canMove = true;
				if (y - 1 >= 0 && gameBoard [x, y - 1] == gameBoard [x, y])
					canMove = true;
				if (y + 1 < 4 && gameBoard [x, y + 1] == gameBoard [x, y])
					canMove = true;

			}
		return canMove;
	}
}
