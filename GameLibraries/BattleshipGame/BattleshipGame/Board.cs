using System;
using System.Collections.Generic;

namespace BattleshipGame
{
	public class Board
	{
		public char[,] charBoard;
		public List<ShipPlacement> ships = new List<ShipPlacement>();



		public string formatBoard(){
			string temp = "";
			for (int x = 0; x < 10; x++)
				for (int y = 0; y < 10; y++)
					temp += charBoard [x, y];
			return temp;
		}

		public char fireAtBoard(int target, ref ShipPlacement potentialShip){
			BoardUtils.printCharBoard (charBoard);
			int[] pos = BoardUtils.numberToCoord (target);
		//	Main.GameMaster.singleton.debugPrint ("Fire at: " + pos[0] + "." + pos[1] + "   ****************");
			char hitSign = charBoard [pos [0], pos [1]];

			if (char.IsNumber (hitSign) && hitSign != GameConstants.EMPTYCHAR) // We got a new hit
				hitSign = handleHit (pos, ref potentialShip);
			else if (hitSign == GameConstants.EMPTYCHAR)
				charBoard [pos [0], pos [1]] = GameConstants.MISSCHAR;
			else { //We hit a target we have already hit before
				hitSign = GameConstants.MISSCHAR;
			}

		//	Main.GameMaster.singleton.debugPrint ("Target sign: " + hitSign);
			return hitSign;
		}



		private char handleHit(int[] pos, ref ShipPlacement potentialShip){
			ShipPlacement targetShip = ships.Find (x => x.coords.Find (y => y [0] == pos [0] && y [1] == pos [1]) != null); // Finds the ship with matching cordinates

			targetShip.hitCoord (pos);
			targetShip.hp--;
		//	Main.GameMaster.singleton.debugPrint ("Is a fresh hit, hp left: " + targetShip.hp);

			charBoard[pos[0], pos[1]] = GameConstants.HITCHAR;
			potentialShip = targetShip;

			if (targetShip.hp == 0) {
				sinkShip (targetShip);
				return GameConstants.SUNKCHAR;
			}
			return GameConstants.HITCHAR;
		}

		private void sinkShip(ShipPlacement s){
			foreach(int[] c in s.hitCoords)
				charBoard [c [0], c [1]] = GameConstants.SUNKCHAR;
		}

	
		public char[,] generateEnigmaBoard(){
			char[,] tempBoard = new char[10, 10];
			for (int x = 0; x < 10; x++)
				for (int y = 0; y < 10; y++) {
					if (charBoard [x, y] == GameConstants.EMPTYCHAR || char.IsNumber (charBoard [x, y]))
						tempBoard [x, y] = GameConstants.UNEXPLOREDCHAR;
					else
						tempBoard [x, y] = charBoard [x, y];
				}
			
			return tempBoard;
		}


		public void createEmptyTargetBoard(){charBoard = BoardUtils.generateEmptyBoard ();}
	}


	public class ShipPlacement{
		public Battleship.ShipType type;
		public List<int[]> coords = new List<int[]>();
		public List<int[]> hitCoords = new List<int[]>();
		public bool horizontal;
		public int[] startPos;
		public int hp;
		public ShipPlacement(Battleship.ShipType type, int size, List<int[]> coords, bool horizontal){
			this.type = type;
			this.hp = size;
			this.coords = coords;
			this.horizontal = horizontal;

			if(coords.Count > 0)
				startPos = (int[])coords [0].Clone();
		}
		public void hitCoord(int[] pos){
			int[] temp = coords.Find (x => x [0] == pos [0] && x [1] == pos [1]);
			hitCoords.Add (temp);
			coords.Remove (temp);
		}
	}
}

