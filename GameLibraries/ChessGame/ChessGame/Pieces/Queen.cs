using System;
using System.Collections.Generic;

namespace ChessBot
{
	public class Queen : PlayingPiece
	{
		private List<int[]> moveDirections = new List<int[]> (){new int[2]{ -1, 0}, new int[2]{ 1, 0} , new int[2]{ 0, 1}, new int[2]{ 0, -1}, new int[2]{ -1, -1}, new int[2]{ 1, 1} , new int[2]{ -1, 1}, new int[2]{ 1, -1}};

		public Queen (Color color, int[] pos){
			this.playerColor = color;
			this.pos = pos;
			this.type = PieceType.queen;
			pieceSign = "Q";
		}
			
		public override void onPlayingMove (){}


		public override void calcAvailableMoves (PlayingPiece[,] board){
			avaliableMoves.Clear ();
			checkLongMoves (moveDirections, board);
		}

		public override PlayingPiece clone (){return new Queen (playerColor, (int[])pos.Clone ());}
	}
}

