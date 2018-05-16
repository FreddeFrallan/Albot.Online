using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess{
	public class ChessPiece : MonoBehaviour{
		public Color playerColor;
		public int pos;
		public PieceType type;
		private float speed = 150;
		private List<Vector3> moveTargets = new List<Vector3>();
		private Vector3 currentTarget;

		public ChessPiece init(Material mat){
			setMaterial (mat);
			return this;
		}

		public void move(Vector3 targetPos){
			moveTargets.Add (targetPos);
			if (moveTargets.Count == 1)
				StartCoroutine (slideToPosition ());
		}

		IEnumerator slideToPosition(){
			while (moveTargets.Count > 0) {
				currentTarget = moveTargets [0];
				float currentDist = Vector3.Distance (transform.position, currentTarget);
				while (currentDist > 2) {
					Vector3 dir = Vector3.Normalize (currentTarget - transform.position);
					transform.position += dir * speed * Time.deltaTime;

					float newDist = currentDist = Vector3.Distance (transform.position, currentTarget);
					if (newDist > currentDist)
						break;
					yield return new WaitForEndOfFrame ();
				}
				transform.position = currentTarget;
				moveTargets.RemoveAt (0);
			}
			
		}



		public void removeChessPiece(){
			Destroy (this.gameObject);
		}

		public void setMaterial(Material mat){
			MeshRenderer meshR = GetComponent<MeshRenderer> ();
			if (meshR == null) {
				foreach (MeshRenderer m in transform.GetComponentsInChildren<MeshRenderer>())
					m.material = mat;
			}
			else
				meshR.material = mat;
		}
	}

	
	public enum Color{
		white, black
	}
	
	public enum PieceType{
		pawn,
		bishop,
		knight,
		rook,
		queen,
		king
	}
}