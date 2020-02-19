using Barebones.MasterServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClientUIVersionText : MonoBehaviour {

    public TextMeshProUGUI text;

	// Use this for initialization
	void Start () {
        text.text = "Ver: " +  ConnectionToMaster.getAlbotVersion();	
	}
	
}
