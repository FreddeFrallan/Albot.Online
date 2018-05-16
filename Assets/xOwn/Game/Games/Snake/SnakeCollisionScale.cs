using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snake{

	public class SnakeCollisionScale : MonoBehaviour {

		public float maxScale = 2;
		public float minScale = 1;
		public float speed = 1f;

		private bool goingUp = true;

		// Update is called once per frame
		void Update () {
			float targetScale = transform.localScale.x + (goingUp ? speed : -speed)*Time.deltaTime;

			if (targetScale > maxScale || targetScale < minScale)
				goingUp = !goingUp;

			targetScale = Mathf.Clamp (targetScale, minScale, maxScale);
			transform.localScale = new Vector3 (targetScale, targetScale, targetScale);
		}
	}

}