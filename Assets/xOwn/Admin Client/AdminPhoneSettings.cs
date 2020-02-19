using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdminPhoneSettings : MonoBehaviour {

    public GameObject menuBar, mainMenuText;

	// Use this for initialization
	void Start () {
        if (Application.isMobilePlatform == false)
            return;

        Screen.orientation = ScreenOrientation.Landscape;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        setMenuBarSize();
    }

    private void setMenuBarSize() {
        if(SystemInfo.deviceType == DeviceType.Handheld) {
            // Hardcoded af!
            menuBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50);
            menuBar.transform.position.Set(menuBar.transform.position.x, -20, menuBar.transform.position.z);
            mainMenuText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 32);
        }
    }
	
}
