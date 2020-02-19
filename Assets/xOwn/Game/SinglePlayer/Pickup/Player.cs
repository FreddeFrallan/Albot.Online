using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pickup{

	public class Player : MonoBehaviour {

		private AgentLogic logic;
		private bool canMove = true, hasPickup = false;
		private int pickupsLeft;
		private List<int[]> blockedSquares = new List<int[]>();

		private Action finishedMove, lostGame, wonGame;
		public int[] currentPos;

		private string lastValue = "-";

		public void init(Action finishedMove, Action wonGame, int amountOfPickups, int[] playerPos, List<int[]> notAllowedSquares, AgentLogic logic){
			this.finishedMove = finishedMove;
			this.wonGame = wonGame;
			this.logic = logic;
			pickupsLeft = amountOfPickups;
			currentPos = playerPos;
			blockedSquares = notAllowedSquares;
		}


		public void takeInput(int dir){
			if (canMove == false || canMoveToSquare (dir) == false) {
				finishedMove ();
				return;
			}
			canMove = false;
			logic.resetPlayerSquare (currentPos, lastValue);
			if (dir == 0)currentPos[0]++;
			if (dir == 1)currentPos[1]--;
			if (dir == 2)currentPos[0]--;
			if (dir == 3)currentPos[1]++;
			lastValue = logic.getChar (currentPos);

			StartCoroutine (move (calcMoveTarget (dir)));
		}

		private Vector3 calcMoveTarget(int dir){
			if (dir == 0)return transform.position + new Vector3 (AgentLogic.squareSize, 0, 0);
			if (dir == 1)return transform.position + new Vector3 (0, AgentLogic.squareSize, 0);
			if (dir == 2)return transform.position + new Vector3 (-AgentLogic.squareSize, 0, 0);
			return transform.position + new Vector3 (0, -AgentLogic.squareSize, 0);
		}



		private IEnumerator move(Vector3 target){
			while (Vector3.Distance (target, transform.position) > 0.01) {
				transform.position = Vector3.MoveTowards (transform.position, target, AgentOverlord.moveSpeed);
				yield return new WaitForEndOfFrame ();
			}
			transform.position = target;
			canMove = true;
			finishedMove ();
		}



		private void OnCollisionEnter(Collision coll){
			if (coll.collider.tag == "Obsticale")obsticaleCollision ();
			if (coll.collider.tag == "Item")itemCollision (coll.gameObject);
			if (coll.collider.tag == "DropPoint")dropPointCollision ();
		}



		private bool canMoveToSquare(int dir){
			int x = currentPos[0], y = currentPos[1];
			if (dir == 0)x++;
			if (dir == 1)y--;
			if (dir == 2)x--;
			if (dir == 3)y++;

			if (x < 0 || x >= AgentLogic.boardSize || y < 0 || y >= AgentLogic.boardSize)
				return false;
			return blockedSquares.Find (t => (t [0] == x && t[1] == y )) == null;
		}


		private void obsticaleCollision(){
			canMove = true;
		//	lostGame ();
		}

		private void itemCollision(GameObject ítem){
			if (hasPickup)
				return;
			hasPickup = true;
			lastValue = "-";
			Destroy (ítem);
			hasPickup = true;
		}

		private void dropPointCollision(){
			if (hasPickup == false)
				return;
			hasPickup = false;
			pickupsLeft--;

			if (pickupsLeft == 0)
				wonGame ();
		}
	}

}