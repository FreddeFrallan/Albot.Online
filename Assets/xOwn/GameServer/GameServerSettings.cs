using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameServerSettings : MonoBehaviour {

    private int frameRate = 5;
	// Use this for initialization
	void Awake () {
		setGameServerSettings ();
	}

	public void setGameServerSettings(){
		Application.targetFrameRate = frameRate;
		Debug.LogError ("Setting framerate to: " + frameRate);
	}
}
