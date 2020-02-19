using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using ClientUI;

namespace Othello{
	public class OthelloBoardRenderer : MonoBehaviour {
		public GameObject piecePrefab;
		public BoardSquare[,] field = new BoardSquare[8, 8];
		public Transform fieldStartSquare;
		private float squareSize = 35.5f;
		public List<GameObject> playedPieces = new List<GameObject> ();
		private OthelloBoardLogic boardLogic = new OthelloBoardLogic();
		public ClientUserPanelUI[] panels;

		// Use this for initialization
		void Start () {
			initField ();
		}


		public void resetGame(){
			foreach (GameObject g in playedPieces)
				Destroy (g, 0.5f);
			playedPieces.Clear ();

			initField ();
		}

		public void playMove(int x, int y, bool white){
			createNewPiece (field [x, y], white);
			List<int[]> flips = boardLogic.playMove (x, y, white);
			flipSquares (flips, white);
			int[] currentScore = boardLogic.getScore ();
			panels [0].setScore (currentScore [0]);
			panels [1].setScore (currentScore [1]);
		}

		public void flipSquares(List<int[]> squares, bool targetWhite){
			foreach (int[] s in squares) {
				try{
					if(field[s[0], s[1]].isEmpty == false)
						field[s[0], s[1]].piece.flipToNewColor(targetWhite);
				}
				catch{}
			}
		}

		private void initField(){
			for (int x = 0; x < 8; x++)
				for (int y = 0; y < 8; y++)
					field [x, y] = new BoardSquare (fieldStartSquare.transform.position + new Vector3(x*squareSize, 0, -y*squareSize));

			createNewPiece (field [3, 3], false);
			createNewPiece (field [4, 3], true);
			createNewPiece (field [3, 4], true);
			createNewPiece (field [4, 4], false);
		}
			
		private void createNewPiece(BoardSquare square, bool white){
			square.piece = Instantiate (piecePrefab, square.position, Quaternion.identity).GetComponent<OthelloPiece>();
			square.piece.setColor (white);
		}
	}


	public class BoardSquare{
		public OthelloPiece piece;
		public bool isEmpty{ get { return piece == null; } }
		public bool isWhite{get{ return piece.isWhite;} }
		public Vector3 position;

		public BoardSquare(Vector3 position){this.position = position;}
	}
}