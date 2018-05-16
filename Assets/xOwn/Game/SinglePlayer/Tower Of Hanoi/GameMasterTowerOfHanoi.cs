using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMasterTowerOfHanoi : MonoBehaviour {

	public Slider amountSlider, speedSlider;
	public Text consoleText;
	public GameObject discPrefab;

	public List<Material> materials = new List<Material>();
	public List<TowerPole> poles = new List<TowerPole>();
	private List<GameObject> discs = new List<GameObject>();
	private List<List<int>> board = new List<List<int>> ();

	private List<int[]> moveQ = new List<int[]>();
	private bool wonGame = false;
	private int moveCounter = 0;


	void Start(){
		SinglePlayerGameMaster.init (takeInput, () => {});
		StartCoroutine (moveMaker ());
	}

	public void takeInput(string msg){
		msg = msg.ToLower ();
		if (msg == "restart") {
			restartButtonPressed ();
			return;
		}

		for(int i = 0; i < msg.Length-1; i += 2) {
			int[] move = new int[]{-1, -1};

			if (charToMove (msg[i+1], ref move[1]) == false)break;
			if (msg [i] == 'a')move [0] = 0;
			else if (msg [i] == 'b')move [0] = 1;
			else if (msg [i] == 'c')move [0] = 2;
			else break;

			moveQ.Add (move);
		}
	}

	private IEnumerator moveMaker(){
		while (true) {
			yield return new WaitForSeconds (2 - speedSlider.value / 50);
			if (wonGame == false && moveQ.Count != 0)
				playMove ();
		}
	}

	private void playMove(){
		if (wonGame)
			return;

		if (moveQ.Count == 0) 
			TCPLocalConnection.sendMessage (formatBoard ());
		else {
			int[] tempMove = moveQ [0];
			moveQ.RemoveAt (0);
			makeMove (tempMove[0], tempMove[1]);
		}

		if(moveQ.Count == 0 && wonGame == false)
			TCPLocalConnection.sendMessage (formatBoard ());
	}

	public void restartGame(){
		foreach (GameObject g in discs)
			Destroy (g);
		discs.Clear ();

		foreach (TowerPole tp in poles)
			tp.amountOfDiscs = 0;
		board.Clear ();


		board.Add (new List<int> ());
		board.Add (new List<int> ());
		board.Add (new List<int> ());

		for (int i = 0; i < (int)amountSlider.value; i++) {
			discs.Add( Instantiate (discPrefab, poles [0].addDisc (), Quaternion.identity));
			discs [discs.Count - 1].GetComponent<TowerDisc> ().init (materials [i], i);
			board [0].Add (i);
		}

		moveCounter = 0;
		wonGame = false;
		TCPLocalConnection.sendMessage (formatBoard ());
	}


	public void restartButtonPressed(){
		wonGame = false;
		moveQ.Clear ();
		restartGame ();
	}

	private bool charToMove(char c, ref int move){
		if (char.IsDigit (c) == false)
			return false;

		move = int.Parse(c.ToString());
		return move >= 0 && move < 4;
	}

	private void victory(){
		wonGame = true;
		consoleText.text = "Victory in " + moveCounter + " moves!";
		TCPLocalConnection.sendMessage ("Victory " + moveCounter);
	}

	public void makeMove(int start, int target){
		if (start == target || start < 0 || start > 2 || target < 0 || target > 2)
			return;

		List<int> startCol = board [start];
		List<int> targetCol = board [target];

		int movePiece = getLastIndex (startCol);
		int targetTopPiece = getLastIndex (targetCol);
		if (movePiece == -1 || targetTopPiece > movePiece)
			return;


		removeLast (startCol);
		targetCol.Add (movePiece);
		poles [start].removeDisc ();
		discs [movePiece].transform.position = poles [target].addDisc ();

		moveCounter++;
		consoleText.text = "Moves: " + moveCounter;
		if (checkForVictory ())
			victory ();
	}



	private int getLastIndex(List<int> list){
		if(list.Count == 0)
			return -1;
		return list [list.Count - 1];
	}

	private bool checkForVictory(){return board [0].Count == 0 && board [1].Count == 0;}
	private void removeLast(List<int> list){list.RemoveAt (list.Count - 1);}



	string[] poleLetters = new string[]{"A", "B", "C"};
	private string formatBoard(){
		string s = "";
		for (int i = 0; i < board.Count; i++)
			foreach (int disc in board[i])
				s += poleLetters [i] + disc + " ";
		return s;
	}
}
