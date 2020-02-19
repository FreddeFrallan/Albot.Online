using System;
using System.Collections.Generic;
using System.Linq;

namespace BrakethroughBot {
	public class Board {
		public static readonly int xSize = 8;
		public static readonly int ySize = 8;

		public bool gameOver;
		public List<int[]> white, black;
		private int score = 0;
		private Square[,] grid;


		#region MoveStates
		public const byte allMoves = 0x07;
		public const byte noMoves = 0x00;

		public const byte onlyRight = 0x01;
		public const byte onlyMid = 0x02;
		public const byte onlyLeft = 0x04;

		public const byte midAndLeft = 0x06;
		public const byte midAndRight = 0x03;
		public const byte leftAndRight = 0x05;
		#endregion


		public Board (int[,] startBoard) {
			grid = new Square[xSize, ySize];
			white = new List<int[]>();
			black = new List<int[]>();

			//Create empty board
			Util.iterateOverBoard((x, y) => {
				grid[x, y].move = new byte[2] {allMoves, allMoves};
				if (x == 0) {
					grid[x, y].move[0] = midAndRight;
					grid[x, y].move[1] = midAndRight;
				}
				if (x == xSize - 1) {
					grid[x, y].move[0] = midAndLeft;
					grid[x, y].move[1] = midAndLeft;
				}
			});

			//Add players
			Util.iterateOverBoard((x, y) => {
				grid[x, y].playerSign = startBoard[x, y];
				if (startBoard[x, y] == 0) 
					return;

				int team = startBoard[x, y] == 1 ? 0 : 1;
				((team == 0) ? white : black).Add(new int[] { x, y }); //Add coord to player list
				int dir = startBoard[x, y] * -1;
				int opponentTeam = (team + 1) % 2;

				//Remove forward oponent
				grid[x, y + dir].move[opponentTeam] &= leftAndRight;

				int backRow = y - dir;
				if (backRow < 0 || backRow >= ySize)
					return;

				//Remove Backrow moves
				foreach (int backX in getXList(x)) {
					if (backX == -1)
						grid[x + backX, backRow].move[team] &= midAndLeft;
					else if (backX == 0)
						grid[x + backX, backRow].move[team] &= leftAndRight;
					else
						grid[x + backX, backRow].move[team] &= midAndRight;
				}
			});
			score = Search.evaluateBoard(this);
		}

		public Board (Square[,] g, bool win, List<int[]> white, List<int[]> black, int score) {
			grid = g;
			gameOver = win;
			this.white = white;
			this.black = black;
			this.score = score;
		}



		public Board makeMove (int[] from, int[] target, int team) {
			//If this is a win, we don't have to compute anything
			int tempScore = score;
			bool win = target[1] == 0 || target[1] == ySize - 1;
			if (win)
				return returnFromMakeMove(null, true, null, null, tempScore);

			Square[,] tempGrid = copyGrid();
			bool white = team == 0;
			bool attack = tempGrid[target[0], target[1]].playerSign != 0;
			List<int[]> teamPieces = white ? this.white.ToList() : black.ToList();
			List<int[]> opponentPieces = white ? black.ToList() : this.white.ToList();

			//Update in position lists
			int index = teamPieces.FindIndex((obj) => from[0] == obj[0] && from[1] == obj[1]);
			teamPieces[index] = target;

			if (attack)//Remove opponent piece if take
				opponentPieces.RemoveAt(opponentPieces.FindIndex((obj) => obj[0] == target[0] && obj[1] == target[1]));

			//Update squares
			tempGrid[target[0], target[1]].playerSign = white ? 1 : -1; //New Square
			tempGrid[from[0], from[1]].playerSign = 0;					//Old Square
			editAvailableMoves(ref tempGrid, from, target, team);


			if (white)
				return new Board(tempGrid, false, teamPieces, opponentPieces, tempScore);
			else
				return new Board(tempGrid, false, opponentPieces, teamPieces, tempScore);
		}

		private Board returnFromMakeMove (Square[,] tempGrid, bool win, List<int[]> white, List<int[]> black, int tempScore) {
			return new Board(tempGrid, win, white, black, tempScore);
		}


