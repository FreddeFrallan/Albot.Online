using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battleship{

	public class PlayerBoard : MonoBehaviour {
		const int START_HP = 17;

		public GameObject square;
		private Square[,] grid = new Square[10,10];
		private float squareHeight = 0.2f;
		private List<Ship> ships;

		#region init
		void Start () {spawnGrid ();}

		private void spawnGrid(){
			for (int x = 0; x < 10; x++)
				for (int z = 0; z < 10; z++) {
					grid[x, z] = Instantiate (square, transform.position + new Vector3 (x, squareHeight, z), Quaternion.identity).GetComponent<Square>();
					grid [x, z].transform.parent = transform;
					grid [x, z].gameObject.isStatic = true;
				}
		}

		public void placeShips(List<Ship> ships){
			foreach (Ship s in ships) {
				foreach (int[] pos in s.coords) {
					s.assignSquare (grid [pos [0], pos [1]]);
				}
			
				s.transform.position = s.squares [0].transform.position;
				s.transform.localEulerAngles = new Vector3 (0, 360 - s.rot * 90, 0);
			}
			this.ships = ships;
		}
		#endregion

		
		public void fireAtGrid(int i, bool isLocalPlayerTarget, char targetStatus, ShipType targetType, int[] startPos, bool horizontal){
			if (isLocalPlayerTarget) //If this client is the target we can simply run rendering.
				grid [(int)(i / 10), i % 10].fireAtSquare ();
			else { //If we are firing at the enemy we must use info from server about how to render
				if(targetStatus == GameOverlord.singleton.sunkSign)
					grid [(int)(i / 10), i % 10].fireAtOponentSquare(targetStatus, targetType,  grid[startPos[1], startPos[0]].transform.position, horizontal);
				else
					grid [(int)(i / 10), i % 10].fireAtOponentSquare(targetStatus, targetType,  Vector3.zero , horizontal);
			}
		}

		public void removeShipAtCoord(int[] coord){
			Ship targetShip = ships.Find (x => x.coords.Find (y => y [0] == coord [0] && y [1] == coord [1]) != null); //Finds a ship with matchin coordinate
			if (targetShip == null)
				return;

			ships.Remove (targetShip);
			Destroy (targetShip.gameObject);
		}
		

		#region char boards
		public char[,] generateEnigmaCharBoard(){
			char[,] charB = new char[10, 10];
			for (int x = 0; x < 10; x++)
				for (int z = 0; z < 10; z++) {
					Square s = grid [x, z];

					if (s.status == SquareStatus.hit)
						charB [x, z] = GameOverlord.singleton.hitSign;
					else if (s.status == SquareStatus.miss)
						charB [x, z] = GameOverlord.singleton.missSign;
					else if (s.status == SquareStatus.sunk)
						charB [x, z] = GameOverlord.singleton.sunkSign;
					else
						charB [x, z] = GameOverlord.singleton.undiscoveredSign;
				}

			return charB;
		}
		#endregion


		public int calculateHP(){
			int foundHits = 0;
			for (int x = 0; x < 10; x++)
				for (int z = 0; z < 10; z++) 
					foundHits += grid[x, z].status == SquareStatus.hit || grid[x, z].status == SquareStatus.sunk ? 1 : 0;

			return START_HP - foundHits;
		}
	}

}