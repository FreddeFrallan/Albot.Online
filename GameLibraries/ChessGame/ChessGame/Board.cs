using System;
using System.Collections.Generic;

namespace ChessBot
{
	public class Board
	{
		private PlayingPiece[,] board = new PlayingPiece[8, 8];
		private List<PlayingPiece> whites = new List<PlayingPiece>();
		private List<PlayingPiece> blacks = new List<PlayingPiece>();

		public bool whiteInCheck = false;
		public bool blackInCheck = false;

	
		public Board(){initStartBoard ();}
		public Board(PlayingPiece[,] state){
			board = state;
			initPlayerLists ();
		}
			
		private List<Move> getAvailableMoves(bool white){
			List<Move> moves = new List<Move> ();

			foreach (PlayingPiece p in (white? whites : blacks)) {		
				p.calcAvailableMoves (board);

				foreach (int[] target in p.getAvailableMoves ()) {
					
					bool isCheckMove = board [target [0], target [1]] != null && board [target [0], target [1]].type == PieceType.king;
					bool isCapture = board [target [0], target [1]] != null && isCheckMove == false;
					moves.Add(new Move(p.getPos(), target, isCheckMove, isCapture, p.type));
				}
			}

			return moves;
		}
			
		public void analyzeBoard(bool white){
			List<Move> moves = getAvailableMoves (white);
			foreach (Move m in moves) {
				if (m.isCheck) {
					blackInCheck = white;
					whiteInCheck = !white;
				}
			}
		}

		public List<Move> getValidMoves(bool white){
			List<Move> moves = getAvailableMoves (white);


			List<Move> validMoves = new List<Move> ();
			foreach (Move m in moves)
				if (analyzeIfValidMove (m, white))
					validMoves.Add (m);
			
			return validMoves;
		}

		private bool analyzeIfValidMove(Move m, bool white){
			Board postBoard = playMove (m);
			postBoard.analyzeBoard (!white);
			if (white)
				return !postBoard.whiteInCheck;
			else 
				return !postBoard.blackInCheck;
		}


		private void initStartBoard(){
			//Pawns
			for (int x = 0; x < 8; x++) {
				board [x, 1] = new Pawn(Color.white, new int[2]{x, 1});
				board [x, 6] = new Pawn(Color.black, new int[2]{x, 6});
			}

			//Rooks
			board [0, 0] = new Rook(Color.white, new int[2]{0, 0});
			board [7, 0] = new Rook(Color.white, new int[2]{7, 0});
			board [0, 7] = new Rook(Color.black, new int[2]{0, 7});
			board [7, 7] = new Rook(Color.black, new int[2]{7, 7});
		
			//Knights
			board [1, 0] = new Knight(Color.white, new int[2]{1, 0});
			board [6, 0] = new Knight(Color.white, new int[2]{6, 0});
			board [1, 7] = new Knight(Color.black, new int[2]{1, 7});
			board [6, 7] = new Knight(Color.black, new int[2]{6, 7});

			//Bishops
			board [2, 0] = new Bishop(Color.white, new int[2]{2, 0});
			board [5, 0] = new Bishop(Color.white, new int[2]{5, 0});
			board [2, 7] = new Bishop(Color.black, new int[2]{2, 7});
			board [5, 7] = new Bishop(Color.black, new int[2]{5, 7});

			//Queens
			board [3, 0] = new Queen(Color.white, new int[2]{3, 0});
			board [3, 7] = new Queen(Color.black, new int[2]{3, 7});

			//Kings
			board [4, 0] = new King(Color.white, new int[2]{4, 0});
			board [4, 7] = new King(Color.black, new int[2]{4, 7});

			initPlayerLists ();
		}
		private void initPlayerLists(){
			for (int x = 0; x < 8; x++)
				for (int y = 0; y < 8; y++) {
					PlayingPiece p = board [x, y];
					if (p == null)
						continue;
					
					if (p.getColor () == Color.white)
						whites.Add (p);
					else
						blacks.Add (p);
				}
		}


		public override string ToString (){
			string chessBoard = "";
			for (int y = 7; y >= 0; y--) {
				for (int x = 0; x < 8; x++) {
					chessBoard += board [x, y] == null ? "-" : board [x, y].getPieceSign ();
					chessBoard += "\t";
				}
				chessBoard += "\n";
			}
			return chessBoard;
		}


		public Board playMove(Move move){
			PlayingPiece[,] postField = cloneField (board);
			PlayingPiece movePiece = postField [move.start [0], move.start [1]];
			postField [move.start [0], move.start [1]] = null;
			postField [move.target [0], move.target [1]] = movePiece;
			movePiece.setPos (move.target);
			movePiece.onPlayingMove ();

			if (move.type == PieceType.pawn)
				postField = handlePawnSpecialls (postField, move, movePiece);
				
			
			return new Board (postField);
		}

		private PlayingPiece[,] handlePawnSpecialls(PlayingPiece[,] postField, Move move, PlayingPiece movePiece){
			if (Math.Abs (move.start [1] - move.target [1]) == 2)  //Played double step, give enpeasant!
				(movePiece as Pawn).giveAnPeasant (postField);

			if (move.start [0] != move.target [0])//Played enpeasant! remove targetEnpeasant
				postField = (movePiece as Pawn).onPlayedOnPeasant(postField);
			
			return postField;
		}

		public static PlayingPiece[,] cloneField(PlayingPiece[,] board){
			PlayingPiece[,] postField = new PlayingPiece[8,8];
			for (int x = 0; x < 8; x++)
				for (int y = 0; y < 8; y++)
					if (board [x, y] != null)
						postField [x, y] = board [x, y].clone ();
			
			return postField;
		} 
	}

	public class Move{
		public int[] target, start;
		public bool isCheck = false, isCapture = false;
		public PieceType type;
		public Move(int[] start, int[] target, bool isCheck, bool isCapture, PieceType type){this.start = start; this.target = target; this.isCheck = isCheck; this.isCapture = isCapture; this.type = type;}
		public override string ToString (){
			string capture = isCapture ? "\tCapture" : "";
			string check = isCheck ? "\tCheck" : "";
			return type + "\t" + start [0] + "." + start [1] + " -> " + target [0] + "." + target [1] + capture + check;
		}
	}
}

