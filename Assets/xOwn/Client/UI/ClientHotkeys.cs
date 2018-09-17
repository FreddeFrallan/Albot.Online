using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientHotkeys : MonoBehaviour {

    public KeyCode exitFullScreenButton = KeyCode.Escape;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(exitFullScreenButton))
            exitFullScreen();
	}

    private void exitFullScreen() {
        Screen.fullScreen = false;
    }
}
