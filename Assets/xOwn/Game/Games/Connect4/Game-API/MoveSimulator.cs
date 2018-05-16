using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Barebones.Networking;
using System.Linq;

namespace Connect4_TCP_API{

	public class APIGameLogic{


		public static MessageConclusion simulateMove(Command command){
			playMove (command.board, command.move, command.player);

			if (command.getPossibleMoves)
				getPossibleMoves (command.board);

			string boardMsg = command.board.encodeBoard (true, command.getPossibleMoves, command.evaluate);
			return new MessageConclusion(){msg = boardMsg, status = ResponseStatus.Success, toServer = false};
		}

		public static MessageConclusion aquirePossibleMoves(Command command){
			getPossibleMoves (command.board);
			string boardMsg = command.board.encodeBoard (false, true, command.evaluate);
			return new MessageConclusion(){msg = boardMsg, status = ResponseStatus.Success, toServer = false};
		}


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