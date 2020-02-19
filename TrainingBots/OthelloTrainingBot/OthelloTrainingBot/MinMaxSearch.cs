using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OthelloTrainingBot
{
 	class MinMaxSearch
	{
		public static Random rand = new Random();
		public static int round = 0;
		public static int maxDepth;


		public static int[] findBestMove(int[,] board, int depth){
			int[] bestMoveId = new int[2];
			int bestValue = -10000;
			int bestDepth = 0;
			//Console.Clear ();

			List<int[]> PossibleMoves = BoardLogic.getAvailabeMoves (true, board);
			maxDepth = depth;

			if(depth == 0)
				return PossibleMoves [rand.Next (0, PossibleMoves.Count - 1)];


			foreach(int[] i in PossibleMoves){
				Move tempMove = new Move (i, board, true);
				int[] value = alphaBeta (tempMove, maxDepth-1, -10000, 10000, false);

				if (value[0] > bestValue) {
					bestValue = value[0];
					bestMoveId = tempMove.moveID;
					bestDepth = value[1];
				}
				if(value[0] == bestValue && value[1] < bestDepth && value[0] < 0){
					bestValue = value[0];
					bestMoveId = tempMove.moveID;
					bestDepth = value[1];
				}
				if(value[0] == bestValue && value[1] > bestDepth && value[0] > 0){
					bestValue = value[0];
					bestMoveId = tempMove.moveID;
					bestDepth = value[1];
				}
			}
				

			return bestMoveId;
		}
			

		public static int[] alphaBeta(Move move, int depth, int A, int B, bool maxPlayer){
			if (depth == 0 /*|| move.boardValue >= 3000 || move.boardValue <= -3000*/)
				return new int[2]{move.boardValue, maxDepth -depth};
			

			List<int[]> possibleMoves = BoardLogic.getAvailabeMoves(maxPlayer, move.postBoard);
			if(possibleMoves.Count == 0)
				return new int[2]{BoardLogic.getRealScore(move.postBoard), maxDepth-depth};

			if (maxPlayer) {
				int[] value = new int[2]{-6000, maxDepth - depth};

				foreach (int[] i in possibleMoves) {
					Move tempMove = new Move (i, move.postBoard, true);

					int[] tempValue = alphaBeta (tempMove, depth - 1, A, B, false);
					if (tempValue [0] >= value [0])
						value = tempValue;

					A = Math.Max (value[0], A);
					if (B <= A)
						break;
				}


				return value;
			}


			int[] minValue = new int[2]{6000, maxDepth- depth};
			foreach (int[] i in possibleMoves) {
				Move tempMove = new Move (i, move.postBoard, false);

				int[] tempValue = alphaBeta (tempMove, depth - 1, A, B, true);
				if (tempValue [0] <= minValue [0])
					minValue = tempValue;

				B = Math.Min (minValue[0], B);

				if (B <= A)
					break;
			}

			return minValue;
		}


		public static int[,] cloneBoard(int[,] board){
			int[,] tempBoard = new int[6, 7];
			for (int x = 0; x < 7; x++)
				for (int y = 0; y < 6; y++)
					tempBoard [y, x] = board [y, x];

			return tempBoard;
		}


	}




	class Move{
		public int[,] postBoard;
		public int[] moveID;
		public int boardValue;

		public Move(int[] moveID, int[,] board, bool maxMove){
			this.moveID = moveID;
			int[,] cloneBoard = (int[,])board.Clone();
			playMove (cloneBoard, moveID, maxMove);
			boardValue = BoardLogic.valueFunc (board);
		}

		private void playMove(int[,] board, int[] moveID, bool maxMove){
			List<int[]> flips = BoardLogic.findFlipsFromMove (moveID [0], moveID [1], maxMove, board);
			board [moveID [0], moveID [1]] = maxMove ? 1 : -1;
			foreach (int[] i in flips)
				board [i [0], i [1]] = maxMove ? 1 : -1;
			postBoard = board;
		}


	}
		
}

