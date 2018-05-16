using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingPiece : MonoBehaviour {

	private float fallSpeed = 6;
	private float fallAcc = 15;

	public void dropPiece(float targetYPos){
		StartCoroutine (fall(targetYPos));
	}

	IEnumerator fall(float targetYPos){
		while (transform.position.y > targetYPos) {
			Vector2 tempPos = transform.position;
			tempPos.y -= fallSpeed * Time.deltaTime;
			transform.position = tempPos;

			if (tempPos.y <= targetYPos)
				break;

			fallSpeed += fallAcc * Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}

		Vector2 tempPosFinal = transform.position;
		tempPosFinal.y = targetYPos;
		transform.position = tempPosFinal;
	}
}
