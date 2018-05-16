using System;
using System.Collections.Generic;

namespace BrakethroughBot {
	public class HashScores {

		private static Dictionary<string, int> hashScores = new Dictionary<string, int>();

		public static bool boardExists (Board b, ref int score) {
			string hash = hashBoard(b);
			if (hashScores.ContainsKey(hash)) {
				score = hashScores[hash];
				return true;
			}
			return false;
		}

		public static void storeBoard (Board b, int score) {hashScores.Add(hashBoard(b), score);}
		public static void clearHashes () {hashScores.Clear();}
		private static string hashBoard (Board b) {return hashTeam(b.white) + "." + hashTeam(b.black);}

		private static string hashTeam (List<int[]> team) {
			string s = "";
			foreach (int[] p in team)
				s += p[0].ToString() + p[1].ToString();
			return s;
		}

	}
}
