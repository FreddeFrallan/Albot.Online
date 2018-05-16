using System;
using System.Collections.Generic;

namespace ChessBot
{
	public class Test{
		public static void Main(){
			Board b = new Board ();
			List<Move> moves = b.getValidMoves (true);

			for (int i = 1; i < 3; i++) {
				b = b.playMove (moves [i]);
				printBoard (b);
				moves = b.getValidMoves (true);
			}

			moves = b.getValidMoves (false);
			b = b.playMove (moves [2]);
			printBoard (b);

			moves = b.getValidMoves (true);
			b = b.playMove (moves [4]);
			printBoard (b);
		}

		private static void printBoard(Board b){
			Console.WriteLine (b.ToString ());	
			Console.ReadLine ();
			Console.Clear ();
		}
	}
}

