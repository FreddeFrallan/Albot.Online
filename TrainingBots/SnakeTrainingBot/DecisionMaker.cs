using System;
using System.Collections.Generic;

namespace SnakeBot {
	public class DecisionMaker {

		private static int boardSize = 20;
		private static Random rand = new Random();

		public static string decideNextMove (CurrentBoard b, JSONObject player) {
			int[] moveDir = extractDir(player);
			int[] pos = extractPos(player);
			int[] nextPos = new int[] { pos[0] + moveDir[0], pos[1] + moveDir[1]};


			if (willCollide(nextPos, b)) {
				return getNonCollision(moveDir, pos, b);
			}
			else
				return " ";
		}



		private static bool willCollide (int[] nextPos, CurrentBoard b) {
			if (nextPos[0] < 0 || nextPos[0] >= boardSize || nextPos[1] < 0 || nextPos[1] >= boardSize)
				return true;

			int[,] grid = b.getGrid();
			return grid[nextPos[0], nextPos[1]] != 0;
		}



		private static string getNonCollision (int[] moveDir, int[] pos, CurrentBoard b) {
			int moveAxis = moveDir[0] == 0 ? 0 : 1;
			int[] newMoveDir = new int[] { 0, 0 };


			List<int> directions = new List<int>();
			if (rand.Next(0, 100) > 50)
				directions.AddRange(new int[] { -1, 1 });
			else
				directions.AddRange(new int[] { 1, -1});


			foreach (int dir in directions) {
				newMoveDir[moveAxis] = dir;
				int[] nextPos = new int[] { pos[0] + newMoveDir[0], pos[1] + newMoveDir[1] };
				if (willCollide(nextPos, b) == false) {
					if (moveAxis == 0 && dir > 0) return "0";
					if (moveAxis == 0 && dir < 0) return "2";
					if (moveAxis == 1 && dir > 0) return "3";
					else return "1";
				}
			}

			return " ";
		}




		private static int[] extractPos (JSONObject p) {
			int x = (int)p.GetField("posX").n;
			int y = (int)p.GetField("posY").n;
			return new int[] { x, y };
		}

		private static int[] extractDir (JSONObject p) {
			switch (p.GetField("direction").str) {
				case "Right": return new int[] { 1, 0};
				case "Up": return new int[] { 0, -1 };
				case "Left": return new int[] { -1, 0 };
				case "Down": return new int[] { 0, 1 };
			}
			return new int[] { 0, 0 };
		}
	}
}
