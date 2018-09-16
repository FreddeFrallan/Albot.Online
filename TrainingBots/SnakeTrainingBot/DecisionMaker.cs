using System;
using System.Collections.Generic;
using System.Linq;

using static SnakeBot.Constants;
using static SnakeBot.SnakeStructs;

namespace SnakeBot {
	public class DecisionMaker {

		private static Random rand = new Random();

		public static string decideNextMove (Board board) {
			int[] moveDir = dirToArr(board.getPlayerDirection());
            Position pos = board.getPlayerPosition();// extractPos(board.getPlayerJObj());
			Position nextPos = new Position() { x = pos.x + moveDir[0], y = pos.y + moveDir[1]};

            List<MoveScore> moves = CalcMoveScores(board);

            int maxScore = moves.Max(x => x.score);
            List<MoveScore> bestMoves = moves.Where(x => x.score == maxScore).ToList();

            if (maxScore == 2) { // Move away from player in situations where few possible moves
                int maxDist = bestMoves.Max(x => squareDistanceFromEnemy(board.getPlayerPosition(), board.getEnemyPosition(), x.dir));
                bestMoves = bestMoves.Where(x => 
                    squareDistanceFromEnemy(board.getPlayerPosition(), board.getEnemyPosition(), x.dir) == maxDist
                ).ToList();
            }

            return bestMoves[rand.Next(bestMoves.Count)].dir;
        }

        static List<MoveScore> CalcMoveScores(Board board) {
            string dir = board.getPlayerDirection();
            List<string> possMoves = APIFunctions.getPossibleMoves(dir);
            List<MoveScore> moves = new List<MoveScore>(3);
            foreach (string direction in possMoves) {
                moves.Add(new MoveScore() { dir = direction, score = DegreesOfFreedom(board, direction) });
            }
            return moves;
        }

        static int DegreesOfFreedom(Board board, string direction) {
            int x = board.getPlayerPosition().x;
            int y = board.getPlayerPosition().y;
            int newX, newY;
            switch (direction) {
                case Fields.right:
                    newX = x + 1;
                    if (board.cellBlocked(newX, y))
                        return 0;
                    return 1 + cellFree(board, newX + 1, y) + cellFree(board, newX, y + 1) + cellFree(board, newX, y - 1);
                case Fields.left:
                    newX = x - 1;
                    if (board.cellBlocked(newX, y))
                        return 0;
                    return 1 + cellFree(board, newX - 1, y) + cellFree(board, newX, y + 1) + cellFree(board, newX, y - 1);
                case Fields.down:
                    newY = y + 1;
                    if (board.cellBlocked(x, newY))
                        return 0;
                    return 1 + cellFree(board, x, newY + 1) + cellFree(board, x + 1, newY) + cellFree(board, x - 1, newY);
                default: // up
                    newY = y - 1;
                    if (board.cellBlocked(x, newY))
                        return 0;
                    return 1 + cellFree(board, x, newY - 1) + cellFree(board, x + 1, newY) + cellFree(board, x - 1, newY);
            }
        }

        private static int cellFree(Board board, int x, int y) {
            return Convert.ToInt32(!board.cellBlocked(x, y));
        }

        static int squareDistance(int x1, int y1, int x2, int y2) {
            int deltaX = x2 - x1;
            int deltaY = y2 - y1;
            return deltaX * deltaX + deltaY * deltaY;
        }

        static int squareDistanceFromEnemy(Position player, Position enemy, string dir) {
            int x = player.x;
            int y = player.y;
            switch (dir) {
                case Fields.right: x++; break;
                case Fields.left: x--; break;
                case Fields.down: y++; break;
                default: y--; break; // up
            }
            return squareDistance(x, y, enemy.x, enemy.y);
        }

        private static int[] dirToArr (string dir) {
			switch (dir) {
				case Fields.right: return new int[] { 1, 0};
				case Fields.up: return new int[] { 0, -1 };
				case Fields.left: return new int[] { -1, 0 };
				case Fields.down: return new int[] { 0, 1 };
			}
			return new int[] { 0, 0 };
		}

    }
}

