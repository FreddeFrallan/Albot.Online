using System;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace OthelloTrainingBot{
	public class MainClass{

		private static int searchDepth = 0;
		private static bool useWeights = true;

		public static void initSettings(int level){
			searchDepth = level;
			BoardLogic.initUseWeights (useWeights);
		}

		public static string playMove(string input){
			int[,] board = parseBoard(input);
			int[] move = MinMaxSearch.findBestMove(board, searchDepth);
			return (move[0] + move[1] * 8).ToString();
		}

		private static int[,] parseBoard(string board){
			string[] words = board.Split (' ');
			int[,] field = new int[8, 8];
			for (int y = 0; y < 8; y++)
				for (int x = 0; x < 8; x++) 
					field [x, y] = int.Parse (words [x + 8 * y]);

			return field;
		}
	}
}
