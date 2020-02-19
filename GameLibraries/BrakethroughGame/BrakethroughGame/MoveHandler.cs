using System;

namespace BreakthroughGame{
	
	public class MoveHandler{
		private GameLogic logic;

		public MoveHandler(GameLogic logic){
			this.logic = logic;
		}

		public bool checkIfValidMove(string msg, ref int[] start, ref int[] target, int player){
			if (msg.Length < 4)
				return false;

			msg = msg.ToUpper ();	
			string startMsg = msg.Substring (0, 2);
			string targetMsg = msg.Substring (2, 2);
			if (stringToCoord (startMsg, ref start) == false || stringToCoord (targetMsg, ref target) == false)
				return false;

			return logic.validMove (start, target, player);
		}


		private bool stringToCoord(string msg, ref int[] coord){
			if (char.IsLetter (msg [0]) == false || char.IsDigit(msg[1]) == false)
				return false;

			coord = new int[]{ (int)msg [0] - (int)'A', int.Parse (msg [1].ToString ()) - 1};	
			return true;
		}

	}
}

