using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowInfoBox : MonoBehaviour {

    public ChartAndGraph.InfoBox infoBox;
    public DataController dataController;
    public Text text;
	
	void Start () {
		
	}

    void OnEnable() {
        text.text = dataController.getInfoString(infoBox.getX());
    }


}
