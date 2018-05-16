using System;
using System.Collections.Generic;

namespace BrakethroughBot {
	public class Util {
		public static void iterateOverBoard (Action<int, int> a) {
			for (int y = Board.ySize-1; y >= 0; y--)
				for (int x = 0; x < Board.xSize; x++)
					a(x, y);
		}


		public static void printBoard (Square[,] board) {
			for (int y = 0; y < Board.ySize; y++) {
				string s = "";
				for (int x = 0; x < Board.xSize; x++)
					s += board[x, y].playerSign + "\t";
				Console.WriteLine(s);
			}
		}

		public static void printCoord (int[] c) {Console.WriteLine(c[0] + "." + c[1]);}
		public static void writeCoord (int[] c) {Console.Write(c[0] + "." + c[1]); }

		public static void printMove (int[] c) {
			Console.WriteLine("Move: " + c[0] + "." + c[1] + "  " + c[2] + "." + c[3]);
		}


		public static void printAvailableMoves (Board b, int team) {
			List<int[]> moves = b.getAvailableMoves(team);
			foreach (int[] move in moves) 
				Console.WriteLine("Move: " + move[0] + "." + move[1] + " -> " + move[2] + "." + move[3]);
		}


		public static void printPieceMoves (Square s, int team) {
		//	Console.WriteLine(s.move[team][0] + " " + s.move[team][1] + " " + s.move[team][2]);
		}


		public static string coordToStr (int[] c) {
			return (((char)((c[0]) + (int)'A')).ToString() + (8 - c[1]));
		}
	}
}
