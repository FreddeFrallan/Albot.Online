using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientRealtimeGame{
	
	public class PositionInterpolation : MonoBehaviour {
		public bool isLocked = false;
		private Vector3 lastPos = Vector3.zero;
		private Vector3  nextPos = Vector3.zero;
		private Vector3 lerpSpeed = Vector3.zero;

		private float lastTime = 0;
		private float maxDeltaDist = 0.3f;



		// Update is called once per frame
		void Update () {
			transform.position += lerpSpeed * Time.deltaTime;
		}
			
		public void setNewPos(Vector3 currentPos, Vector3 targetPos, float time){
			isLocked = true;

			float dt = (time - lastTime)/1000;
			if (dt == 0)
				return;
			lastTime = time;

			lastPos = cloneVector (nextPos);
			nextPos = currentPos;

			if (Vector3.Distance (transform.position, lastPos) >= maxDeltaDist) //If we are currently of position by to much{
				transform.position = lastPos;

			lerpSpeed = (nextPos - lastPos)/dt;
			isLocked = false;
		}

		private Vector3 cloneVector(Vector3 original){
			return new Vector3 (original.x, original.y, original.z);
		}

		private Vector3 multVector(Vector3 target, float factor){
			return new Vector3 (target.x * factor, target.y * factor, target.z * factor);
		}

		private bool vectorMatch(Vector3 a, Vector3 b){
			return a.x == b.x && a.y == b.y && a.z == b.z;
		}
	}

}