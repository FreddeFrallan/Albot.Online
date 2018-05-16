using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Connect4{
	public enum Piece{Empty = 0, Yellow = 1, Red = -1}
	
	public class Renderer : MonoBehaviour{

		public GameObject fieldSquare, yellowPiece, redPiece, winningPiece;

		private int numRows = 6;
		private int numColumns = 7;
		private int[,] field;
		private int[] lastDropPos = new int[2];
		private List<int[]> winList = new List<int[]> ();
		private Piece winColor;
		public List<GameObject> dropedPieces = new List<GameObject>();

		// Use this for initialization
		void Start () {
			CreateField ();
		}

		// create an empty field and instantiate the cells
		void CreateField(){
			field = new int[numColumns, numRows];
			for(int x = 0; x < numColumns; x++)
				for(int y = 0; y < numRows; y++){
					field[x, y] = (int)Piece.Empty;
					Instantiate(fieldSquare, new Vector3(x, y * -1, -1), Quaternion.identity);
				}
		}

		public void dropPiece(int move, Piece color){
			if (color == Piece.Empty)
				return;

			GameObject newPiece = (GameObject)Instantiate (color == Piece.Yellow ? yellowPiece : redPiece, 
				new Vector2 (move, 1),
				Quaternion.identity,
				null
			);
			dropedPieces.Add (newPiece);

			int dropTarget = calcDropTargetPos (move);
			if (dropTarget <= 0) {
				field [move, -dropTarget] = color == Piece.Yellow ? 1 : -1;
				newPiece.GetComponent<PlayingPiece> ().dropPiece (dropTarget);
				lastDropPos = new int[]{ move, -dropTarget };
			}
		}

		//If greater then zero colomn is full!
		private int calcDropTargetPos(int move){
			for (int y = numRows - 1; y >= 0; y--)
				if (field [move, y] == 0)
					return -y;

			return 1;
		}

		public void onGameOver(Piece winColor){
			this.winColor = winColor;

			checkWins(getRow(lastDropPos[1]));
			checkWins(getCol(lastDropPos[0]));
			checkWins(getLTDiag(lastDropPos[0], lastDropPos[1]));
			checkWins(getLBDiag(lastDropPos[0], lastDropPos[1]));

			foreach (int[] w in winList)
				Instantiate (winningPiece, new Vector3 (w [0], -w [1], -1), Quaternion.identity);
		}

		private void checkWins(List<int[]> checkList){
			int foundCounter = 0;
			for (int i = 0; i < checkList.Count; i++) {
				foundCounter = field[checkList[i][0], checkList[i][1]] == (int)winColor ? foundCounter + 1 : 0;
				if (foundCounter == 4)
					winList.AddRange(checkList.GetRange(i-3, 4));
				else if (foundCounter > 4)
					winList.Add (checkList [i]);
			}
		}


		private List<int[]> getRow(int y){
			List<int[]> theList = new List<int[]> ();
			for (int i = 0; i < numColumns; i++)
				theList.Add (new int[]{i, y});
			return theList;
		}
		private List<int[]> getCol(int x){
			List<int[]> theList = new List<int[]> ();
			for (int i = 0; i < numRows; i++)
				theList.Add (new int[]{x, i});
			return theList;
		}
		private List<int[]> getLTDiag(int x, int y){
			List<int[]> theList = new List<int[]> ();
			int ltx = Mathf.Clamp(x - y, 0, 6);
			int lty = Mathf.Clamp(y-x, 0, 5);
			while (ltx < numColumns && lty < numRows)
				theList.Add (new int[]{ltx++, lty++});
			return theList;
		}
		private List<int[]> getLBDiag(int x, int y){
			List<int[]> theList = new List<int[]> ();
			int ltx = Mathf.Clamp (-5 + x + y, 0, 6);
			int lty = Mathf.Clamp (y + x, 0, 5);
			while (ltx < numColumns && lty >= 0)
				theList.Add (new int[]{ltx++, lty--});
			return theList;
		}
	}
	
}