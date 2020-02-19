using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connect4Bot{
	public class Connect4BoardEvaluation : MonoBehaviour {

		public static int evaluateStartPosition(int[,] board){
			int lastFound = 0;
			int currentScore = 0;

			//Count Cols
			for (int x = 0; x < 7; x++) {
				for (int y = 0; y < 6; y++) {
					if (board [y, x] == lastFound && lastFound != 0)
						currentScore += lastFound;
					else
						lastFound = board [y, x];
				}
			}

			lastFound = 0;
			//Count Rows
			for (int y = 0; y < 6; y++) {
				for (int x = 0; x < 7; x++) {
					if (board [y, x] == lastFound && lastFound != 0)
						currentScore += lastFound;
					else
						lastFound = board [y, x];
				}
			}

			//Left upper Diagonall
			for (int i = 0; i < 3; i++) {
				lastFound = 0;
				int x = 0;
				for (int y = 3 + i; y >= 0; y--, x++) {
					if (board [y, x] == lastFound && lastFound != 0)
						currentScore += lastFound;
					else
						lastFound = board [y, x];
				}
			}

			//Left lower Diagonall
			for (int i = 1; i < 4; i++) {
				lastFound = 0;
				int y = 5;
				for (int x = i; x < 7; y--, x++) {
					if (board [y, x] == lastFound && lastFound != 0)
						currentScore += lastFound;
					else
						lastFound = board [y, x];
				}
			}

			//Right upper Diagonall
			for (int i = 0; i < 3; i++) {
				lastFound = 0;
				int x = 6;
				for (int y = 3 + i; y >= 0; y--, x--) {
					if (board [y, x] == lastFound && lastFound != 0)
						currentScore += lastFound;
					else
						lastFound = board [y, x];
				}
			}


			//Right lower Diagonall
			for (int i = 5; i >= 3; i--) {
				lastFound = 0;
				int y = 5;
				for (int x = i; x >= 0; y--, x--) {
					if (board [y, x] == lastFound && lastFound != 0)
						currentScore += lastFound;
					else
						lastFound = board [y, x];
				}
			}

			return currentScore;
		}



		public static int evaluateMoveBoard(int[,] board, int moveX, int moveY, bool maxMove){
			int tempScore = 0;

			int left = searchDir (board, moveX, moveY, maxMove, -1, 0);
			int right = searchDir (board, moveX, moveY, maxMove, 1, 0);
			int top = searchDir (board, moveX, moveY, maxMove, 0, -1);
			int bot = searchDir (board, moveX, moveY, maxMove, 0, 1);

			int leftTop = searchDir (board, moveX, moveY, maxMove, -1, -1);
			int rightTop = searchDir (board, moveX, moveY, maxMove, 1, -1);
			int leftBot = searchDir (board, moveX, moveY, maxMove, -1, 1);
			int rightBot = searchDir (board, moveX, moveY, maxMove, 1, 1);

			if (left + right >= 3)
				return 6666;
			if (top + bot >= 3)
				return 6666;
			if (leftTop + rightBot >= 3)
				return 6666;
			if (leftBot + rightTop >= 3)
				return 6666;

			//Vert
			if(top + bot > 0)
				tempScore += countExtraScore(top, bot);
			//Hor
			if(left + right > 0)
				tempScore += countExtraScore(left, right);
			//Diag1
			if(leftTop + rightBot > 0)
				tempScore += countExtraScore(leftTop, rightBot);
			//Diag2
			if(leftBot + rightTop > 0)
				tempScore += countExtraScore(leftBot, rightTop);


			return tempScore;
		}

		private static int countExtraScore(int sideOne, int sideTwo){
			if (sideOne == 2 || sideTwo == 2)
				return 1;

			return sideOne + sideTwo;
		}

		public static int searchDir(int[,] board,int moveX, int moveY, bool maxMove, int xDir, int yDir){
			int foundCounter = 0;
			int searchValue = maxMove ? 1 : -1;
			moveX += xDir;
			moveY += yDir;

			while (moveX >= 0 && moveY >= 0 && moveX < 7 && moveY < 6) {
				if (board [moveY, moveX] == searchValue)
					foundCounter++;
				else
					break;

				moveX += xDir;
				moveY += yDir;
			}

			return foundCounter;
		}
	}

}