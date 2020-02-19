using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snake{

	public class SnakeBlock : MonoBehaviour {

		public GameObject colorSphere;


		public Vector3 getPos(){return colorSphere.transform.position;}
		public void setColor(Material m){
			colorSphere.SetActive (true);
			colorSphere.GetComponent<MeshRenderer> ().material = m;
		}
	}

}