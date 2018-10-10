using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowUserSessions : MonoBehaviour {

    public ChartAndGraph.InfoBox infoBox;
    public LoginData loginData;
    public Text text;
	
	void Start () {
		
	}

    void OnEnable() {
        text.text = loginData.getSessionsString(infoBox.getX());
    }


}
