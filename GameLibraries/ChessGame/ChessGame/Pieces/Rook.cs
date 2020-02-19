using System;
using System.Collections.Generic;

namespace ChessBot
{
	public class Rook : PlayingPiece
	{
		private List<int[]> moveDirections = new List<int[]> (){new int[2]{ -1, 0}, new int[2]{ 1, 0} , new int[2]{ 0, 1}, new int[2]{ 0, -1}};
		private bool canCastle = true;

		public Rook (Color color, int[] pos){
			this.playerColor = color;
			this.pos = pos;
			this.type = PieceType.rook;
			pieceSign = "R";
		}
			
		public override void onPlayingMove (){canCastle = false;}


		public override void calcAvailableMoves (PlayingPiece[,] board){
			avaliableMoves.Clear ();
			checkLongMoves (moveDirections, board);
		}

		public override PlayingPiece clone (){return new Rook (playerColor, (int[])pos.Clone ());}
	}
}

