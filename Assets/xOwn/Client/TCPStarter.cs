using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCPStarter : MonoBehaviour{
	void Start(){
		if (TCPLocalConnection.isReady == false)
			TCPLocalConnection.init ();

	//	Destroy (gameObject);
	}
}