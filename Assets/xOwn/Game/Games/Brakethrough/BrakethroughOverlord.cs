using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Breakthrough{

	public class BrakethroughOverlord : MonoBehaviour {

		public BrakeThroughRenderer renderer;
		private int[,] gameField;
		private int startCharValue = 65;
		private int xSize, ySize;


		// Use this for initialization
		void Start () {
			initGameField (8, 8);
			renderer.initVizBoard (xSize, ySize);
			makeMove (new int[]{ 0, 1 }, new int[]{ 0, 2 }, 1);
			makeMove (new int[]{ 0, 6 }, new int[]{ 0, 5 }, -1);
		}
		

		private void makeMove(int[] start, int[] target, int player){
			if (validMove (start, target, player) == false)
				return;


			gameField [target [0], target [1]] = gameField [start [0], start [1]];
			gameField [start [0], start [1]] = 0;
			renderer.displayBoard (gameField);
		}


		private bool validMove(int[] start, int[] target, int player){
			int xDist = start [0] - target [0];
			int yDist = start [1] - target [1];

			if (Math.Abs (xDist) > 1 || yDist + player != 0)
				return false;

			if (target [0] >= xSize || target [0] < 0 || target [1] >= ySize || target [1] < 0)
				return false;
			if (start [0] >= xSize || start [0] < 0 || start [1] >= ySize || start [1] < 0)
				return false;


			int startValue = gameField [start [0], start [1]];
			int targetValue = gameField [target [0], target [1]];
			if (startValue != player || targetValue == player)
				return false;

			//Attack
			if (xDist != 0 && (targetValue == player || targetValue == 0))
				return false;
			//Normal
			if (xDist == 0 && targetValue != 0)
				return false;

			return true;
		}



		private void initGameField(int X, int Y){
			gameField = new int[X, Y];
			xSize = X;
			ySize = Y;
			iterateBoard((int x, int y) => {
				if(y < 2)
					gameField[x, y] = 1;
				else if(y > 5)
					gameField[x, y] = -1;
				else
					gameField[x, y] = 0;
			});
		}

		private void iterateBoard(Action<int, int> a){
			for(int y = 0; y < ySize; y++)
				for(int x = 0; x < xSize; x++)
					a(x, y);
		}

		private void printBoard(){
			print ("***********************************");
			for (int y = 0; y < ySize; y++) {
				string s = "";
				for (int x = 0; x < xSize; x++)
					s += gameField [x, y].ToString () + "\t";
				print (s);
			}
		}

		private bool winCheck(){
			for (int x = 0; x < xSize; x++) {
				if (gameField [x, 0] == -1)
					return true;
				
				if (gameField [x, ySize-1] == 1)
					return true;
			}

			return false;
		}


		private bool notationToCoord(string msg, out int[] coord){
			coord = new int[0];
			if (msg.Length < 2)
				return false;

			char x = msg.ToUpper()[0];
			char y = msg [1];

			if (char.IsLetter (x) == false || char.IsDigit(y) == false)
				return false;

			coord = new int[]{ (int)x - startCharValue, int.Parse (y.ToString()) };
			return true;
		}
	}
}