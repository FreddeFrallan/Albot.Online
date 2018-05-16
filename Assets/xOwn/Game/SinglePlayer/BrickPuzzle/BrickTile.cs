using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrickTile : MonoBehaviour {

	public Text theText;
	public Image currentImage;
	public bool isMoving = false;
	public int value;
	public int[] pos;
	private PuzzleGameLogic logic;

	public void init(int value, int x, int y, PuzzleGameLogic logic){
		theText.text = value.ToString ();
		pos = new int[]{ x, y };
		this.logic = logic;
	}

	public void startSlide(Transform t, int x, int y){
		isMoving = true;
		StartCoroutine (slide (t));
		pos = new int[]{x, y};
	}

	private IEnumerator slide(Transform t){
		while (Vector3.Distance (transform.position, t.position) > 0.1) {
			transform.position = Vector3.MoveTowards (transform.position, t.position, PuzzleGameMaster.slideSpeed);
			yield return new WaitForEndOfFrame ();
		}	
		isMoving = false;
		logic.finishedRound ();
	}

	public override string ToString (){
		return pos [0] + "." + pos [1];
	}
}