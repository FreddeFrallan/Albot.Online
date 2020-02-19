using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bomberman{

	public class GameBoard : MonoBehaviour {

		public static GameBoard singleton;
		public GameObject squarePrefab;
		private int mapSize = 9;
		public static readonly int mapMaxOffset = 8; //mapSize / 2   pre calculated because of opt
		private static readonly float halfMapMaxOffset = 4;
		private readonly float squareSize = 1;


		public List<List<MapSquare>> gridMap = new List<List<MapSquare>>();

		// Use this for initialization
		public void Init () {
			singleton = this;
			generateGridMap ();
		}


		private void generateGridMap(){
			float xFirstSpawn = (mapMaxOffset * squareSize) / -2;
		
			for (int y = 0; y < mapSize; y++) {
				gridMap.Add (new List<MapSquare> ());
				float yPos = -xFirstSpawn - y*squareSize;

				for (int x = 0; x < mapSize; x++) {
					float xPos = xFirstSpawn + x * squareSize;

					gridMap[y].Add(Instantiate (squarePrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<MapSquare>());
					gridMap [y] [x].transform.localPosition = transform.position + new Vector3 (xPos, 0, yPos);  //Set position without worring about scaling
					gridMap [y] [x].ID = new int[]{y, x};

					if (y % 2 == 1 && x % 2 == 1)   //Place obsticles
						gridMap [y] [x].setState (SquareState.Obsticale);
					else
						gridMap [y] [x].setState (SquareState.Empty);
				}
			}
		}

		public void updatePlayerPos(int[] newCoord, int[] lastCoord, PlayerController player){
			MapSquare targetSquare = gridMap[newCoord[1]][newCoord[0]];
			MapSquare oldSquare = gridMap [lastCoord [1]] [lastCoord [0]];
		
			if(targetSquare != oldSquare)
				oldSquare.removePlayer();

			if (targetSquare.currentState() == SquareState.Empty) {
				targetSquare.setState (SquareState.Player);
				targetSquare.setPlayer (player);
			}
		}

		public static void placeBomb(int[] pos){singleton.gridMap[pos[1]][pos[0]].setState(SquareState.Bomb);}
		public static bool explodeBomb(int[] pos){return singleton.gridMap [pos [1]] [pos [0]].explode ();}

		//No error checks since it's that we cannot return null.
		//Instead we expect the caller to validate with "validCoord"
		public static Vector3 getGridPos(int x, int y){return singleton.gridMap[y][x].getPos();}

		public static bool validCoord(int x, int y){
			if (x < 0 || y < 0 || y >= singleton.mapSize || x >= singleton.mapSize)
				return false;
			return singleton.gridMap[y][x].getIsWalkable();
		}


		public static int[] getCurrentGridPos(Vector3 pos){
			int x = convertCoordToIndex (pos.x);
			int y = mapMaxOffset - convertCoordToIndex (pos.z);
			return new int[]{x, y};
		}


		private static int convertCoordToIndex(float coord){
			return Mathf.RoundToInt ((coord + halfMapMaxOffset));
		}
	}
}