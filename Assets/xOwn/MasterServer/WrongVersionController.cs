using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrongVersionController : MonoBehaviour {

	private string DOWNLOAD_URL = "https://pingul.github.io";
	public GameObject UIPanel;

	public void activateWrongVersionPanel(){
		gameObject.SetActive (true);
	}


	public void onTakeMeToWebsiteClick(){
		Application.OpenURL (DOWNLOAD_URL);
	}
}
