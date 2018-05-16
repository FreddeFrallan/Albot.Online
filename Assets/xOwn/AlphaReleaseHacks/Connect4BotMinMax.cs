using System.Collections;
using System.Collections.Generic;
using System;


namespace Connect4Bot{
	public class Connect4BotMinMax{
		public class MinMaxSearch
		{
			public static Random rand;
			public static int round = 0;
			public static int maxDepth;

			public static int findBestMove(int[,] board, int depth){
				int startValue = Connect4BoardEvaluation.evaluateStartPosition (board);
				int bestMoveId = getPossibleMoves(board)[0];
				int bestValue = -10000;
				int bestDepth = 0;
				maxDepth = depth;


				List<int[]> values = new List<int[]> ();
				foreach(int i in getPossibleMoves(board)){
					Move tempMove = new Move (i, board, true, startValue);
					int[] value = alphaBeta (tempMove, maxDepth-1, -10000, 10000, false);
					values.Add(new int[]{value[0], tempMove.moveID, value[1]});

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
				if (depth == 0 || move.boardValue >= 3000 || move.boardValue <= -3000)
					return new int[2]{move.boardValue, maxDepth -depth};


				List<int> possibleMoves = getPossibleMoves(move.postBoard);
				if (maxPlayer) {
					int[] value = new int[2]{-6000, maxDepth - depth};

					foreach (int i in possibleMoves) {
						Move tempMove = new Move (i, move.postBoard, true, move.boardValue);

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
				foreach (int i in possibleMoves) {
					Move tempMove = new Move (i, move.postBoard, false, move.boardValue);

					int[] tempValue = alphaBeta (tempMove, depth - 1, A, B, true);
					if (tempValue [0] <= minValue [0])
						minValue = tempValue;

					B = Math.Min (minValue[0], B);

					if (B <= A)
						break;
				}

				return minValue;
			}
				
			public static List<int> getPossibleMoves(int[,] board){
				List<int> possibleMoves = new List<int> ();
				for (int i = 0; i < 7; i++)
					if (board [0, i] == 0)
						possibleMoves.Add (i);

				return possibleMoves;
			}

			public static int[,] cloneBoard(int[,] board){
				int[,] tempBoard = new int[6, 7];
				for (int x = 0; x < 7; x++)
					for (int y = 0; y < 6; y++)
						tempBoard [y, x] = board [y, x];

				return tempBoard;
			}


		}




		public	class Move{
			public int[,] postBoard;
			public int moveID;
			int moveYID;
			public int boardValue;

			public Move(int moveID, int[,] board, bool maxMove, int startValue){
				this.moveID = moveID;
				playMove (board, moveID, maxMove);
				int tempScore = Connect4BoardEvaluation.evaluateMoveBoard (postBoard, moveID, moveYID, maxMove);
				boardValue = maxMove ? startValue + tempScore : startValue - tempScore;
			}

			private void playMove(int[,] board, int moveID, bool maxMove){
				postBoard = MinMaxSearch.cloneBoard (board);
				for (int y = 5; y >= 0; y--)
					if (postBoard [y, moveID] == 0) {
						moveYID = y;
						postBoard [y, moveID] = maxMove ? 1 : -1;
						return;
					}
			}
		}
	}
}