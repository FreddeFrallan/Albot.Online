﻿using System.Collections;
using System.Collections.Generic;
using Barebones.Networking;
using System.Linq;
using TCP_API;
using UnityEngine;

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
			return new APIMsgConclusion(){msg = boardMsg, status = ResponseStatus.Success, target = MsgTarget.Player};
		}

        /// <summary>
        /// Returns a list of possible available moves, depening on the specified current board.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>APIMsgConclusion</returns>
		public static APIMsgConclusion aquirePossibleMoves(Connect4Command command){
			getPossibleMoves (command.board);
			string boardMsg = command.board.encodeBoard (false, true, command.evaluate);
			return new APIMsgConclusion(){msg = boardMsg, status = ResponseStatus.Success, target = MsgTarget.Player };
		}

        /// <summary>
        /// Returns a list of possible available moves, depening on the specified current board.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>APIMsgConclusion</returns>
        public static APIMsgConclusion evaluateBoard(Connect4Command command) {
            string boardMsg = command.board.encodeBoard(false, false, true);
            return new APIMsgConclusion() { msg = boardMsg, status = ResponseStatus.Success, target = MsgTarget.Player };
        }

        public static APIMsgConclusion makeClientMove(Connect4Command command) {return new APIMsgConclusion() { msg = command.move.ToString(), target = MsgTarget.Server, status = ResponseStatus.Success };}
		#endregion


		#region Simulator
		private static void getPossibleMoves(Board board){
			board.possibleMoves = new List<int> ();
            JSONObject topRow = board.grid.list[0];
			for (int i = 0; i < Consts.BOARD_WIDTH; i++)
				if (topRow.list[i].i == 0)
					board.possibleMoves.Add (i);
		}

		private static void playMove(Board board, int move, int player){
			int targetRow = findFreeRow (board.grid, move);
            if (targetRow == -1) {// Do nothing if illegal move
                Debug.Log("Tried to simulate illegal move: " + move);
                return;
            }
            board.grid.list[targetRow].list[move].i = player;
		}

		private static int findFreeRow(JSONObject grid, int col){
			for (int y = Consts.BOARD_HEIGHT - 1; y >= 0; y--)
				if (grid.list[y].list[col].i == 0)
					return y;
			return -1;
		}
		#endregion

	}
}