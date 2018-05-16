using Game.Math;
using System.Collections.Generic;

namespace Connect4{

	public class BoardLogic {

		public class IllegalMove : System.Exception { public IllegalMove() {} } 

		public enum Piece {
			Red = -1,
			None,
			Yellow
		}

		public static readonly int ROWS = 6;
		public static readonly int COLS = 7;
		private Matrix mBoard;

		public BoardLogic() {
			mBoard = new Matrix(COLS, ROWS);
		}

		public override string ToString() {
			string s = "";
			for (int c = 0; c < COLS; ++c) {
				for (int r = ROWS - 1; r >= 0; --r) {
					// To make things compatible for now
					Piece p = (Piece)mBoard.at(c, r);
					if (p == Piece.Red) s += "1 ";
					else if (p == Piece.Yellow) s += "2 ";
					else s += "0 ";
				}
			}
			return s;
		}

		public void dropPiece(Piece m, int col) {
			int r = ROWS - 1;
			while (r >= 0 && (Piece)mBoard.at(col, r) == Piece.None) --r;
			if (r == ROWS - 1) 
				throw new IllegalMove();
			mBoard.set(col, r + 1, (int)m);
		}

		public bool isAllowedMove(int col) {
			return (Piece)mBoard.at(col, ROWS - 1) == Piece.None;
		}

		public Game.PlayerColor hasWinner() {
			Matrix rows = new Matrix(4, 1, new List<int>{1, 1, 1, 1});
			foreach (int v in mBoard.convolve(rows)) {
				if (v == -4) return Game.PlayerColor.Red;
				else if (v == 4) return Game.PlayerColor.Yellow;
			}

			Matrix cols = new Matrix(1, 4, new List<int>{1, 1, 1, 1});
			foreach (int v in mBoard.convolve(cols)) {
				if (v == -4) return Game.PlayerColor.Red;
				else if (v == 4) return Game.PlayerColor.Yellow;
			}

			Matrix diag = new Matrix(4, 4, 
				new List<int>{1, 0, 0, 0,
					0, 1, 0, 0,
					0, 0, 1, 0,
					0, 0, 0, 1,});
			foreach (int v in mBoard.convolve(diag)) {
				if (v == -4) return Game.PlayerColor.Red;
				else if (v == 4) return Game.PlayerColor.Yellow;
			}

			Matrix diag2 = new Matrix(4, 4, 
				new List<int>{0, 0, 0, 1,
					0, 0, 1, 0,
					0, 1, 0, 0,
					1, 0, 0, 0,});
			foreach (int v in mBoard.convolve(diag2)) {
				if (v == -4) return Game.PlayerColor.Red;
				else if (v == 4) return Game.PlayerColor.Yellow;
			}
			return Game.PlayerColor.None;
		}

		public void print() {
			mBoard.print();
		}
	}

} // namespace Connect4
