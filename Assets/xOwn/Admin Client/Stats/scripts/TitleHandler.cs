using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class TitleHandler : MonoBehaviour {

    public Text title;

    private string baseText = "Amount of logins ";
    

	void Start () {
        title.text = baseText + DateTime.Now.Date.ToString("yyyy/MM/dd");
	}

    public void setDate(string date) {
        title.text = baseText + date;
    }
}
