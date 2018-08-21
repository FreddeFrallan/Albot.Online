using System.Collections;
using System.Collections.Generic;
using Barebones.Networking;
using System;
using UnityEngine;

namespace TCP_API.Connect4{

    /// <summary>
    /// Constants regarding API commands and fields.
    /// </summary>
	public class Consts : APIStandardConstants{
		public static readonly int BOARD_HEIGHT = 6;
		public static readonly int BOARD_WIDTH = 7;
	}

    /// <summary>
    /// Container of the parsed information from the Json msg sent by the user.
    /// This information later specifies what actions should be taken regarding how we respond.
    /// </summary>
    public class Connect4Command : TCP_API.TCPCommand{
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
        private static List<DiagCheck> diagChecks = new List<DiagCheck>() {
            //Left->Up diags
            new DiagCheck(){x=0, y = 3, dX = 1, dY = -1},
            new DiagCheck(){x=0, y = 4, dX = 1, dY = -1},
            new DiagCheck(){x=0, y = 5, dX = 1, dY = -1},
            new DiagCheck(){x=1, y = 5, dX = 1, dY = -1},
            new DiagCheck(){x=2, y = 5, dX = 1, dY = -1},
            new DiagCheck(){x=3, y = 5, dX = 1, dY = -1},

            //Right->Up Diags
            new DiagCheck(){x=6, y = 3, dX = -1, dY = -1},
            new DiagCheck(){x=6, y = 4, dX = -1, dY = -1},
            new DiagCheck(){x=6, y = 5, dX = -1, dY = -1},
            new DiagCheck(){x=5, y = 5, dX = -1, dY = -1},
            new DiagCheck(){x=4, y = 5, dX = -1, dY = -1},
            new DiagCheck(){x=3, y = 5, dX = -1, dY = -1},
        };

        public List<string> winChecks = new List<string>();

        public JSONObject grid;
		public BoardState boardState = 0;
		public List<int> possibleMoves;

        public Board(JSONObject board, bool evaluate) {
            grid = board;
            if (evaluate)
                evaluateBoard();
        }


        public string encodeBoard(bool board = true, bool sendPMoves = false, bool evaluated = false){
			JSONObject jBoard = new JSONObject ();

			if (board)
				jBoard.AddField (Consts.Fields.board, grid.Print());
            if(evaluated)
                jBoard.AddField(Consts.Fields.boardState, boardState.ToString());
			if (sendPMoves) {
                if (possibleMoves.Count == 0) {
                    List<JSONObject> list = new List<JSONObject>(); // Ugly fix for sending empty list instead of "null"
                    jBoard.AddField(Consts.Fields.possibleMoves, new JSONObject(list.ToArray()));
                } else {
                    JSONObject list = new JSONObject();
                    foreach (int i in possibleMoves)
                        list.Add(i);

                    jBoard.AddField(Consts.Fields.possibleMoves, list);
                }
			}

            return jBoard.Print();
		}

        public void evaluateBoard() {
            long counter = 0, last = 0;
            bool draw = true;

            //Rows
            for (int y = 0; y < Consts.BOARD_HEIGHT; y++)
                for (int x = 0; x < Consts.BOARD_WIDTH; x++) {
                    BoardState s = checkNewNumber(grid.list[y].list[x].i, ref counter, ref last, ref draw);
                    if(s != BoardState.ongoing) {
                        boardState = s;
                        return;
                    }    
                }

            //Cols
            resetCounters(ref counter, ref last);
            for (int x = 0; x < Consts.BOARD_WIDTH; x++)
                for (int y = 0; y < Consts.BOARD_HEIGHT; y++) {
                    BoardState s = checkNewNumber(grid.list[y].list[x].i, ref counter, ref last, ref draw);
                    if (s != BoardState.ongoing) {
                        boardState = s;
                        return;
                    }
                }

            //Diags
            foreach(DiagCheck d in diagChecks) {
                resetCounters(ref counter, ref last);
                int x = d.x; int y = d.y;
                do {
                    BoardState s = checkNewNumber(grid.list[y].list[x].i, ref counter, ref last, ref draw);
                    if (s != BoardState.ongoing) {
                        boardState = s;
                        return;
                    }

                    x += d.dX; y += d.dY;
                } while (isInRange(x, y));
            }


            if (draw)
                boardState = BoardState.draw;
            else
                boardState = BoardState.ongoing;
        }

        private bool isInRange(int x, int y) { return x >= 0 && y >= 0 && x < Consts.BOARD_WIDTH && y < Consts.BOARD_HEIGHT; }
        private struct DiagCheck {public int x, y, dX, dY;}
        private void resetCounters(ref long counter, ref long last) { counter = 0; last = 0; }
        private BoardState checkNewNumber(long n, ref long counter, ref long last, ref bool draw) {
            if (n == last)
                counter++;
            else
                counter = 1;

            if (n == 0)
                draw = false;

            last = n;
            if(counter == 4 && last == 1)
                return BoardState.playerWon;
            if (counter == 4 && last == -1)
                return BoardState.enemyWon;
            return BoardState.ongoing;
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
    }		



}