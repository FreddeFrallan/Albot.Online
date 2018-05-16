using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ClientUI;

public class PreGamePlayerSlot : MonoBehaviour {

	[SerializeField]
	private Image icon = null, checkbox = null;
	[SerializeField]
	private Text usernameTextField;

	private int iconNumber;
	private bool changedValues = false, isReady, clearPanels = false;
	private string username;

	//Small workaround that iconSprite can only be set from main thread.
	//And Local TCP connection is working on a different thread.
	void Update(){
		if (changedValues == false && clearPanels == false)
			return;


		if(clearPanels){
			clearPanel();
			clearPanels = false;
			return;
		}


		icon.sprite = ClientIconManager.loadIcon(iconNumber);
		icon.enabled = true;
		usernameTextField.text = username;
		checkbox.enabled = isReady;
		changedValues = false;
	}

	public void setUserPanel(int iconNumber, string username, bool isReady){
		this.iconNumber = iconNumber;
		this.username = username;
		this.isReady = isReady;
		changedValues = true;
	}

	public void startClearPanel(){
		clearPanels = true;
	}

	private void clearPanel(){
		usernameTextField.text = "";
		icon.enabled = false;
		checkbox.enabled = false;
	}

	public void setUserReady(bool status = true){
		checkbox.enabled = status;
	}
}
