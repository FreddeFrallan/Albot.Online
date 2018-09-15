using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.AnimatedLineRenderer;
using Game;
using System.Linq;

namespace Snake{
	public class SnakeHead : MonoBehaviour{

        public GameObject tailPrefab;
        public SnakeExplosion snakeExploder;
        private AnimatedLineRenderer tailRenderer;
		private List<Vector3> targetPos = new List<Vector3>();
		private Vector3 nextTarget, oldPos, delta;
        private List<Vector3> queuePoints = new List<Vector3>();
        private float slideDist = 0.05f;
        private int lastPointsAdded = 5;

        private Material snakeMaterial;

		public void init(Material m){
			nextTarget = transform.position;
            snakeMaterial = m;
			GetComponent<MeshRenderer> ().material = m;
            createNewTailObject();
        }

        private void createNewTailObject() {
            if (tailRenderer != null && tailRenderer.gameObject != null) //Set old tail to static
                tailRenderer.gameObject.isStatic = true;

            GameObject temp = Instantiate(tailPrefab, transform.position, Quaternion.identity, transform);
            tailRenderer = temp.GetComponent<AnimatedLineRenderer>();
            temp.GetComponent<LineRenderer>().material= snakeMaterial;

            //Add the last points to our new line renderer
            List<Vector3> tempList = new List<Vector3>();
            for (int i = lastPointsAdded; i > 0; i--) {
                if (i > queuePoints.Count)
                    continue;

                tempList.Add(queuePoints[queuePoints.Count - i]);
            }
            queuePoints.Clear();

            tempList.ForEach(p => enqueuePos(p));
            enqueuePos(transform.position);
        }

		public void startNewTargetPos(){
            slideToNewTarget(nextTarget);
            oldPos = nextTarget;
			nextTarget = targetPos [0];
			targetPos.RemoveAt (0);

			delta = nextTarget - oldPos;
            createNewTailObject();
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

        public void explodeSnake() {
            StartCoroutine(snakeExploder.SplitMesh(true));
        }
	}
}