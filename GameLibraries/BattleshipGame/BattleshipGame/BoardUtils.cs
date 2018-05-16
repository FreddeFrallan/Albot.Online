using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleshipGame
{
	public class BoardUtils
	{
		public static int[] numberToCoord(int target){return new int[2]{target % 10, (int)target / 10 };}

		public static void printCharBoard(char[,] charboard){
		//	Main.GameMaster.singleton.debugPrint ("Printing board ***********");
			for (int y = 0; y < 3; y++) {
				string temp = "";
				for (int x = 0; x < 10; x++)
					temp += charboard [x, y] + " ";
		//		Main.GameMaster.singleton.debugPrint (temp);
			}
		}

		public static char[,] charBoardFromShipList(List<ShipPlacement> ships){
			char[,] charBoard = generateEmptyBoard ();
			foreach (ShipPlacement s in ships) {
			//	Main.GameMaster.singleton.debugPrint (s.type.ToString());
				foreach (int[] pos in s.coords) {
				//	Main.GameMaster.singleton.debugPrint (" " + pos[0] + "." + pos[1]);
					charBoard [pos [0], pos [1]] =  GameConstants.ships.Find(x => x.type == s.type).sign;
				}
			}
		//	printCharBoard (charBoard);
			return charBoard;
		}

		public static bool charBoardFromString(string input, out char[,] charBoard){
			charBoard = new char[10, 10];
			input = input.Trim ();
		//	Main.GameMaster.singleton.debugPrint ("String is: " + input.Length);
			if (input.Length < 100)
				return false;

			for (int x = 0; x < 10; x++)
				for (int y = 0; y < 10; y++)
					charBoard [x, y] = input [y * 10 + x];

			return true;
		}

		public static List<string> generateRows(char[,] charBoard){
			List<string> rows = new List<string> ();
			for (int y = 0; y < 10; y++) {
				string temp = "";
				for (int x = 0; x < 10; x++)
					temp += charBoard [y, x].ToString ();
				rows.Add (temp);
			}

			return rows;
		}
		public static List<string> generateCols(char[,] charBoard){
			List<string> cols = new List<string> ();
			for (int x = 0; x < 10; x++) {
				string temp = "";
				for (int y = 0; y < 10; y++)
					temp += charBoard [y, x].ToString ();
				cols.Add (temp);
			}

			return cols;
		}


		public static bool containsRightAmountOfShips(char[,] charBoard, ref List<ShipPlacement> ships){
			List<string> rows = BoardUtils.generateRows (charBoard);
			List<string> cols = BoardUtils.generateCols (charBoard);
			int[] shipCounter = new int[5]{ 0, 0, 0, 0, 0 };
			List<ShipPlacement> newShips = new List<ShipPlacement> ();

			countShipsInStringList (rows, ref shipCounter, true, newShips);
			countShipsInStringList (cols, ref shipCounter, false, newShips);

			string count = "";
			foreach (int i in shipCounter)
				count += "  " + i;
		//	Main.GameMaster.singleton.debugPrint (count);

			foreach (int i in shipCounter)
				if (i != 1)
					return false;

			ships = newShips;
			return true;
		}


		private static void countShipsInStringList(List<string> input, ref int[] shipCounter, bool horizontal, List<ShipPlacement> newShips){
			for(int i = 0; i < input.Count; i++)
				foreach (string word in splitStringIntoMatchingWords(input[i]))
					foreach (Ship sh in GameConstants.ships)
						if (word == sh.boardString) {
							shipCounter [GameConstants.ships.IndexOf (sh)]++;

							int startIndex = input [i].IndexOf (sh.boardString); //We can do this because if the board is correct it will be only one instance of every boat. If not we will catch this later in the shipcount
							newShips.Add(decideShipPlacement(horizontal, i, startIndex, sh.size, sh.type));
						}
		}

		private static ShipPlacement decideShipPlacement(bool horizontal, int lineIndex, int startIndex, int size, Battleship.ShipType type){
			int[] startPos = new int[2];

			if (horizontal) {
				startPos [0] = startIndex;
				startPos[1] = lineIndex;
			}
			else{
				startPos [0] = lineIndex;
				startPos [1] = startIndex;
			}

			int xDir = horizontal ? 1 : 0;
			int yDir = horizontal ? 0 : 1;
			List<int[]> tempCoords = new List<int[]> ();

			for (int i = 0; i < size; i++) {
				tempCoords.Add (new int[]{ startPos [1], startPos [0] }); // Wtf invert?!

				startPos [0] += xDir;
				startPos [1] += yDir;
			}

			return new ShipPlacement (type, size, tempCoords, horizontal);
		}



		//Instead of regex we do a linear split. Might be overkill but should enhance performance
		private static List<string> splitStringIntoMatchingWords(string s){
			List<string> words = new List<string> ();
			string lastFound = s [0].ToString();

			for (int i = 1; i < s.Length; i++)
				if (s [i] == lastFound [0])
					lastFound += s [i];
				else {
					words.Add (lastFound);
					lastFound = s[i].ToString();
				}

			words.Add (lastFound);

			return words;
		}


		public static char[,] generateEmptyBoard(){
			char[,] charBoard = new char[10, 10];
			for (int x = 0; x < 10; x++)
				for (int y = 0; y < 10; y++)
					charBoard [x, y] = GameConstants.UNEXPLOREDCHAR;

			return charBoard;
		}
	}
}

