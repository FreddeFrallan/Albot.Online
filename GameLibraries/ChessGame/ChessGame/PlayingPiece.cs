using System;
using System.Collections.Generic;

namespace ChessBot
{
	public abstract class PlayingPiece
	{
		protected Color playerColor;
		public Color getColor(){return playerColor;}
		protected int[] pos;
		public int[] getPos(){return (int[])pos.Clone();}
		public PieceType type;
		protected string pieceSign;
		public string getPieceSign(){return playerColor == Color.black ? pieceSign.ToLower() : pieceSign;}
		public abstract PlayingPiece clone();
		public abstract void onPlayingMove();
		public void setPos(int[] newPos){pos = (int[])newPos.Clone ();}

		protected List<int[]> avaliableMoves = new List<int[]>();
		public List<int[]> getAvailableMoves(){return avaliableMoves;}
		public abstract void calcAvailableMoves (PlayingPiece[,] board);

		public SquareStatus getSquareState(PlayingPiece[,] board, int[] pos){
			if(pos[0] < 0 || pos[0] >= 8 || pos[1] < 0 || pos[1] >= 8)
				return SquareStatus.outOfBounds;

			return board[pos[0], pos[1]] == null ? SquareStatus.empty : SquareStatus.occupied;
		}

		public void checkLongMoves(List<int[]> directions, PlayingPiece[,] board){
			foreach (int[] dir in directions) {
				int[] checkPos = (int[])pos.Clone ();
				checkPos [0] += dir [0];
				checkPos [1] += dir [1];
				SquareStatus square = getSquareState (board, checkPos);

				while (square != SquareStatus.outOfBounds) {
					if (square == SquareStatus.occupied) {


						if (board [checkPos [0], checkPos [1]].getColor () != getColor ()) {
							avaliableMoves.Add ((int[])checkPos.Clone());
						}

						break;
					}


					avaliableMoves.Add ((int[])checkPos.Clone());

					checkPos [0] += dir [0];
					checkPos [1] += dir [1];
				 	square = getSquareState (board, checkPos);
				}
			}
		}


		public override string ToString (){
			return playerColor.ToString() + " " + type + "\t" + pos [0] + "." + pos [1];
		}
	}


	public enum Color{
		white, black
	}

	public enum PieceType{
		pawn,
		bishop,
		knight,
		rook,
		queen,
		king
	}

	public enum SquareStatus{
		empty,
		occupied,
		outOfBounds
	}
}

