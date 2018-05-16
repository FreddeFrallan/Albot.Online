using System;
using System.Collections.Generic;

namespace Othello
{
	public class BoardLogic
	{
		//1 == White
		//2 == Black
		private int[,] board = new int[8,8];
		public  BoardLogic(){
			for (int x = 0; x < 8; x++)
				for (int y = 0; y < 8; y++)
					board [x, y] = 0;

			board [3, 3] = 2;
			board [3, 4] = 1;
			board [4, 3] = 1;
			board [4, 4] = 2;
		}


		public List<int[]> playMove(int x, int y, bool white){
			board [x, y] = white ? 1 : 2;
			List<int[]> flips = findFlipsFromMove (x, y, white);
			applyFlips (flips, white);
			return flips;
		}


		private void applyFlips(List<int[]> flips, bool targetWhite){
			foreach (int[] i in flips)
				board [i [0], i [1]] = targetWhite ? 1 : 2;
		}

		public int[] getScore(){
			int white = 0, black = 0;
			for (int x = 0; x < 8; x++)
				for (int y = 0; y < 8; y++) {
					if (board [x, y] == 1)
						white++;
					else if (board [x, y] == 2)
						black++;
				}
			return new int[]{ white, black };
		}

		public List<int[]> getAvailabeMoves(bool white){
			List<int[]> moves = new List<int[]> ();
			for (int x = 0; x < 8; x++)
				for (int y = 0; y < 8; y++) {
					if (board[x, y] == 0 && findFlipsFromMove (x, y, white).Count > 0)
						moves.Add (new int[]{ x, y });
				}
			return moves;
		}

		public override string ToString (){
			string boardFormat = "";
			for (int y = 0; y < 8; y++)
				for (int x = 0; x < 8; x++)
					boardFormat += board [x, y].ToString () + " ";
			return boardFormat;
		}

		//Starting down then going CCW
		private int[,] searchOrder = new int[,]{{0,1}, {1,1}, {1,0}, {1,-1}, {0,-1}, {-1,-1}, {-1,0}, {-1, 1}}; 
		private List<int[]> findFlipsFromMove(int x, int y, bool white){
			List<int[]> allFlips = new List<int[]> ();

			for (int i = 0; i < 8; i++) {
				List<int[]> tempFinds = new List<int[]> ();
				int posX = x + searchOrder [i,0];
				int posY = y + searchOrder [i,1];
				while (posX >= 0 && posX < 8 && posY >= 0 && posY < 8) {
					if (board [posX, posY] == 0)
						break;

					if (board [posX, posY] == (white ? 2 : 1))
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

	}
}

