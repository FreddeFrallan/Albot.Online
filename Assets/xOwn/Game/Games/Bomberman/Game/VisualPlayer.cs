using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;


namespace Bomberman{

	public class VisualPlayer : BombermanVisualObj {

		private float moveSpeed = 3.15f;
		private float maxOffDist = 1f;
		private Vector3 targetPos;
		private bool dead = false;


		public void init(PlayerColor color, Material mat, Vector3 startPos, int id){
			initVisualObj (color, id);
			GetComponent<MeshRenderer> ().material = mat;

			transform.position = startPos;
			targetPos = transform.position;
			StartCoroutine (lerpToPos ());
		}

		private IEnumerator lerpToPos(){
			while (true) {
				transform.position = Vector3.MoveTowards (transform.position, targetPos, moveSpeed * Time.deltaTime);
				yield return new WaitForEndOfFrame ();
			}
		}


		public void death(){
			dead = true;
			Destroy (gameObject);
		}

		public void moveToPos(Vector3 currentPos, Vector3 targetPos){
			if (dead)
				return;

			this.targetPos = targetPos;
			if (Vector3.Distance (transform.localPosition, currentPos) > maxOffDist)
				transform.localPosition = currentPos;
		}
	}
}