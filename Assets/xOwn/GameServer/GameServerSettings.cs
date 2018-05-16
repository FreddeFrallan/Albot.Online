using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameServerSettings : MonoBehaviour {

	public int serverFPS = 10;

	// Use this for initialization
	void Awake () {
		setGameServerSettings (serverFPS);
	}

	public void setGameServerSettings(int fps){
		Application.targetFrameRate = fps;
		print ("Setting framerate to: " + fps);
	}
}
