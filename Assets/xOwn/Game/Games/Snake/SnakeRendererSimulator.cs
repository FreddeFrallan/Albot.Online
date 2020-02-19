using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace Snake{
	
	public class SnakeRendererSimulator : MonoBehaviour {

		public SnakeRenderer theRenderer;
		private List<BoardUpdate> moves = new List<BoardUpdate> ();
        private BoardUpdate finalBoard;
		private GameInfo finalMsg;

		// Use this for initialization
		private void Start () {
			generateGame ();
			StartCoroutine (sendGame ());
		}
			
		private IEnumerator sendGame(){
			while (moves.Count > 1) {
				yield return new WaitForSeconds (SnakeGameLogic.refreshRate);
				theRenderer.handleBoardUpdate (moves [0]);
				moves.RemoveAt (0);
			}
			yield return new WaitForSeconds (SnakeGameLogic.refreshRate);
            theRenderer.handleAdminGameOver(moves[0]);
            //theRenderer.displayCrash (new Vector2 (finalMsg.crashPos [0] [0], finalMsg.crashPos [0] [1]));
		}

		private void generateGame(){
			for (int i = 0; i < 8; i++) {
				BoardUpdate b = new BoardUpdate ();
				b.redDir = 0;
				b.blueDir = 2;
				b.updateNumber = (uint)i;
                b.blueCoords = newPos(2 + i, 5);//new int[]{ 2 + i};
                b.redCoords = newPos(8 - i, 7);//new int[]{ 38 - i};
                moves.Add (b);
			}

            finalBoard = new BoardUpdate();
            finalBoard.redDir = 0;
            finalBoard.blueDir = 2;
            finalBoard.updateNumber = (uint)8;
            finalBoard.blueCoords = newPos(10, 5);
            finalBoard.redCoords = newPos(0, 7);
            finalBoard.gameOver = true;
            finalBoard.winnerColor = PlayerColor.Blue;
            moves.Add(finalBoard);

            finalMsg = new GameInfo ("Some Username", Game.PlayerColor.None, 
				new int[][]{new int[]{20, 0}},
				true
			);
			
		}

        private Position2D[] newPos(int x, int y) {
            return new Position2D[] { new Position2D() { x = x, y = y } };
        }
	}

}