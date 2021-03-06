using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tile2048 : MonoBehaviour {

	public TextMeshProUGUI theText;
	public Image currentImage;
	public int[] currentPos;
	public int currentScore;
	public bool isMoving = false;
	private bool hasMerged = false;
	private Tile2048 mergeTarget;
	private GameLogic2048 logic;

	private static int[] textSize = new int[]{42, 38, 30, 25, 20, 17, 15};
	private static Vector3[] RGBcolors = new Vector3[]{new Vector3(238,228,218), new Vector3(236,224,200),  new Vector3(242,176,121),
		new Vector3(246,248,99), new Vector3(250,147,95), new Vector3(246,93,61), new Vector3(241,215,105), new Vector3(235,211,88),
		new Vector3(229, 192, 41), new Vector3(227, 186, 20), new Vector3(255, 60, 62), new Vector3(60, 145, 62), new Vector3(60, 145, 145),  new Vector3(230, 54, 145)
		, new Vector3(94, 200, 230), new Vector3(255, 255, 255)};


	public void init(int score, int[] pos, GameLogic2048 logic){
		currentPos = pos;
		currentScore = score;
		theText.text = currentScore.ToString ();
		setTextValues ();
		this.logic = logic;
	}

	public static int getSmartScore(int score){
		if (score == 0)
			return 0;

		int sum = 1;
		for (int i = 1; i < 30; i++) {
			sum *= 2;
			if (sum == score)
				return i;
		}

		print ("Error smart value");
		return -1;
	}

	public void startSlide(GameObject target, int[] newPos){
		currentPos = newPos;
		isMoving = true;
		StartCoroutine (slide (target));
	}



	private IEnumerator slide(GameObject target){
		while (Vector3.Distance (transform.position, target.transform.position) > 0.1) {
			transform.position = Vector3.MoveTowards (transform.position, target.transform.position, GameMaster2048.slideSpeed);
			yield return new WaitForEndOfFrame ();
		}	
		isMoving = false;
	}


	public int[] calcNewPos(int[] pos, Tile2048[,] board){
		int posX = pos [0] + 1; int posY = pos [1];
		int[] finalPos = pos;

		while (posX < 4) {
			if (board [posX, posY] == null)
				finalPos [0] = posX;
			 else break;
			posX++;
		} 
		return finalPos;
	}

	public bool merge(){
		if (mergeTarget == null)
			return false;
		mergeTarget.updateScore ();
		logic.addToScore (currentScore);
		Destroy (gameObject);
		return true;
	}

	public void findMergeTargets(int[] pos, Tile2048[,] board){
		hasMerged = false;
		if (pos [0] + 1 >= 4)
			return;
		if(board[pos[0] + 1, pos[1]].canMerge(currentScore)){
			hasMerged = true;
			mergeTarget = board [pos [0] + 1, pos [1]];
		}
	}


	public bool canMerge(int score){
		if (hasMerged)
			return false;
		return score == currentScore;
	}


	public void updateScore(){
		currentScore *= 2;
		theText.text = currentScore.ToString ();
		setTextValues ();
	}

	private void setTextValues(){
		int colorIndex = getSmartScore (currentScore) - 1;
        Color tempC = ColorSchema2048.brickColors[colorIndex];
		currentImage.color = tempC;

		theText.fontSize = textSize [theText.text.Length-1];
		theText.color = colorIndex > 1 ? Color.white : Color.black;
        theText.alignment = TextAlignmentOptions.Center;
	}
}