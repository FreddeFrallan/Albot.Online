using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrakethroughBot {
	public class Search {
		public static Dictionary<int, int> rowScores = new Dictionary<int, int>();
		private static int winScore = int.MaxValue / 20;
		private static int depth = 4;
		private static Random r = new Random();

		public static void init (int d) {
			rowScores.Clear();
			depth = d;
			for (int i = 0; i < Board.ySize; i++)
				rowScores.Add(i, (int)Math.Pow(3, i));
		}


		public static void minMaxSearch (Board startBoard) {
			List<int[]> moves = startBoard.getAvailableMoves(0);
			int[] results = new int[moves.Count];

			/*
			Parallel.For(0, moves.Count, i => {
				int[] m = moves[i];
				Board simBoard = simulateMove(startBoard, new int[] { m[0], m[1] }, new int[] { m[2], m[3] }, 0);
				results[i] = alphaBetaSearch(simBoard, 1, depth, int.MinValue, int.MaxValue);
				//results[i] = (tempSearch(simBoard, 1, depth));
			});
			*/

			for (int i = 0; i < moves.Count; i++){
				int[] m = moves[i];
				Board simBoard = simulateMove(startBoard, new int[] { m[0], m[1] }, new int[] { m[2], m[3] }, 0);
				results[i] = (tempSearch(simBoard, 1, depth));
			}


			int bestIndex = getRandomBestMove(results.ToList());
			int[] bestMove = moves[bestIndex];
			MoveHandler.setMove(bestMove);
		}


		private static int tempSearch (Board b, int team, int depth) {
			if (b.gameOver) 
				return returnGameOverBoard(opponentTeam(team)) * (depth+1);
			if (depth == 0) {
				return evaluateBoard(b);
				return b.getScore();
			}
			

			List<int> results = new List<int>();
			foreach (int[] m in b.getAvailableMoves(team)) {
				Board simBoard = simulateMove(b, new int[]{m[0], m[1] }, new int[] { m[2], m[3] }, team);
				results.Add(tempSearch(simBoard, opponentTeam(team), depth - 1));
			}

			if(team == 0)
				return getItem(results,(int x, int y) => x > y);
			else
				return getItem(results, (int x, int y) => x < y);
		}


		private static int alphaBetaSearch (Board b, int team, int depth, int alpha, int beta) {
			int score = 0;

			if (b.gameOver)
				return returnGameOverBoard(opponentTeam(team)) * (depth + 1);
			if (depth == 0) {
				score = evaluateBoard(b);
				return score;
			}
			//return b.getScore();

			List<int[]> moves = b.getAvailableMoves(team);
			Action<int> update;

			if (team == 0) {
				score = int.MinValue;
				update = (int tempRes) => { score = Math.Max(tempRes, score); alpha = Math.Max(tempRes, alpha); };
			}
			else {
				score = int.MaxValue;
				update = (int tempRes) => { score = Math.Min(tempRes, score); beta = Math.Min(tempRes, beta); };
			}


			foreach (int[] m in moves) {
				Board simBoard = simulateMove(b, new int[] { m[0], m[1] }, new int[] { m[2], m[3] }, team);
				int tempRes = alphaBetaSearch(simBoard, opponentTeam(team), depth - 1, alpha, beta);

				update(tempRes);
				if (beta <= alpha)
					break;
			}

			return score;
		}





		#region util
		private static Board simulateMove (Board b, int[] from, int[] target, int team) {return b.makeMove(from, target, team);}
		private static int opponentTeam (int team) { return (team + 1) % 2;}

		public static int evaluateBoard (Board b) {
			int score = 0;

			foreach (int[] p in b.white) {
				int row = Board.ySize - (p[1] + 1);
				score += rowScores[row];
			}
			foreach (int[] p in b.black)
				score -= rowScores[p[1]];
			
			return score;
		}

		private static int returnGameOverBoard (int team) {
			if (team == 0)
				return winScore;
			return -winScore;
		}

		private static int getItem (List<int> l, Func<int, int, bool> comp) {
			int t = l[0];
			for (int i = 1; i < l.Count; i++)
				if (comp(l[i], t))
					t = l[i];
			return t;
		}

		private static int getIndex (List<int> l, Func<int, int, bool> comp) {
			int t = 0;
			for (int i = 1; i < l.Count; i++)
				if (comp(l[i], t))
					t = i;
			return t;
		}



		private static int getRandomBestMove (List<int> moves) {
			List<int> temp = moves.OrderByDescending(x => x).ToList();

			List<int> indicies = new List<int>();
			for (int i = 0; i < moves.Count; i++)
				if (moves[i] == temp[0])
					indicies.Add(i);

			return indicies[r.Next(0, indicies.Count - 1)];
		}
		#endregion
	}





}
