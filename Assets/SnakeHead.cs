using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snake{
	public class SnakeHead : MonoBehaviour{
		private List<Vector3> targetPos = new List<Vector3>();
		private Vector3 nextTarget, oldPos, delta;


		public void startNewTargetPos(){
			oldPos = nextTarget;
			nextTarget = targetPos [0];
			targetPos.RemoveAt (0);

			delta = nextTarget - oldPos;
		}

		public void init(Material m){
			oldPos = transform.position;
			GetComponent<MeshRenderer> ().material = m;
		}
		public void interpolateBetweenPos(float procentCovered){transform.position = oldPos + delta*procentCovered;}
		public void addTargetPos(Vector3 pos){targetPos.Add (pos);}
	}
}