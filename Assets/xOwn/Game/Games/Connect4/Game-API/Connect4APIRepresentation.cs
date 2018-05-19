using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.Networking;
using System;

namespace TCP_API.Connect4{

    /// <summary>
    /// Constants regarding API commands and fields.
    /// </summary>
	public class Consts{
		public static readonly int BOARD_HEIGHT = 6;
		public static readonly int BOARD_WIDTH = 7;

		public static class Fields{
			public const string board = "Board";
			public const string evaluate = "Evaluate";
			public const string possibleMoves = "PossMoves";
			public const string move = "Move";
			public const string player = "Player";
		}

		public static class Actions{
			public const string simMove = "SimulateMove";
			public const string evalBoard = "EvaluateBoard";
			public const string getPossMoves = "GetPossibleMoves";
		}
	}

    /// <summary>
    /// Container of the parsed information from the Json msg sent by the user.
    /// This information later specifies what actions should be taken regarding how we respond.
    /// </summary>
    public class Connect4Command : TCPCommand{
        public Board board;
        public int move;
        public bool evaluate;
        public bool getPossibleMoves;
    }

    /// <summary>
    /// Board representation of the a Connect4 game state.
    /// 
    /// TODO Optimize
    /// </summary>
    public class Board{
		public string[,] grid = new string[Consts.BOARD_WIDTH, Consts.BOARD_HEIGHT];
		public int winner = 0;
		public List<int> possibleMoves;

		public string encodeBoard(bool board = true, bool sendPMoves = false, bool evaluated = false){
			JSONObject jBoard = new JSONObject ();

			if (board)
				jBoard.AddField (Consts.Fields.board, ToString ());
			if (sendPMoves) {
				JSONObject list = new JSONObject ();
				foreach (int i in possibleMoves)
					list.Add (i);
				
				jBoard.AddField (Consts.Fields.possibleMoves, list);
			}

			return jBoard.Print();
		}

		public override string ToString (){
			string s = "";
			Utils.iterateBoard((x, y) =>  {s += grid[x, y] + " ";} );
			return s;
		}

	}

	public class Utils{
        /// <summary>
        /// Iterates through all the possible cells in a Connec4 grid, and applies the action "a".
        /// </summary>
        /// <param name="a"></param>
		public static void iterateBoard(Action<int, int> a){
			for (int y = 0; y < Consts.BOARD_HEIGHT; y++)
				for (int x = 0; x < Consts.BOARD_WIDTH; x++)
					a (x, y);
		}

        /// <summary>
        /// Takes raw text representation of the board and returns a Board Obj.
        /// </summary>
        /// <param name="board"></param>
        /// <returns>Board</returns>
        public static Board parseBoard(string board) {
            string[] boardWords = board.Trim().Split(' ');
            Board parsedBoard = new Board();
            Utils.iterateBoard((x, y) => { parsedBoard.grid[x, y] = boardWords[y * Consts.BOARD_WIDTH + x]; });

            return parsedBoard;
        }
    }		
}