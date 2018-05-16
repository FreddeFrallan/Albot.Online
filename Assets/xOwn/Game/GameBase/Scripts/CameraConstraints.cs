using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraConstraints : MonoBehaviour {

	private List<Vector3> posBuffer = new List<Vector3>();
	private int bufferSize = 5;

	void Start(){
		for(int i = 0; i < bufferSize; i++)
			posBuffer.Add (transform.position);
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (transform.position != posBuffer [posBuffer.Count - 1]) {
			posBuffer.Add (transform.position);
			posBuffer.RemoveAt (0);
		}
	}


	void OnCollisionEnter(){
		transform.position = posBuffer[0];
		print ("Yo");
	}
}
