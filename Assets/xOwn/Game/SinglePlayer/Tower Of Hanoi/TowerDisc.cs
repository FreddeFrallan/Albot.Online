using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDisc : MonoBehaviour {

	public MeshRenderer rend;
	public int id;
	private float scaleFactor = 0.45f;

	public void init(Material mat, int id){
		rend.material = mat;
		this.id = id;
		scale ();
	}

	private void scale(){
		Vector3 scale = transform.localScale;
		scale.x -= scaleFactor * id;
		scale.z -= scaleFactor * id;
		transform.localScale = scale;
	}


}
