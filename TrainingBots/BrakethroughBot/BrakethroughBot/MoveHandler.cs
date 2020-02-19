using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace BrakethroughBot {
	public class MoveHandler {

		private static int[][] currentMove = new int[2][];
		public static string getMoveString () {
			return Util.coordToStr(currentMove[0]) + Util.coordToStr(currentMove[1]);
		}

		public static void setMove (int[] move) {
			setMove(new int[] { move[0], move[1] }, new int[] { move[2], move[3] });
		}
		public static void setMove (int[] start, int[] target) {
			currentMove[0] = (int[])start.Clone();;
			currentMove[1] = (int[])target.Clone();
		}


		private static Random r = new Random();


		public static void playMove (int[,] startBoard) {
			Search.minMaxSearch(new Board(startBoard));
		}

	}

}
