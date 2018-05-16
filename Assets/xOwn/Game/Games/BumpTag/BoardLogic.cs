using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;

namespace BumpTag {
	public class BoardLogic {
		public class IllegalMove : System.Exception { public IllegalMove() {} }
		public class IllegalPiece : System.Exception { public IllegalPiece() {} }

		private Dictionary<PlayerColor, int> colorToNumber = new Dictionary<PlayerColor, int>{
			{PlayerColor.Red, 0},
			{PlayerColor.Blue, 1},
			{PlayerColor.Green, 2},
			{PlayerColor.Yellow, 3}
		};
		public class Piece {
			public PlayerColor color;
			public int pos;
			public int id;

			public Piece(PlayerColor c, int p, int _id) {
				color = c;
				pos = p;
				id = _id;
			}
		}
		
		private const int NBR_PIECES = 4;
		private List<Piece> mPieces;


		public BoardLogic(int nbrPlayers) { 
			List<PlayerColor> colors;
			if (nbrPlayers == 2)
				colors = new List<PlayerColor>(){ PlayerColor.Red, PlayerColor.Green };
			else if (nbrPlayers == 3) 
				colors = new List<PlayerColor>(){ PlayerColor.Red, PlayerColor.Blue, PlayerColor.Green };
			else if (nbrPlayers == 4)
				colors = new List<PlayerColor>(){ PlayerColor.Red, PlayerColor.Blue, PlayerColor.Green, PlayerColor.Yellow };
			else 
				throw new System.Exception("Only 2-4 players are allowed");

			int id = 0;
			mPieces = new List<Piece>();
			foreach (PlayerColor c in colors)
				for (int i = 0; i < NBR_PIECES; ++i)
					mPieces.Add(new Piece(c, -1, id++));
		}

		public void movePiece(Piece p, int steps) {
			if (!isValidMove(p, steps))
				throw new IllegalMove();

			int nextPos = nextPosForPieceAndSteps(p, steps);
			Piece otherp = mPieces.FirstOrDefault(_p => _p.pos == nextPos);
			if (otherp != null)
				otherp.pos = -1; // Bump
			p.pos = nextPos;
		}

		public Piece getPiece(int pieceId) {
			Piece piece = mPieces.FirstOrDefault(p => p.id == pieceId);
			if (piece == null)
				throw new IllegalPiece();
			return piece;
		}

		public Piece getPiece(PlayerColor c, int localID){
			return mPieces [colorToNumber [c] * 4 + localID];
		}

		public List<Piece> getPieces(PlayerColor c) {
			return mPieces.Where(p => p.color == c).ToList();
		}

		public bool isValidMove(Piece p, int steps) {
			if (steps < 0)
				return false;

			int nextPos = nextPosForPieceAndSteps(p, steps);
			Piece otherp = mPieces.FirstOrDefault(_p => _p.pos == nextPos);
			if (otherp == null)
				return true;
			if (otherp.color != p.color)
				return true;
			return false;
		}

		public bool hasValidMove(PlayerColor c, int steps){
			List<Piece> pieces = getPieces (c);
			foreach (Piece p in pieces)
				if (isValidMove (p, steps))
					return true;
			return false;
		}

		public string boardAsString(PlayerColor c) { 
			// Create a string representation where the coordinates for the
			// given PlayerColor is always centralised around 0 (i.e. the starting
			// coordinate is always 0)
				// Deltas for the different coordinates
			int playerNumber = colorToNumber[c];
			int dstart = playerNumber* 10;
			int dend = playerNumber*4;

			List<Piece> onBoardPieces = new List<Piece>();
			foreach (Piece p in mPieces) {
				// This should now work
				if (p.pos < 0)continue;

				Piece np = new Piece(p.color, p.pos, p.id);
				if (p.pos < 40) {
					np.pos = p.pos - dstart;
					np.pos += np.pos < 0 ? 40:0; 
				} else {
					np.pos = p.pos - dend;
					np.pos += np.pos < 40 ? 16:0;
				}
				onBoardPieces.Add(np);
			}

			// Creating the string
			List<int> board = new List<int>();
			for (int i = 0; i <= 55; ++i) board.Add(0);
			foreach (Piece p in onBoardPieces) {
				if (p.pos > 55 || p.pos < 0) continue;
				board[p.pos] = (int)p.color;
			}

			string s = "";
			List<Piece> myPieces = onBoardPieces.Where(p => p.color == c).ToList();
			foreach (Piece p in myPieces) s += p.id.ToString() + " ";
			s += "\n";
			foreach (Piece p in myPieces) s += p.pos.ToString() + " ";
			s += "\n";
			foreach (int v in board) s += v.ToString() + " ";
			return s;
		}

		// No bump checking, only checks what the next position would be
		private int nextPosForPieceAndSteps(Piece p, int steps) {
			if (steps < 0) 
				throw new IllegalMove();

			int start, midp, midpp, finp;
			start = midp = midpp = finp = 0; // suppressing errors
			switch (p.color) {
				case PlayerColor.Red:    start =  0; midp = 39; midpp = 40; finp = 43; break;
				case PlayerColor.Blue:   start = 10; midp =  9; midpp = 44; finp = 47; break;
				case PlayerColor.Green:  start = 20; midp = 19; midpp = 48; finp = 51; break;
				case PlayerColor.Yellow: start = 30; midp = 29; midpp = 52; finp = 55; break;
			}
			
			int nextp = p.pos;
			if (nextp < 0) {
				nextp = start;
				--steps;
			}
			for (int i = 0; i < steps; ++i) {
				++nextp;
				if (nextp > finp) // finished
					nextp = 100;
				else if (nextp >= midpp && nextp <= finp)  // going up the middle isle
					continue;
				else if ((nextp - 1) == midp) // turn up the middle isle
					nextp = midpp;
				else // gone around a turn
					nextp = nextp % 40;
			}
			return nextp;
		}
	}

} // namespace BumpTag
