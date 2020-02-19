using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


	namespace Pickup{
	public class BoardGenerator {

		private static int oLeft, iLeft;
		private static List<int[]> posLeft = new List<int[]>();
		private static int boardSize;
		private static string[,] newBoard;
		private static List<int[]> placedItems;
		private static int[] playerPos;



		public static string[,] generateBoard(int size){
			boardSize = size;
			setupVars ();
			calcVars ();
			createBackground ();
			placePlayerAndDrop ();
			placeItems ();
			placeWalls ();

			return newBoard;
		}


		private static void placeWalls(){
			while (oLeft > 0 && posLeft.Count > 0) {
				int[] p = getRandomPos ();
				posLeft.Remove (p);

				string[,] tempGrid = (string[,])newBoard.Clone ();
				tempGrid [p [0], p [1]] = "X";
				if (AvailableSearch.validMap (tempGrid, placedItems) == false)
					continue;
				
				newBoard [p [0], p [1]] = "X";
				oLeft--;
			}
		}


		private static void placeItems(){
			while (iLeft > 0) {
				int[] p = getRandomPos ();
				newBoard [p [0], p [1]] = "I";
				placedItems.Add (p);
				posLeft.Remove (p);
				iLeft--;
			}
		}

		private static void setupVars(){
			placedItems = new List<int[]> ();
			posLeft.Clear ();
			newBoard = new string[boardSize, boardSize];
			iterateOverBoard ((int x, int y) => posLeft.Add (new int[]{ x, y }));
		}


		private static void calcVars(){
			int maxSquares = boardSize * boardSize - 2;
			int availableSquares = maxSquares / 3;
			oLeft = (availableSquares / 2) + 1;
			iLeft = boardSize - 2;
		}

		private static void createBackground(){iterateOverBoard ((int x, int y) => newBoard [x, y] = "-");}

		private static void placePlayerAndDrop(){
			playerPos = getRandomPos ();
			newBoard [playerPos [0], playerPos [1]] = "P";
			posLeft.Remove (playerPos);
			placedItems.Add (playerPos);

			int[] dPos = getRandomPos ();
			newBoard [dPos [0], dPos [1]] = "D";
			posLeft.Remove (dPos);
			placedItems.Add (dPos);
		}

		#region util
		private static int[] getRandomPos(){return posLeft [UnityEngine.Random.Range (0, posLeft.Count)];}
		private static void iterateOverBoard(Action<int, int> func){
			for (int y = 0; y < boardSize; y++)
				for (int x = 0; x < boardSize; x++)
					func (x, y);
		}
		#endregion
	}
}