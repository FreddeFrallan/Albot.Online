using System;
using System.Collections.Generic;

namespace SnakeBot {
	public class CurrentBoard {

		private int boardSize = 20;
		private int[,] grid;

		public CurrentBoard () {
			grid = new int[boardSize, boardSize];
			iterateGrid((x, y) => grid[x, y] = 0);
		}


		private void iterateGrid (Action<int, int> f) {
			for (int y = 0; y < boardSize; y++)
				for (int x = 0; x < boardSize; x++)
					f(x, y);
		}


		public int[,] getGrid () {
			return grid;
		}


		public void handleBlockUpdate (List<JSONObject> blocks) {
			foreach (JSONObject b in blocks)
				grid[(int)b.GetField("posX").n, (int)b.GetField("posY").n] = -1;
		}

		public void insertPlayerPos (JSONObject p, int id) {
			grid[(int)p.GetField("posX").n, (int)p.GetField("posY").n] = id;
		}
	}
}
