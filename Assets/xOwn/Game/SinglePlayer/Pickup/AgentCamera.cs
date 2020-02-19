using System.Collections;
using System.Collections.Generic;
using UnityEngine;
	namespace Pickup{
	public class AgentCamera : MonoBehaviour {
		private static AgentCamera singleton;

		void Awake(){
			singleton = this;
		}

		public void setCameraPos(int boardSize){
			float factor = (float)boardSize;

			float x = -0.75f;
			float y = 0.5f;
			float z = -(3 + factor * 2.1f);
			Camera.main.transform.position = new Vector3(x, y, z);
		}

		public static void initCamera(int boardSize){singleton.setCameraPos (boardSize);}

	}
}