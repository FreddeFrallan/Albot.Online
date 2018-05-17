using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.AnimatedLineRenderer;

namespace Snake{
	public class SnakeHead : MonoBehaviour{

		public AnimatedLineRenderer tailRenderer;
		private List<Vector3> targetPos = new List<Vector3>();
		private Vector3 nextTarget, oldPos, delta;

		public void startNewTargetPos(){
			oldPos = nextTarget;
			nextTarget = targetPos [0];
			targetPos.RemoveAt (0);

			delta = nextTarget - oldPos;
		}
			
		public void init(Material m){
			nextTarget = transform.position;
			GetComponent<MeshRenderer> ().material = m;
			GetComponent<LineRenderer> ().material = m;
		}
			
		public void interpolateBetweenPos(float procentCovered){
			transform.position = oldPos + delta*procentCovered;
			tailRenderer.Enqueue (transform.position);
		}
		public void addTargetPos(Vector3 pos){targetPos.Add (pos);}
	}
}