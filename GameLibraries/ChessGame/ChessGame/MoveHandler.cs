using System;
using System.Collections.Generic;

namespace ChessBot
{
	public class MoveHandler{


		public static Move isValidMove(Board currentBoard, bool white, int[] start, int[] target){
			List<Move> validMoves = currentBoard.getValidMoves (white);
			return validMoves.Find (x => (comparePos (x.start, start) && comparePos (x.target, target)));
		}

		public static BoardStatus getGameState(Board currentBoard, bool white){
			List<Move> validMoves = currentBoard.getValidMoves (white);
			if (validMoves.Count != 0)
				return BoardStatus.normal;

			if (white)
				return currentBoard.whiteInCheck ? BoardStatus.checkmate : BoardStatus.draw;
			else
				return currentBoard.blackInCheck ? BoardStatus.checkmate : BoardStatus.draw;
		}

		private static bool comparePos(int[] a, int[] b){return a [0] == b [0] && a [1] == b [1];}
	}

	public enum BoardStatus{
		normal,
		checkmate,
		draw
	}
}

