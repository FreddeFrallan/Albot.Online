using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.Networking;
using System;

namespace Connect4_TCP_API{

	public class Consts{
		public static readonly int BOARD_HEIGHT = 6;
		public static readonly int BOARD_WIDTH = 7;

		public static class Fields{
			public const string action = "Action";
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

	public class Command{
		public Board board;
		public string player;
		public int move;
		public bool evaluate;
		public bool getPossibleMoves;
		public string action;
	}

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
		public static void iterateBoard(Action<int, int> a){
			for (int y = 0; y < Consts.BOARD_HEIGHT; y++)
				for (int x = 0; x < Consts.BOARD_WIDTH; x++)
					a (x, y);
		}
	}

	public struct MessageConclusion{
		public string msg;
		public bool toServer;
		public ResponseStatus status;
	}





	public class MessageRouter{

		public static MessageConclusion handleIncomingMsg(string msg){
			if (msg.Length == 1)
				return new MessageConclusion (){ msg = msg, toServer = true, status = ResponseStatus.Success};
			
			Command albotCommand;
			try{albotCommand = parseCommand (msg.Trim());}
			catch{return new MessageConclusion (){ status = ResponseStatus.Error, msg = "Error Parsing Msg", toServer = false }; }

			return APIGameLogic.simulateMove (albotCommand);
		}
			

		#region Parsing
		private static Command parseCommand(string msg){
			JSONObject jCommand = new JSONObject (msg.Trim());
			Command albotCommand = parseBasicAlbotCommand (jCommand);
			switch (albotCommand.action) {
			case Consts.Actions.simMove: parseSimulateMove (jCommand, albotCommand);break;
			//case Consts.Actions.simMove: parseSimulateMove (jCommand, albotCommand);break;
			}


			return albotCommand;
		}

		private static Command parseBasicAlbotCommand(JSONObject jCommand){
			Command albotCommand = new Command ();
			albotCommand.action = jCommand.GetField (Consts.Fields.action).str;
			albotCommand.board = parseBoard (jCommand.GetField (Consts.Fields.board).str);

			return albotCommand;
		}


		private static void parseSimulateMove(JSONObject jCommand, Command albotCommand){
			albotCommand.evaluate = jCommand.HasField (Consts.Fields.evaluate);
			albotCommand.move = (int)jCommand.GetField (Consts.Fields.move).i;
			albotCommand.getPossibleMoves = jCommand.HasField (Consts.Fields.possibleMoves);
			albotCommand.player = jCommand.GetField (Consts.Fields.player).str;
		}

		private static Board parseBoard(string board){
			string[] boardWords = board.Trim().Split(' ');
			Board parsedBoard = new Board ();
			Utils.iterateBoard((x, y) => { parsedBoard.grid [x, y] = boardWords [y * Consts.BOARD_WIDTH + x];});

			return parsedBoard;
		}
		#endregion
	}
		
}