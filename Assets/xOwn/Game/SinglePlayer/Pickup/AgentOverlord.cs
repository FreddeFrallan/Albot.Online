using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


	namespace Pickup{
	public class AgentOverlord : MonoBehaviour {

		public static float moveSpeed;

		public AgentLogic logic;
		public SettingSlider sizeSlider, slideSpeed;
		public Toggle generate;
		public TextMeshProUGUI console;
		private List<int> moveQ = new List<int> ();
		private bool initBoard = false;
		private string[,] currentBoard;
		private int currentBoardSize;
		private bool victory = false;

		// Use this for initialization
		void Start () {
			togglePressed ();
			setMoveSpeed ();
			SinglePlayerGameMaster.init (takeInput, () => {});
		}
			
		private void playMove(){
			if (initBoard || victory)
				return;
		
			if (moveQ.Count == 0)
				TCPLocalConnection.sendMessage (formatBoard (currentBoard));
			else {
				int tempMove = moveQ [0];
				moveQ.RemoveAt (0);
				logic.playMove (tempMove);
			}
		}


		public void takeInput(string input){
			if (input.ToLower () == "restart") {
				newGameButtonClicked ();
				return;
			}

			if (initBoard) {
				if (parseInputedBoard (input) == false)
					requestBoard ();
				else {
					console.text = "";
					initBoard = false;
					currentBoard = logic.resetGame ((int)currentBoardSize, currentBoard);
					playMove ();
				}
				return;
			}
				
			foreach (char c in input) {
				if (char.IsDigit (c) == false)
					continue;

				int temp = int.Parse (c.ToString ());
				if(temp < 0 || temp > 3)
					continue;
				moveQ.Add (temp);
			}
			playMove ();
		}

		public void moveFinished(string[,] currentBoard){
			if (initBoard)
				return;
			
			this.currentBoard = currentBoard;
			playMove ();
		}

		public void newGameButtonClicked(){
			victory = false;
			initBoard = false;
			moveQ.Clear ();
			console.text = "";
			string[,] newBoard = new string[(int)sizeSlider.slider.value, (int)sizeSlider.slider.value];

			if (generate.isOn) {
				newBoard = generateNewBoard ();
				currentBoard = logic.resetGame ((int)sizeSlider.slider.value, newBoard);
				TCPLocalConnection.sendMessage (formatBoard (currentBoard));
			} else
				requestBoard ();
		}


		private string[,] generateNewBoard(){
			currentBoardSize = (int)sizeSlider.slider.value;
			return BoardGenerator.generateBoard(currentBoardSize);
		}

		private bool parseInputedBoard(string input){
			input = trimInput (input);
			currentBoardSize = Mathf.RoundToInt(Mathf.Sqrt (input.Length));
			currentBoard = new string[currentBoardSize, currentBoardSize];
			int p = 0, d = 0, i = 0;


			for (int y = 0; y < currentBoardSize; y++)
				for (int x = 0; x < currentBoardSize; x++) {
					currentBoard [x, y] = input [x + y * currentBoardSize].ToString ().ToUpper();
					if (currentBoard [x, y] == "P")p++;
					if (currentBoard [x, y] == "D")d++;
					if (currentBoard [x, y] == "I")i++;
				}

			return p == 1 && d == 1 && i > 0;
		}

		private void requestBoard(){
			if(initBoard)console.text = "Invalid start board";
			else console.text = "Waiting for board";
			
			TCPLocalConnection.sendMessage ("InitBoard");
			initBoard = true;
		}

		private string trimInput(string input){
			string s = "";
			foreach (char c in input)
				if (char.IsWhiteSpace (c) == false)
					s += c.ToString ();
			return s;
		}


		public void gameOver(int moves){
			console.text = "Victory!\nSteps: " + moves.ToString ();
			TCPLocalConnection.sendMessage ("Victory " + moves.ToString ());
			victory = true;
		}
		public void togglePressed(){sizeSlider.slider.interactable = generate.isOn;}
		public void setMoveSpeed(int x = 0){moveSpeed = slideSpeed.slider.value / 100;}
		private string formatBoard(string[,] board){
			string s = "";
			for (int y = 0; y < currentBoardSize; y++)
				for (int x = 0; x < currentBoardSize; x++)
					s += board [x, y] + " ";
			return s;
		}
			
	}
}