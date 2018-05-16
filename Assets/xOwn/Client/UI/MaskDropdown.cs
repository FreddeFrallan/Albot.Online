using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Barebones.MasterServer;

public class MaskDropdown : MonoBehaviour {

	public Vector2 activeSize;
	public Vector2 unActiveSize;

	public float speed = 10;
	public bool activated = false;

	private bool isMovingX = false;
	private bool isMovingY = false;

	private bool goingActive = false;
	private RectTransform theRect;

	void Start(){
		theRect = GetComponent<RectTransform> ();
		theRect.sizeDelta = activated ? activeSize : unActiveSize;
	}


	public void toggleActive(){
		/*
		goingActive = !goingActive;

		if (isMovingX == false)
			StartCoroutine (changeWidth());
		if (isMovingY == false)
			StartCoroutine (changeHeight());
			*/
	}

	public void turnOff(){
		if (goingActive)
			toggleActive ();
	}

	private IEnumerator changeWidth(){
		isMovingX = true;
		while (isMovingX) {
			Vector2 tempSize = theRect.sizeDelta;
			tempSize.x += goingActive ? speed * Time.deltaTime : -speed * Time.deltaTime;

			if (tempSize.x > activeSize.x || tempSize.x < unActiveSize.x) {
				tempSize.x = goingActive ? activeSize.x : unActiveSize.x;
				isMovingX = false;
			}

			theRect.sizeDelta = tempSize;
			yield return new WaitForEndOfFrame ();
		}
	}

	private IEnumerator changeHeight(){
		isMovingY = true;
		while (isMovingY) {
			Vector2 tempSize = theRect.sizeDelta;
			tempSize.y += goingActive ? speed * Time.deltaTime : -speed * Time.deltaTime;

			if (tempSize.y > activeSize.y || tempSize.y < unActiveSize.y) {
				tempSize.y = goingActive ? activeSize.y : unActiveSize.y;
				isMovingY = false;
			}

			theRect.sizeDelta = tempSize;
			yield return new WaitForEndOfFrame ();
		}
	}
}
