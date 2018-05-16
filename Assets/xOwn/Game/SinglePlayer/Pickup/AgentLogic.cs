using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

	namespace Pickup{
	public class AgentLogic : MonoBehaviour {

		public AgentOverlord overlord;
		public GameObject bgTile, obsticale, dropPoint, item, playerObj;
		public GameObject backgroundParent;
		private List<GameObject> spawnedMap = new List<GameObject>();
		private Player thePlayer;

		public static int boardSize = 8, moveCounter = 0;
		public static float squareSize = 1.05f;
		private GameObject[,] bgTiles;
		private int pickups = 0;
		private List<int[]> blockedSquares = new List<int[]> ();
		private string[,] currentBoard;

		private int[] playerStartPos;
		private bool hasInit = false;
		private string oldPosValue;
		private int[] oldPos;


		public void playMove(int move){
			thePlayer.takeInput (move);
		}


		public string[,] resetGame(int gameSize, string[,] gameBoard){
			if (hasInit)
				destroyOldBoard ();
			hasInit = true;
			moveCounter = 0;
			boardSize = gameSize;

			AgentCamera.initCamera (boardSize);
			bgTiles = new GameObject[boardSize, boardSize];

			currentBoard = new string[gameSize, gameSize];
			generateBackground ();
			generateMap (gameBoard);
			thePlayer.init (finishedMove, wonGame, pickups, playerStartPos, blockedSquares, this);

			return currentBoard;
		}

		private void destroyOldBoard(){
			iterateOverBoard ((int x, int y) => {Destroy(bgTiles[x, y]);});
			for (int i = spawnedMap.Count - 1; i >= 0; i--)
				Destroy (spawnedMap [i]);
		}

		private void generateBackground(){
			float topLeftCorner = ((float)boardSize / 2) * squareSize;

			iterateOverBoard ((int x, int y) => {
				Vector3 spawnPos = new Vector3(-topLeftCorner, topLeftCorner, 0) + new Vector3(x, -y, 0)*squareSize;
				bgTiles[x, y] = Instantiate(bgTile, spawnPos, Quaternion.identity, backgroundParent.transform);
			});
		}

		private void generateMap(string[,] map){
			pickups = 0;
			blockedSquares.Clear ();

			iterateOverBoard ((int x, int y) => {
				Vector3 spawnPos = bgTiles[x, y].transform.position + new Vector3(0, 0, -1);
				string s = map[x, y];
				currentBoard[x, y] = s;

				if(s == "X"){
					spawnedMap.Add(Instantiate(obsticale, spawnPos, Quaternion.identity));
					blockedSquares.Add(new int[]{x, y});
				}
				else if(s == "I"){
					spawnedMap.Add(Instantiate(item, spawnPos, Quaternion.identity));
					pickups++;
				}
				else if(s == "D")
					spawnedMap.Add(Instantiate(dropPoint, spawnPos, Quaternion.identity));
				else if(s == "P"){
					spawnedMap.Add(Instantiate(playerObj, spawnPos, playerObj.transform.rotation));
					thePlayer = spawnedMap[spawnedMap.Count-1].GetComponent<Player>();
					playerStartPos = new int[]{x, y};
				}
			});
		}


		private void finishedMove(){
			moveCounter++;
			overlord.moveFinished (generateCurrentBoard());
		}

		private string[,] generateCurrentBoard(){
			int[] pPos = thePlayer.currentPos;
			string[,] temp = new string[boardSize, boardSize];
			iterateOverBoard ((int x, int y) => {
				if(x == pPos[0] && y == pPos[1])
					temp[x, y] = "P";
				else
					temp[x, y] = currentBoard[x, y];
			});
			return temp;
		}
		
		public string getChar(int[] p){return currentBoard [p [0], p [1]];}
		public void resetPlayerSquare(int[] pos, string value){currentBoard [pos [0], pos [1]] = value;}
		private void wonGame(){overlord.gameOver (moveCounter);}
		private void iterateOverBoard(Action<int, int> func){
			for (int y = 0; y < boardSize; y++)
				for (int x = 0; x < boardSize; x++)
					func (x, y);
		}

	}
}