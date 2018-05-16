using System;
using System.Collections.Generic;

namespace ChessBot
{
	public class Knight : PlayingPiece
	{
		private List<int[]> moveDirections = new List<int[]> (){new int[2]{ -1, 2}, new int[2]{ 1, 2} , new int[2]{-2, 1}, new int[2]{2, 1}, new int[2]{-2, -1}, new int[2]{2, -1} , new int[2]{-1, -2}, new int[2]{1, -2}};

		public Knight (Color color, int[] pos){
			this.playerColor = color;
			this.pos = pos;
			this.type = PieceType.knight;
			pieceSign = "KN";
		}

		public override void onPlayingMove (){}

		public override void calcAvailableMoves (PlayingPiece[,] board){
			avaliableMoves.Clear ();
			foreach (int[] dir in moveDirections) {
				int[] checkPos = new int[2]{ pos [0] + dir [0], pos [1] + dir [1] };

				SquareStatus square = getSquareState (board, checkPos);
				if (square == SquareStatus.outOfBounds)
					continue;

				if (square == SquareStatus.empty)
					avaliableMoves.Add ((int[])checkPos.Clone ());
				else if(board[checkPos[0], checkPos[1]].getColor() != playerColor)
					avaliableMoves.Add ((int[])checkPos.Clone ());
			}
		}

		public override PlayingPiece clone (){return new Knight (playerColor, (int[])pos.Clone ());}
	}
}

