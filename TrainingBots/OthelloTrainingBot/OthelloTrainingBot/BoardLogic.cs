using System;
using System.Collections.Generic;

namespace OthelloTrainingBot
{
	class BoardLogic
	{
		public static Func<int[,], int> valueFunc;

		public static void initUseWeights(bool value){
			if(value)
				valueFunc = getScoreWeights;
			else
				valueFunc = getRealScore;
		}

		public static List<int[]> getAvailabeMoves(bool white, int[,] board){
			List<int[]> moves = new List<int[]> ();
			for (int x = 0; x < 8; x++)
				for (int y = 0; y < 8; y++) {
					if (board[x, y] == 0 && findFlipsFromMove (x, y, white, board).Count > 0)
						moves.Add (new int[]{ x, y });
				}
			return moves;
		}
		
		//Starting down then going CCW
		private static int[,] searchOrder = new int[,]{{0,1}, {1,1}, {1,0}, {1,-1}, {0,-1}, {-1,-1}, {-1,0}, {-1, 1}}; 
		public static List<int[]> findFlipsFromMove(int x, int y, bool white, int[,] board){
			List<int[]> allFlips = new List<int[]> ();
			
			for (int i = 0; i < 8; i++) {
				List<int[]> tempFinds = new List<int[]> ();
				int posX = x + searchOrder [i,0];
				int posY = y + searchOrder [i,1];
				while (posX >= 0 && posX < 8 && posY >= 0 && posY < 8) {
					if (board [posX, posY] == 0)
						break;
					
					if (board [posX, posY] == (white ? -1 : 1))
						tempFinds.Add (new int[]{ posX, posY });
					else {
						allFlips.AddRange (tempFinds);
						break;
					}
					
					posX += searchOrder [i, 0];
					posY += searchOrder [i, 1];
				}
			}
			
			return allFlips;
		}
		

		private const int a = 50, b = 2, c = 8, d = 12, e = 1, f = 5, g = 8;
		private static int[,] boardWeights = new int[,]{
			{a, b, c, d, d, c, b, a},
			{b, e, f, f, f, f, e, b},
			{c, f, g, g, g, g, f, c},
			{d, f, g, f, f, g, f, d},
			{d, f, g, f, f, g, f, d},
			{c, f, g, g, g, g, f, c},
			{b, e, c, d, d, c, e, b},
			{a, b, c, d, d, c, b, a}
		};

		private static int getScoreWeights(int[,] board){
			int white = 0, black = 0;
			for (int x = 0; x < 8; x++)
				for (int y = 0; y < 8; y++)
					if (board [x, y] == 1)
						white += boardWeights[x,y];
					else if (board [x, y] == -1)
						black += boardWeights[x,y];
			
			return white - black;
		}
			
		public static int getRealScore(int[,] board){
			int white = 0, black = 0;
			for (int x = 0; x < 8; x++)
				for (int y = 0; y < 8; y++) {
					if (board [x, y] == 1)
						white +=  1;
					else if (board [x, y] == -1)
						black +=  1;
				}
			return white - black;
		}


		public static void printBoard(int[,] board){
			Console.WriteLine ("");
			for (int y = 0; y < 8; y++) {
				for (int x = 0; x < 8; x++)
					Console.Write(board [x, y].ToString () + "\t");

				Console.WriteLine ("");
			}
		}
	}
}

