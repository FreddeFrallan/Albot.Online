using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.AnimatedLineRenderer;

namespace Snake{
	public class SnakeHead : MonoBehaviour{

		public AnimatedLineRenderer tailRenderer;
		private List<Vector3> targetPos = new List<Vector3>();
		private Vector3 nextTarget, oldPos, delta;
        private float slideDist = 0.05f;

		public void startNewTargetPos(){
            slideToNewTarget(nextTarget);
            oldPos = nextTarget;
			nextTarget = targetPos [0];
			targetPos.RemoveAt (0);

			delta = nextTarget - oldPos;
		}

        private void slideToNewTarget(Vector3 targetPos) {
            while(Vector3.Distance(transform.position, targetPos) > slideDist){
                Vector3 slideDelta = (targetPos - transform.position).normalized * slideDist;
                transform.position = transform.position + slideDelta;
                tailRenderer.Enqueue(transform.position);
            }
            transform.position = targetPos;
            tailRenderer.Enqueue(transform.position);
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