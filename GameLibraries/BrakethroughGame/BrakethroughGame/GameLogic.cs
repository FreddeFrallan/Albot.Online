using System;

namespace BreakthroughGame{

	public class GameLogic{

		private int[,] gameField;
		private int startCharValue = 65;
		private int xSize, ySize;


		public GameLogic(int xSize, int ySize){
			this.xSize = xSize;
			this.ySize = ySize;
			initGameField ();
		}


		public void makeMove(int[] start, int[] target, int player){
			gameField [target [0], target [1]] = gameField [start [0], start [1]];
			gameField [start [0], start [1]] = 0;
		}


		public bool validMove(int[] start, int[] target, int player){
			int xDist = start [0] - target [0];
			int yDist = start [1] - target [1];

			if (Math.Abs (xDist) > 1 || yDist + player != 0)
				return false;

			if (target [0] >= xSize || target [0] < 0 || target [1] >= ySize || target [1] < 0)
				return false;
			if (start [0] >= xSize || start [0] < 0 || start [1] >= ySize || start [1] < 0)
				return false;

			int startValue = gameField [start [0], start [1]];
			int targetValue = gameField [target [0], target [1]];
			if (startValue != player || targetValue == player)
				return false;

			//Attack
			if (xDist != 0 && targetValue == player)
				return false;
			//Normal
			if (xDist == 0 && targetValue != 0)
				return false;

			return true;
		}



		private void initGameField(){
			gameField = new int[xSize, ySize];
			iterateBoard((int x, int y) => {
				if(y < 2)
					gameField[x, y] = 1;
				else if(y > 5)
					gameField[x, y] = -1;
				else
					gameField[x, y] = 0;
			});
		}

		private void iterateBoard(Action<int, int> a){
			for(int y = 0; y < ySize; y++)
				for(int x = 0; x < xSize; x++)
					a(x, y);
		}


		public bool winCheck(){
			for (int x = 0; x < xSize; x++) {
				if (gameField [x, 0] == -1)
					return true;

				if (gameField [x, ySize-1] == 1)
					return true;
			}

			return false;
		}


		private bool notationToCoord(string msg, out int[] coord){
			coord = new int[0];
			if (msg.Length < 2)
				return false;

			char x = msg.ToUpper()[0];
			char y = msg [1];

			if (char.IsLetter (x) == false || char.IsDigit(y) == false)
				return false;

			coord = new int[]{ (int)x - startCharValue, int.Parse (y.ToString()) };
			return true;
		}

		public int[,] getRawBoard(){
			return gameField;
		}

		public string formatBoard(){
			string s = "";
			iterateBoard ((int x, int y) => {
				s += gameField[x, y] + " ";
			});
			return s;
		}

	}

}

