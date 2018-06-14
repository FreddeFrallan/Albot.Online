using System;
using System.Collections.Generic;

namespace SnakeBot {
	public class DecisionMaker {

		private static Random rand = new Random();

		public static string decideNextMove (Board b) {
			int[] moveDir = extractDir(b.getPlayerJObj());
			int[] pos = extractPos(b.getPlayerJObj());
			int[] nextPos = new int[] { pos[0] + moveDir[0], pos[1] + moveDir[1]};

            b.printBoard();
            Console.WriteLine("Next: " + nextPos[0] + "." + nextPos[1]);
            if (willCollide(nextPos, b))
                return getNonCollision(moveDir, pos, b);
            
            return " ";
		}


		private static bool willCollide (int[] nextPos, Board b) {
			if (nextPos[0] < 0 || nextPos[0] >= Constants.BOARD_SIZE || nextPos[1] < 0 || nextPos[1] >= Constants.BOARD_SIZE)
				return true;

			int[,] grid = b.getGrid();
			return grid[nextPos[0], nextPos[1]] == Constants.BLOCKED_BOARD_VALUE;
		}


		private static string getNonCollision (int[] moveDir, int[] pos, Board b) {
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
					if (moveAxis == 0 && dir > 0) return "Right";
					if (moveAxis == 0 && dir < 0) return "Left";
					if (moveAxis == 1 && dir > 0) return "Down";
					else return "Up";
				}
			}

			return " ";
		}




		private static int[] extractPos (JSONObject p) {
			int x = (int)p.GetField(Constants.Fields.posX).n;
			int y = (int)p.GetField(Constants.Fields.posY).n;
			return new int[] { x, y };
		}

		private static int[] extractDir (JSONObject p) {
			switch (p.GetField(Constants.Fields.direction).str) {
				case "Right": return new int[] { 1, 0};
				case "Up": return new int[] { 0, -1 };
				case "Left": return new int[] { -1, 0 };
				case "Down": return new int[] { 0, 1 };
			}
			return new int[] { 0, 0 };
		}
	}
}
