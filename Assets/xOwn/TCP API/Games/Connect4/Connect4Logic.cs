using System.Collections;
using System.Collections.Generic;
using Barebones.Networking;
using System.Linq;
using TCP_API;

namespace TCP_API.Connect4 {

	public class APIGameLogic{

        #region API
        /// <summary>
        /// Simulates the future board, depending on the specified current board and move.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>APIMsgConclusion</returns>
        public static APIMsgConclusion simulateMove(Connect4Command command){
			playMove (command.board, command.move, command.player);

			if (command.getPossibleMoves)
				getPossibleMoves (command.board);

			string boardMsg = command.board.encodeBoard (true, command.getPossibleMoves, command.evaluate);
			return new APIMsgConclusion(){msg = boardMsg, status = ResponseStatus.Success, toServer = false};
		}

        /// <summary>
        /// Returns a list of possible available moves, depening on the specified current board.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>APIMsgConclusion</returns>
		public static APIMsgConclusion aquirePossibleMoves(Connect4Command command){
			getPossibleMoves (command.board);
			string boardMsg = command.board.encodeBoard (false, true, command.evaluate);
			return new APIMsgConclusion(){msg = boardMsg, status = ResponseStatus.Success, toServer = false};
		}

        /// <summary>
        /// Returns a list of possible available moves, depening on the specified current board.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>APIMsgConclusion</returns>
        public static APIMsgConclusion evaluateBoard(Connect4Command command) {
            command.board.winner = BoardEvaluator.evaluateBoard(command.board);
            string boardMsg = command.board.encodeBoard(false, false, true);
            return new APIMsgConclusion() { msg = boardMsg, status = ResponseStatus.Success, toServer = false };
        }

        public static APIMsgConclusion makeClientMove(Connect4Command command) {return new APIMsgConclusion() { msg = command.move.ToString(), toServer = true, status = ResponseStatus.Success };}
		#endregion


		#region Simulator
		private static void getPossibleMoves(Board Board){
			Board.possibleMoves = new List<int> ();
			for (int i = 0; i < Consts.BOARD_WIDTH; i++)
				if (Board.grid [i,0] == "0")
					Board.possibleMoves.Add (i);
		}

		private static void playMove(Board board, int move, string player){
			int targetRow = findFreeRow (board.grid, move);
			board.grid [move, targetRow] = player;
		}

		private static int findFreeRow(string[,] grid, int col){
			for (int y = Consts.BOARD_HEIGHT - 1; y >= 0; y--)
				if (grid [col, y] == "0")
					return y;
			return -1;
		}
		#endregion

	}
}