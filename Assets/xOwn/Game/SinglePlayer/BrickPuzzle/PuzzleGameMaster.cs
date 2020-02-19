using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleGameMaster : MonoBehaviour {

	public static float slideSpeed = 15;
	public SettingSlider size, shuffleAmount, moveSpeedSlider;
	public PuzzleGameLogic logic;
	private int[,] currentBoard;
	private int moveCounter = 0;
	private List<int> moveQ = new List<int>();
	private bool wonGame = false;

	void Start(){
		SinglePlayerGameMaster.init (takeInput, () => {});
		slideSpeed = moveSpeedSlider.slider.value;
    }


	public void takeInput(string msg){
		if (msg.ToLower () == "restart") {
			restartButtonPressed ();
			return;
		}

		foreach (char c in msg) {
			int move = 0;
			if (charToMove (c, ref move) == false)
				break;

			moveQ.Add (move);
		}
		playMove ();
	}

	private void playMove(){
		if (wonGame)
			return;
		
		if (moveQ.Count == 0) 
			TCPLocalConnection.sendMessage (formatBoard (currentBoard));
		else {
			int tempMove = moveQ [0];
			moveQ.RemoveAt (0);
			logic.playMove (tempMove);
		}
	}

	public void roundFinished(){
		moveCounter++;
		playMove ();
	}
	private IEnumerator sendBoardToPlayer(){
		yield return new WaitForSeconds (1);
		TCPLocalConnection.sendMessage (formatBoard (currentBoard));
	}

	private bool charToMove(char c, ref int move){
		if (char.IsDigit (c) == false)
			return false;

		move = int.Parse(c.ToString());
		return move >= 0 && move < 4;
	}


	public void restartGame(){
		currentBoard = logic.restartGame ((int)size.slider.value, (int)shuffleAmount.slider.value);
		moveCounter = 0;
		StartCoroutine (sendBoardToPlayer ());
	}
	public void victory(){
		wonGame = true;
		TCPLocalConnection.sendMessage ("Victory " + moveCounter);
	}


	private string formatBoard(int[,] board){
		string boardString = "";
		for (int y = 0; y < board.GetLength (0); y++)
			for (int x = 0; x < board.GetLength (0); x++)
				boardString += board [x, y].ToString () + " ";
		return boardString;
	}

	public void restartButtonPressed(){
		wonGame = false;
		moveQ.Clear ();
		restartGame ();
	}

	public void speedValueChanged(int t){
        if (t == moveSpeedSlider.slider.maxValue)
            slideSpeed = Mathf.Infinity;
        else
            slideSpeed = moveSpeedSlider.slider.value * 8;
	}
}
