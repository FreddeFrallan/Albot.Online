using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ClientUI;

namespace Breakthrough{

	public class BrakeThroughRenderer : MonoBehaviour {

		public Transform leftDownCorner;
		public GameObject emptySquare, whiteSquare, darkSquare;
		public Sprite blackPawn, whitePawn;
		public ClientUserPanelUI p1, p2;

		private List<GameObject> spawnedTiles = new List<GameObject>();
		private PawnSquare[,] pawnSquares; 
		private float squareSize = 0.99f;
		private int xSize, ySize;

		void Start(){
			initVizBoard (8, 8);
			p1.initTurnTime (10);
			p2.initTurnTime (10);
		}

		public void initVizBoard(int xSize, int ySize){
			this.xSize = xSize;
			this.ySize = ySize;
			pawnSquares = new PawnSquare[xSize, ySize];

			iterateBoard((int x, int y) =>{
					GameObject square = (y * xSize + x + y) % 2 == 0 ? whiteSquare : darkSquare;
					Vector3 spawnPos = leftDownCorner.transform.position + new Vector3 (x+0.5f, y, 0) * squareSize;
					spawnedTiles.Add( Instantiate (square, spawnPos + new Vector3(0, 0, 0.1f), square.transform.rotation, leftDownCorner));

					pawnSquares[x,y] = Instantiate (emptySquare, spawnPos, emptySquare.transform.rotation).GetComponent<PawnSquare>();
			});
		}

		public void setPlayerScore(Game.PlayerColor player, int score){
			ClientUserPanelUI panel = player == Game.PlayerColor.White ? p1 : p2;
			panel.setScore (score);
		}

		public void startTimer(bool white){
			if (white) {
				p1.startTimer (10);
				p2.stopTimer ();
			} else {
				p1.stopTimer ();
				p2.startTimer (10);
			}
		}


		public void displayBoard(int[,] board){
			int whitePawns = 0, blackPawns = 0;

			iterateBoard ((int x, int y) => {
				if(board[x, y] == 0)
					pawnSquares[x, y].updateSprite(null);
				else if(board[x, y] == 1){
					pawnSquares[x, y].updateSprite(whitePawn);
					whitePawns++;
				}
				else{
					pawnSquares[x, y].updateSprite(blackPawn);
					blackPawns++;
				}
			});

			int maxPawns = board.GetLength (0) * 2;
			p1.setScore (maxPawns - blackPawns);
			p2.setScore (maxPawns - whitePawns);
		}

		private void iterateBoard(Action<int, int> a){
			for(int y = 0; y < ySize; y++)
				for(int x = 0; x < xSize; x++)
					a(x, y);
		}
	}
}