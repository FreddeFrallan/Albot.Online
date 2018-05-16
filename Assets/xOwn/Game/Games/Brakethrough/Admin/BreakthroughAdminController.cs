using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breakthrough;
using System;


namespace AdminUI{

	public class BreakthroughAdminController : AdminController {

		private int width = 8, height = 8;
		public BrakeThroughRenderer theRenderer;
		private List<int[,]> turns = new List<int[,]>();


		public override void init (){
			base.init ();
			theRenderer.displayBoard (generateStartBoard ());
		}


		public override void initLog (string[] logHistory){
			turns.Clear ();
			turns.Add (generateStartBoard ());
			addLogMove (logHistory);
		}

		public override void addLogMove (string[] logMsg){
			foreach (string s in logMsg)
				turns.Add(playMove (s, playerTurn ()));
			displayLatestBoard ();
		}

		private void displayLatestBoard(){
			theRenderer.displayBoard (flipBoard(turns [turns.Count - 1]));
		}

		private int[,] playMove(string move, int player){
			int[,] newBoard = (int[,])turns [turns.Count - 1].Clone();
			int[] start = parseMove(move.Substring(0, 2));
			int[] target = parseMove(move.Substring(2, 2));

			newBoard[start[0], start[1]] = 0;
			newBoard [target [0], target [1]] = player;
			return newBoard;
		}


		private int[] parseMove(string m){
			int[] move = new int[2];
			move [0] = (int)m [0] - (int)'A';
			move [1] = height - int.Parse (m [1].ToString ());
			return move;
		}


		private int[,] parseBoard(string s){
			int[,] tempBoard = new int[width, height];
			string[] words = s.Split (' ');
			iterateBoard ((x, y) => {
				tempBoard[x, y] =  int.Parse(words[y*height + x]);
			});
			return tempBoard;
		}


		private int[,] generateStartBoard(){
			int[,] tempBoard = new int[width, height];
			iterateBoard ((x, y) => {
				if(y < 2)
					tempBoard[x, y] = -1;
				else if(y >= height-2)
					tempBoard[x, y] = 1;
				else
					tempBoard[x, y] = 0;
			});
			return tempBoard;
		}




		private int[,] flipBoard(int[,] b){
			int[,] tempBoard = new int[width, height];
			iterateBoard ((x, y) => {
				tempBoard[x, height-y-1] = b[x, y];
			});
			return tempBoard;
		}

		private void iterateBoard(Action<int, int> a){
			for (int y = 0; y < height; y++)
				for (int x = 0; x < width; x++)
					a (x, y);
		}

		private int playerTurn(){return turns.Count % 2 == 1 ? 1 : -1;}
	}
}