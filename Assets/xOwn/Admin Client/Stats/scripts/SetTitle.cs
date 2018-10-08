using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class SetTitle : MonoBehaviour {

    public Text title;

	// Use this for initialization
	void Start () {
        title.text = "Amount of logins " + DateTime.Now.Date.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
