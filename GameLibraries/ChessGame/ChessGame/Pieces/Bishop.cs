using System;
using System.Collections.Generic;

namespace ChessBot
{
	public class Bishop : PlayingPiece
	{
		private List<int[]> moveDirections = new List<int[]> (){new int[2]{ -1, -1}, new int[2]{ 1, 1} , new int[2]{ -1, 1}, new int[2]{ 1, -1}};

		public Bishop (Color color, int[] pos){
			this.playerColor = color;
			this.pos = pos;
			this.type = PieceType.bishop;
			pieceSign = "B";
		}

		public override void onPlayingMove (){}
			
		public override void calcAvailableMoves (PlayingPiece[,] board){
			avaliableMoves.Clear ();
			checkLongMoves (moveDirections, board);
		}
			
		public override PlayingPiece clone (){return new Bishop (playerColor, (int[])pos.Clone ());}
	}
}

