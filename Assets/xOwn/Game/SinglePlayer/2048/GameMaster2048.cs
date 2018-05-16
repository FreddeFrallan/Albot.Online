using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster2048 : MonoBehaviour {


	public static float slideSpeed;
	public Slider moveSpeedSlider;
	public Text scoreText;
	public GameLogic2048 logic;
	private int[,] currentBoard;
	private List<int> moveQ = new List<int>();
	private bool playing = false;

	void Start(){
		SinglePlayerGameMaster.init (takeInput, restartGame);
		logic.init (gameOver, roundFinished, updateScoreText);
		slideSpeed = moveSpeedSlider.value;
		restartGame ();
	}


	public void takeInput(string msg){
		if (msg.ToLower () == "restart") {
			restartGame ();
			return;
		}
		if (playing == false)
			return;


		foreach (char c in msg) {
			int move = 0;
			if (charToMove (c, ref move) == false)
				break;

			moveQ.Add (move);
		}
		playMove ();
	}

	private void playMove(){
		if (moveQ.Count == 0) 
			TCPLocalConnection.sendMessage (logic.formatBoard ());
		else {
			int tempMove = moveQ [0];
			moveQ.RemoveAt (0);
			logic.playMove (tempMove);
		}
	}

	public void roundFinished(){playMove ();}
	private IEnumerator sendBoardToPlayer(){
		yield return new WaitForSeconds (1);
		TCPLocalConnection.sendMessage (logic.formatBoard ());
	}

	private bool charToMove(char c, ref int move){
		if (char.IsDigit (c) == false)
			return false;

		move = int.Parse(c.ToString());
		return move >= 0 && move < 4;
	}


	public void restartGame(){
		logic.startNewGame ();
		playing = true;
		StartCoroutine (sendBoardToPlayer ());
	}
	public void gameOver(){
		moveQ.Clear ();
		playing = false;
		TCPLocalConnection.sendMessage ("GameOver " + logic.getCurrentScore ());
		//restartGame ();
	}

	public void restartButtonPressed(){
		moveQ.Clear ();
		restartGame ();
	}

	public void updateScoreText(int newScore){scoreText.text = newScore.ToString ();}
	public void speedValueChanged(int t){slideSpeed = moveSpeedSlider.value;}
}
