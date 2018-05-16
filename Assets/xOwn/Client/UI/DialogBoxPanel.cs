using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogBoxPanel : MonoBehaviour {

	public Text infoText, buttonText;


	public void showPanel(string btnText, string fntText, int fntSize, int btnFntSize){
		buttonText.text = btnText;
		buttonText.fontSize = btnFntSize;
		infoText.text = fntText;
		infoText.fontSize = fntSize;

		this.gameObject.SetActive (true);
	}

	public void disable(){
		this.gameObject.SetActive(false);
	}
}
