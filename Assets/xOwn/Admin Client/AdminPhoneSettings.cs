using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdminPhoneSettings : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (Application.isMobilePlatform == false)
            return;

        Screen.orientation = ScreenOrientation.Landscape;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
	
}
