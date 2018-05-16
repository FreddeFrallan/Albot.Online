using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Othello{
	
	public class OthelloPiece : MonoBehaviour {
		public bool isWhite = false;
		private bool isFlipping = false;
		private float flipTime = 0.3f;
		private float flipHeight = 15;
		private int flipQueue = 0;
		private Vector3 localRot = Vector3.zero;

		public void setColor(bool white){
			localRot = white ? new Vector3 (180, 0, 0) : new Vector3(0,0,0);
			isWhite = white;
		}

		public void flipToNewColor(bool targetWhite){
			if (targetWhite == isWhite)
				return;
			isWhite = targetWhite;
			if (isFlipping == false) {
				isFlipping = true;
				StartCoroutine (flip ());
			} else 
				flipQueue++;

		}

		void Update(){
			if (flipQueue > 0 && isFlipping == false) {
				flipQueue--;
				isFlipping = true;
				StartCoroutine (flip ());
			}
			transform.localEulerAngles = localRot;
		}

		private IEnumerator flip(){
			float startRot = Mathf.Round( localRot.x);
			Vector3 startPos = transform.position;
			float targetHeight = 0;
			float flipStartTime = Time.time;

			while (isFlipping) {
				float targetRot = localRot.x;
				if (Time.time >= flipStartTime + flipTime) {
					isFlipping = false;
					targetRot = startRot + 180;
					transform.position = startPos;
					localRot = new Vector3 (targetRot, 0, 0);
					break;
				}
				else {
					float t = (Time.time - flipStartTime) / (flipTime);
					targetRot  = Mathf.LerpAngle (startRot, startRot + 180, t);
					targetHeight = Mathf.Sin (Mathf.Deg2Rad *( targetRot % 180)) * flipHeight;
					transform.position = startPos + new Vector3 (0, targetHeight, 0);
					localRot = new Vector3 (targetRot, 0, 0);
				}
				yield return new WaitForEndOfFrame ();
			}
		}

	}
}