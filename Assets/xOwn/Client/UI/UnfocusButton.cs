using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnfocusButton : MonoBehaviour {

	public EventSystem system;

	public void unfocusButton(){
		system.SetSelectedGameObject (null);
	}
}
