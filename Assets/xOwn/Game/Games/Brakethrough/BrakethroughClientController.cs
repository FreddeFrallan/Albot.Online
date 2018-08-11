using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using AlbotServer;
using Game;
using ClientUI;

namespace Breakthrough{

	public class BrakethroughClientController : ClientController {

		public override GameType getGameType (){return GameType.Breakthrough;}
		public override void initProtocol (CommProtocol protocol){this.protocol = (BrakethroughProtocol)protocol;}
		public override void onOutgoingLocalMsg (string msg, PlayerColor color){sendServerMsg(msg, color, (short)BrakethroughProtocol.MsgType.move);}


		private BrakethroughProtocol protocol;
		private BrakeThroughRenderer renderer;
		private bool waitingForInput = true;
		private int currentUpdate = -1;
		private BoardMsg lastBoardMsg;

        protected override void initHandlers (){
			connectionToServer.RegisterHandler ((short)BrakethroughProtocol.MsgType.playerInit, handleInitSettings);
			connectionToServer.RegisterHandler ((short)ServerCommProtocl.PlayerJoinedGameRoom, handlePlayerJoinedRoom);
			connectionToServer.RegisterHandler ((short)BrakethroughProtocol.MsgType.boardUpdate, handleBoardUpdate);
			connectionToServer.RegisterHandler ((short)BrakethroughProtocol.MsgType.moveRequest, handleMoveRequest);
			connectionToServer.RegisterHandler ((short)BrakethroughProtocol.MsgType.gameInfo, handleGameStatus);
			StartCoroutine (findAndInitRenderer<BrakeThroughRenderer>((x) => renderer = x));
			StartCoroutine (handleNetworkMsgQueue ());
		}


		#region comm handlers
		public void handleBoardUpdate(NetworkMessage updateMsg){
			byte[] bytes = updateMsg.reader.ReadBytesAndSize ();
			BoardMsg msg = ClientController.Deserialize<BoardMsg> (bytes);

			if (msg.updateNumber <= currentUpdate)
				return;
			currentUpdate = msg.updateNumber;
			renderer.displayBoard (msg.board);
			renderer.startTimer (msg.updateNumber % 2 == 0);
		}

		public void handleMoveRequest(NetworkMessage updateMsg){
			byte[] bytes = updateMsg.reader.ReadBytesAndSize ();
			BoardMsg msg = ClientController.Deserialize<BoardMsg> (bytes);
			lastBoardMsg = msg;
			requestMove (msg);
		}


		protected override void readTCPMsg (ReceivedLocalMessage msg){
			if (waitingForInput == false)
				return;

			int[] start = new int[2], target = new int[2];
			if(validInput(msg.message, ref start, ref target) == false){
				requestMove(lastBoardMsg);
				return;
			}

			if (lastBoardMsg.updateNumber % 2 != 0)
				ClientUtil.rotateCoords (lastBoardMsg.board.GetLength(1), ref start, ref target);

			string outString = ClientUtil.moveToString (start, target);
			onOutgoingLocalMsg (outString, lastBoardMsg.updateNumber % 2 == 0 ? PlayerColor.White : PlayerColor.Black);
			waitingForInput = false;
		}
			
			
		public void requestMove(BoardMsg msg){
			PlayerColor requestColor = msg.updateNumber % 2 == 0 ? PlayerColor.White : PlayerColor.Black;
			if (ClientPlayersHandler.hasLocalPlayerOfColor (requestColor)) {
				waitingForInput = true;
				ClientPlayersHandler.onReceiveServerMsg (formatBoard (msg.requestMoveColor, msg.board), msg.requestMoveColor);
			} 
			else
				waitingForInput = false;
		}

		public void handleInitSettings(NetworkMessage initMsg){
			byte[] bytes = initMsg.reader.ReadBytesAndSize ();
			GameInfo msg = ClientController.Deserialize<GameInfo> (bytes);
			ClientPlayersHandler.initPlayerColor (msg.username, msg.myColor);
			isListeningForTCP = true;
		}
		public void handleGameStatus(NetworkMessage gameStatusMsg){
			byte[] bytes = gameStatusMsg.reader.ReadBytesAndSize ();
			GameInfo msg = ClientController.Deserialize<GameInfo> (bytes);

			if (msg.gameOver) {
                string gameOverString = TCP_API.APIStandardConstants.Fields.gameOver;
                TCPLocalConnection.sendMessage (gameOverString);
				gameOver ();
				waitingForInput = false;

				string gameOverMsg;
				if (msg.winnerColor == PlayerColor.None)
					gameOverMsg = "It's a draw!";
				else
					gameOverMsg = msg.winnerColor + " won";

				AlbotDialogBox.setGameOver ();
                AlbotDialogBox.activateButton(() => { ClientUIStateManager.requestGotoState(ClientUIStates.GameLobby); }, DialogBoxType.GameState, gameOverMsg, "Return to lobby", 70, 25);
            }
		}


		public void handlePlayerLeftRoom(NetworkMessage msg){
			PlayerInfoMsg readyMsg = msg.ReadMessage<PlayerInfoMsg> ();
			PlayerInfo p = readyMsg.player;
			localGameUI.removeConnectedPlayer (p.color, p.username, p.iconNumber);
		}
		public void handlePlayerJoinedRoom(NetworkMessage msg){
			PlayerInfoMsg readyMsg = msg.ReadMessage<PlayerInfoMsg> ();
			PlayerInfo p = readyMsg.player;
			localGameUI.initPlayerSlot (p.color, p.username, p.iconNumber);
		}

		private string formatBoard(PlayerColor color, int[,] board){
			string msg = "";

			if(color == PlayerColor.Black)
				for (int y = 0; y < board.GetLength (1); y++)
					for (int x = 0; x < board.GetLength (0); x++)
						msg += (board[x, y] * -1).ToString() + " ";
			else
				for (int y = board.GetLength (1)-1; y >= 0 ; y--)
					for (int x = 0; x < board.GetLength (0); x++)
						msg += board[x, y].ToString() + " ";
			
				
			return msg;
		}
		#endregion


		#region input check
		private bool validInput(string msg, ref int[] start, ref int[] target){
			if (msg.Length < 4)
				return false;

			msg = msg.ToUpper ();	
			string startMsg = msg.Substring (0, 2);
			string targetMsg = msg.Substring (2, 2);
			if (ClientUtil.stringToCoord (startMsg, ref start) == false || ClientUtil.stringToCoord (targetMsg, ref target) == false)
				return false;

			return true;
		}

		#endregion

	}

	public class ClientUtil{

		public static bool stringToCoord(string msg, ref int[] coord){
			if (char.IsLetter (msg [0]) == false || char.IsDigit(msg[1]) == false)
				return false;
			coord = new int[]{ (int)msg [0] - (int)'A', int.Parse (msg [1].ToString ())};	
			return true;
		}

		public static string moveToString(int[] start, int[] target){return coordToString (start) + coordToString (target);}
		public static string coordToString(int[] c){return ((char)(c [0] + (int)'A')).ToString() + c [1].ToString();}

		public static void rotateCoords(int boardHeight, ref int[] start, ref int[] target){
			start[1] = boardHeight - start [1]  + 1;
			target[1] = boardHeight - target [1] + 1;
		}
	}
}