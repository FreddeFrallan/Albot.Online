using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.AnimatedLineRenderer;
using Game;
using System.Linq;

namespace Snake{
	public class SnakeHead : MonoBehaviour{

		public AnimatedLineRenderer tailRenderer;
		private List<Vector3> targetPos = new List<Vector3>();
		private Vector3 nextTarget, oldPos, delta;
        private List<Vector3> queuePoints = new List<Vector3>();
        private float slideDist = 0.05f;

		public void init(Material m){
			nextTarget = transform.position;
			GetComponent<MeshRenderer> ().material = m;
			GetComponent<LineRenderer> ().material = m;
		}


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
                enqueuePos(transform.position);
            }
            transform.position = targetPos;
            enqueuePos(transform.position);
        }
			
			
		public void interpolateBetweenPos(float procentCovered){
			transform.position = oldPos + delta*procentCovered;
            enqueuePos(transform.position);
        }
        private void enqueuePos(Vector3 newPos) {
            if(queuePoints.Any(p => GameUtils.compareVec3(newPos, p)) == false) {
               tailRenderer.Enqueue(newPos);
                queuePoints.Add(newPos);
            }
        }

		public void addTargetPos(Vector3 pos){targetPos.Add (pos);}
	}
}