		private void editAvailableMoves (ref Square[,] tempGrid, int[] oldPos, int[] newPos, int team) {
			int dir = (team == 0) ? -1 : 1;
			int opponentTeam = (team + 1) % 2;
			int[] xNewList = getXList(newPos[0]);
			int[] xOldList = getXList(oldPos[0]);

			//Add moves to team from empty oldPos
			if (oldPos[1] > 0 && oldPos[1] < ySize-1) {
				int formerBackRow = oldPos[1] - dir;

				//Iterate oldBack pos and activate their moves
				foreach (int x in xOldList) {
					if (x == -1)
						grid[oldPos[0] + x, formerBackRow].move[team] |= onlyRight;
					else if (x == 0)
						grid[oldPos[0] + x, formerBackRow].move[team] |= onlyMid;
					else
						grid[oldPos[0] + x, formerBackRow].move[team] |= onlyLeft;
				//	tempGrid[oldPos[0] + x, formerBackRow].move[team][1 - x] = true;
				}
			}
			//Add old opponent forward
			tempGrid[oldPos[0], newPos[1]].move[opponentTeam] |= onlyMid;

			foreach (int x in xNewList) {
				if (x == -1)
					grid[newPos[0] + x, oldPos[1]].move[team] &= midAndLeft;
				else if (x == 0)
					grid[newPos[0] + x, oldPos[1]].move[team] &= leftAndRight;
				else
					grid[newPos[0] + x, oldPos[1]].move[team] &= midAndRight;
			}
			/*
			//Remove backrow from team
			foreach (int x in xNewList)
				tempGrid[newPos[0]+x, oldPos[1]].move[team][1 - x] = false;
			*/
			//Remove forwardOponent
			tempGrid[newPos[0], newPos[1] + dir].move[opponentTeam] &= leftAndRight;
		}

		private int[] getXList (int xPos) {
			if (xPos == 0)
				return new int[] { 0, 1 };
			if (xPos == xSize-1)
				return new int[] { -1, 0 };

			return new int[] { -1, 0, 1 };
		}



		public List<int[]> getAvailableMoves (int team) {
			bool white = team == 0;
			int dir = white ? -1 : 1;
			List<int[]> teamPieces = white ? this.white : black;

			List<int[]> moves = new List<int[]>();
			foreach (int[] piece in teamPieces) {
				Square s = grid[piece[0], piece[1]];

				switch (s.move[team]) {
					case noMoves: break;
					case allMoves: 
						for (int i = 0; i < 3; i++)
							moves.Add(new int[] { piece[0], piece[1], piece[0] + (i - 1), piece[1] + dir });
					break;

					case leftAndRight: 
						moves.Add(new int[] { piece[0], piece[1], piece[0] - 1, piece[1] + dir });
						moves.Add(new int[] { piece[0], piece[1], piece[0] + 1, piece[1] + dir });
					break;

					case midAndRight:
						moves.Add(new int[] { piece[0], piece[1], piece[0], piece[1] + dir });
						moves.Add(new int[] { piece[0], piece[1], piece[0] + 1, piece[1] + dir });
					break;

					case midAndLeft:
						moves.Add(new int[] { piece[0], piece[1], piece[0] - 1, piece[1] + dir });
						moves.Add(new int[] { piece[0], piece[1], piece[0], piece[1] + dir });
					break;

					case onlyLeft:
						moves.Add(new int[] { piece[0], piece[1], piece[0] - 1, piece[1] + dir });
					break;

					case onlyRight:
						moves.Add(new int[] { piece[0], piece[1], piece[0] + 1, piece[1] + dir });
					break;

					case onlyMid:
						moves.Add(new int[] { piece[0], piece[1], piece[0], piece[1] + dir });
					break;
				}

				/*
				for (int i = 0; i < 3; i++)
					if (s.move[team][i])
						moves.Add(new int[] { piece[0], piece[1], piece[0] + (i - 1), piece[1] + dir });
				*/
			}

			return moves;
		}



		private Square[,] copyGrid () {
			Square[,] temp = new Square[xSize, ySize];
			//Square[,] temp = (Square[,])grid.Clone();

			Util.iterateOverBoard((x, y) => {
				temp[x, y].playerSign = grid[x, y].playerSign;
				Console.WriteLine(temp[x, y].move);
				temp[x, y].move = new byte[2] {temp[x, y].move[0], temp[x, y].move[1]};
			});
			return temp;
		}

		public void printBoard () {Util.printBoard(grid);}
		public int getScore () { return score;}
	}

	//User -> team = 0
	//Opponent -> team = 1
	//A -> All
	//B -> All but left
	//C -> All but Mid
	//D -> All all but right
	//E -> A
	public struct Square {
		public byte[] move;
		public int playerSign;
	}


}
