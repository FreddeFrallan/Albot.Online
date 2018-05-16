using System;
using System.Collections.Generic;

namespace ChessBot
{
	public class Pawn : PlayingPiece
	{
		private bool canDoubleStep = true;
		private bool canEnPeasant = false;
		private int[] enPeasantPos;

		public Pawn (Color color, int[] pos, bool canDoubleStep = true, bool canEnPeasant = false){
			this.playerColor = color;
			this.pos = pos;
			this.type = PieceType.pawn;
			pieceSign = "P";
			this.canDoubleStep = canDoubleStep;
			this.canEnPeasant = canEnPeasant;
		}

		public override void onPlayingMove (){
			canDoubleStep = false;
			canEnPeasant = false;
		}

		public void giveAnPeasant(PlayingPiece[,] board){
			for (int i = -1; i <= 1; i += 2) {
				int[] sidePos = new int[2]{ pos [0] + i, pos [1] };
				SquareStatus temp = getSquareState(board, sidePos);
				if (temp != SquareStatus.occupied)
					continue;

				PlayingPiece p = board[sidePos[0], sidePos[1]];
				if (p.type != PieceType.pawn || p.getColor () == playerColor)
					continue;

				(p as Pawn).enableEnPeasant (this);
			}
		}

		public PlayingPiece[,] onPlayedOnPeasant(PlayingPiece[,] postBoard){
			int[] enemyPos = getPos ();
			enemyPos [1] += playerColor == Color.white ? -1 : 1;
			postBoard[enemyPos[0], enemyPos[1]] = null;
			return postBoard;
		}

		public void enableEnPeasant(PlayingPiece target){
			canEnPeasant = true;
			enPeasantPos = target.getPos();
			enPeasantPos [1] += playerColor == Color.white ? 1 : -1;
		}

		public override void calcAvailableMoves (PlayingPiece[,] board){
			avaliableMoves.Clear ();
			int yDir = playerColor == Color.white ? 1 : -1;

			//Forward Walk
			SquareStatus forwardSquare = getSquareState(board, new int[2]{pos[0], pos[1] + yDir});
			if (forwardSquare == SquareStatus.empty) {
				avaliableMoves.Add (new int[2]{ pos [0], pos [1] + yDir });
				//Double step
				if(canDoubleStep && getSquareState(board, new int[2]{pos[0], pos[1] + yDir*2}) == SquareStatus.empty)
					avaliableMoves.Add (new int[2]{ pos [0], pos [1] + yDir*2 });
			}

			//Attack Squares
			for (int x = -1; x < 2; x += 2) {
				int[] checkPos = new int[]{ pos [0] + x, pos [1] + yDir };
				SquareStatus square = getSquareState (board, checkPos);
				if (square == SquareStatus.occupied && board[pos[0] + x, pos[1] + yDir].getColor() != playerColor)
					avaliableMoves.Add ((int[])checkPos.Clone());
			}

			//EnPeasant
			if (canEnPeasant) {
				avaliableMoves.Add (enPeasantPos);
				canEnPeasant = false;
			}
		}


		public override PlayingPiece clone (){
			return new Pawn (playerColor, (int[])pos.Clone (), canDoubleStep, canEnPeasant);
		}
	}
}

