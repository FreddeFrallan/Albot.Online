using System.Collections;
using System.Collections.Generic;
using Barebones.Networking;
using System;

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
        public List<string> winChecks = new List<string>();

		public string[,] grid = new string[Consts.BOARD_WIDTH, Consts.BOARD_HEIGHT];
		public BoardState boardState = 0;
		public List<int> possibleMoves;

        public Board(string rawBoard, bool evaluate) {
            string[] cells = rawBoard.Trim().Split(' ');
            if (evaluate)
                generateGridAndWinData(cells);
            else
                Utils.iterateBoard((x, y) => { grid[x, y] = cells[y * Consts.BOARD_WIDTH + x]; });
        }


        public string encodeBoard(bool board = true, bool sendPMoves = false, bool evaluated = false){
			JSONObject jBoard = new JSONObject ();

			if (board)
				jBoard.AddField (Consts.Fields.board, ToString ());
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


        private void generateGridAndWinData(string[] cells) {
            string[] rows = new string[] { "", "", "", "", "", "" };
            string[] cols = new string[] { "", "", "", "", "", "", "" };
            string[] lDiags = new string[6] { "", "", "", "", "", "" };
            string[] rDiags = new string[6] { "", "", "", "", "", "" };
            Utils.iterateBoard((x, y) => {
                string cell = cells[y * Consts.BOARD_WIDTH + x];
                cell = (cell == "-1" ? "2" : cell); //Switch "-1" to a "2"

                grid[x, y] = cell;
                rows[y] += cell;
                cols[x] += cell;

                int diff = x - y;
                int sum = x + y;
                if (diff >= -2 && diff <= 3)
                    lDiags[diff + 2] += cell;
                if (sum >= 3 && sum <= 8)
                    rDiags[sum - 3] += cell;
            });

            winChecks.AddRange(rows);
            winChecks.AddRange(cols);
            winChecks.AddRange(lDiags);
            winChecks.AddRange(rDiags);
        }

        public string getWinChecks() {
            string temp = "";
            for(int i = 0; i < winChecks.Count; i++)
                temp += i + ":  " + winChecks[i] + "\n";
            return temp;
        }

        public override string ToString() {
            string s = "";
            Utils.iterateBoard((x, y) => { s += grid[x, y] + " "; });
            return s.TrimEnd();
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