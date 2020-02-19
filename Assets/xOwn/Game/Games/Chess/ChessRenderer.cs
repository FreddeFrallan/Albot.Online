using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


namespace Chess{
	public class ChessRenderer : MonoBehaviour {
		public Material whiteMat, blackMat;
		private Vector3 whiteRot = new Vector3 (90, 0, 0), blackRot = new Vector3 (90, 0, 0);


		public GameObject pawn, rook, bishop, knight, king, queen;
		public BoardSquare[,] field = new BoardSquare[8, 8];
		public Transform fieldStartSquare;
		private float squareSize = 29f;
		public Text scoreWhite, scoreBlack;

		// Use this for initialization
		void Start () {
			initField ();
		}

		public void makeMove(int[] start, int[] target){
			field [target [0], target [1]].moveToSquare (field [start [0], start [1]].piece);
			field [start [0], start [1]].moveFromeSquare ();
		}
			

		private void initField(){
			for (int x = 0; x < 8; x++)
				for (int y = 0; y < 8; y++) {
					field [x, y] = new BoardSquare (fieldStartSquare.transform.position + new Vector3(x*squareSize, 0, y*squareSize));
				}

			//Pawns
			for (int x = 0; x < 8; x++) {
				field [x, 1].piece = Instantiate (pawn, field [x, 1].position, Quaternion.Euler (whiteRot)).GetComponent<ChessPiece> ();
				field [x, 1].piece.setMaterial (whiteMat);
				field [x, 6].piece = Instantiate (pawn, field [x, 6].position, Quaternion.Euler (blackRot)).GetComponent<ChessPiece> ();
				field [x, 6].piece.setMaterial (blackMat);
			}

			//Rooks
			field [0, 0].piece = Instantiate (rook, field [0, 0].position, Quaternion.identity).GetComponent<ChessPiece> ().init(whiteMat);
			field [7, 0].piece = Instantiate (rook, field [7, 0].position, Quaternion.identity).GetComponent<ChessPiece> ().init(whiteMat);
			field [0, 7].piece = Instantiate (rook, field [0, 7].position, Quaternion.identity).GetComponent<ChessPiece> ().init(blackMat);
			field [7, 7].piece = Instantiate (rook, field [7, 7].position, Quaternion.identity).GetComponent<ChessPiece> ().init(blackMat);

			//Knights
			field [1, 0].piece = Instantiate (knight, field [1, 0].position, Quaternion.identity).GetComponent<ChessPiece> ().init(whiteMat);
			field [6, 0].piece = Instantiate (knight, field [6, 0].position, Quaternion.identity).GetComponent<ChessPiece> ().init(whiteMat);
			field [1, 7].piece = Instantiate (knight, field [1, 7].position, Quaternion.identity).GetComponent<ChessPiece> ().init(blackMat);
			field [6, 7].piece = Instantiate (knight, field [6, 7].position, Quaternion.identity).GetComponent<ChessPiece> ().init(blackMat);
				
			//Bishops
			field [2, 0].piece = Instantiate (bishop, field [2, 0].position, Quaternion.Euler (whiteRot)).GetComponent<ChessPiece> ().init(whiteMat);
			field [5, 0].piece = Instantiate (bishop, field [5, 0].position, Quaternion.Euler (whiteRot)).GetComponent<ChessPiece> ().init(whiteMat);
			field [2, 7].piece = Instantiate (bishop, field [2, 7].position, Quaternion.Euler (blackRot)).GetComponent<ChessPiece> ().init(blackMat);
			field [5, 7].piece = Instantiate (bishop, field [5, 7].position, Quaternion.Euler (blackRot)).GetComponent<ChessPiece> ().init(blackMat);
		
			//Queens
			field [3, 0].piece = Instantiate (queen, field [3, 0].position, Quaternion.identity).GetComponent<ChessPiece> ().init(whiteMat);
			field [3, 7].piece = Instantiate (queen, field [3, 7].position, Quaternion.identity).GetComponent<ChessPiece> ().init(blackMat);

			//Kings
			field [4, 0].piece = Instantiate (king, field [4, 0].position, Quaternion.identity).GetComponent<ChessPiece> ().init(whiteMat);
			field [4, 7].piece = Instantiate (king, field [4, 7].position, Quaternion.identity).GetComponent<ChessPiece> ().init(blackMat);
		}

	}


	public class BoardSquare{
		public ChessPiece piece;
		public bool isEmpty{ get { return piece == null; } }
		public bool isWhite{get{ return piece.playerColor == Color.white;} }
		public Vector3 position;

		public BoardSquare(Vector3 position){this.position = position;}
		public void moveFromeSquare(){piece = null;}
		public void moveToSquare(ChessPiece newPiece){
			if (piece != null)
				piece.removeChessPiece ();
			piece = newPiece;
			newPiece.move (position);
		}
	}



}