using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bomberman{

	public class PlayerController : BombermanMapObject {

		private readonly float moveMinDist = 0.1f;
		public GameObject bombPrefab;

		public int[] currentCoord, lastCoord;
		public Game.PlayerColor color;
		public Vector3 targetPos;
		MapSquare targetSquare, lastTargetSquare;
		private bool moving = false, dead = false;
		private float moveSpeed = 3;
		private int HP = 1, bombsLeft = 2;

		void Start(){
			targetPos = transform.position;
		}

		// Update is called once per frame
		void Update () {
			if (moving == false)
				return;
			movePlayer ();
		}

		private void movePlayer(){
			if (Vector3.Distance (transform.position, targetPos) > moveMinDist)
				transform.position = Vector3.MoveTowards (transform.position, targetPos, moveSpeed*Time.deltaTime);
			else {
				moving = false;
				transform.position = targetPos;
			}
		}


		public void initMovePlayer(MoveDirections dir){
			int[] newCoord = new int[2];
			if (getNewCoords (dir, ref newCoord) == false)
				return;
		
			targetSquare = GameBoard.singleton.gridMap [newCoord [1]] [newCoord [0]];
			if (targetSquare.currentState() != SquareState.Empty || targetSquare.plannedToBeMovedTo)
				return;

			targetSquare.planToMoveToSquare ();
			if (lastTargetSquare != null && targetSquare != lastTargetSquare)
				lastTargetSquare.plannedToBeMovedTo = false;
			lastTargetSquare = targetSquare;

			moving = true;
			targetPos = GameBoard.getGridPos (newCoord [0], newCoord [1]);
		}


		private bool getNewCoords(MoveDirections dir, ref int[] newCoord){
			newCoord = new int[]{ currentCoord [0], currentCoord [1] };

			if (dir == MoveDirections.Right) newCoord [0]++;
			else if (dir == MoveDirections.Left) newCoord [0]--;
			else if (dir == MoveDirections.Up) newCoord [1]--;
			else newCoord [1]++;

			return GameBoard.validCoord (newCoord [0], newCoord [1]);
		}
			
		public void takeDamage(){ 
			if (dead)
				return;

			if (--HP <= 0) {
				BombermanOverlord.playerDied (this);
				GameBoard.singleton.gridMap [currentCoord[1]][currentCoord[0]].removePlayer ();
				dead = true;
				gameObject.SetActive (false);
				GameBoard.singleton.gridMap [currentCoord [1] ][currentCoord [0]].removePlayer();
			}
		}

		public void placeBomb(){
			if (GameBoard.validCoord (currentCoord [0], currentCoord [1]) == false)
				return;
			if (bombsLeft == 0)
				return;

			Bomb temp = Instantiate (bombPrefab, GameBoard.getGridPos (currentCoord [0], currentCoord [1]), Quaternion.identity, transform.parent).GetComponent<Bomb> ();
			temp.init (3, 3, currentCoord, this);

			BoardAction dropAction = new BoardAction (ActionType.BombSpawn, temp.ID, BombermanOverlord.vecToFlArray(temp.transform.localPosition));
			BombermanGameStateUpdater.addNewAction (dropAction);
			bombsLeft--;
		}

		public void placedBombExplode(){
			bombsLeft++;
		}

		public void setCurrentCoord(int[] newCoord){
			lastCoord = (int[])currentCoord.Clone ();
			currentCoord = newCoord;
		}
			
		public override MapObj convertToMapObj(){
			MapObj temp = new MapObj (ID, type, BombermanOverlord.vecToFlArray (transform.localPosition), bombsLeft);
			temp.targetPos = BombermanOverlord.vecToFlArray (targetPos);
			temp.color = color;
			return temp;
		}
			

		public float[] getFloatPos(){return BombermanOverlord.vecToFlArray (transform.position);}
		private void printCoord(int[] pos){print (pos [0] + "." + pos [1]);}
	}








	public enum MoveDirections{
		None = -1,
		Right = 0,
		Up = 1,
		Left = 2,
		Down = 3
	}

}