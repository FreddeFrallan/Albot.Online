using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using ConnectFour;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Barebones.MasterServer;
using AlbotServer;
using Game;
using TCP_API;
using TCP_API.Connect4;
using ClientUI;

namespace Connect4{

	public class Connect4ClientController : Game.ClientController {

		//Ingame Unity Components that we are going to talk to.
		private Renderer localRenderer;
        private Connect4HumanControlls humanControlls;

        #region override from base clientController
        public override void initProtocol (Game.CommProtocol protocol){this.protocol = (Connect4.CommProtocol)protocol;}
		public override Game.GameType getGameType (){return Game.GameType.Connect4;}
		protected CommProtocol protocol;
        protected override void initHandlers (){
            apiRouter = new Connect4APIRouter();

            connectionToServer.RegisterHandler ((short)CommProtocol.MsgType.boardUpdate, requestMove);
			connectionToServer.RegisterHandler ((short)CommProtocol.MsgType.playerInit, initSettings);
			connectionToServer.RegisterHandler ((short)CommProtocol.MsgType.RPCMove, handleRPCMove);
			connectionToServer.RegisterHandler ((short)CommProtocol.MsgType.gameInfo, handleGameStatus);
			connectionToServer.RegisterHandler ((short)ServerCommProtocl.PlayerJoinedGameRoom, handlePlayerJoinedRoom);
			connectionToServer.RegisterHandler ((short)ServerCommProtocl.PlayerLeftGameRoom, handlePlayerLeftRoom);
			connectionToServer.RegisterHandler ((short)ServerCommProtocl.PlayerTimerInit, handlePlayerTimerInit);
			connectionToServer.RegisterHandler ((short)ServerCommProtocl.PlayerTimerCommand, handlePlayerTimerCommand);
			StartCoroutine (findAndInitRenderer<Renderer>((x) => localRenderer = x));
			StartCoroutine (handleNetworkMsgQueue ());

            //Quick hack
            humanControlls = localRenderer.gameObject.GetComponent<Connect4HumanControlls>();
            humanControlls.init((move) => {
                onOutgoingLocalMsg(move.ToString(), ClientPlayersHandler.sendFromCurrentPlayer());
                humanControlls.endPlayerTurn();
            });
            if (ClientPlayersHandler.hasLocalHumanPlayer())
                ClientPlayersHandler.getLocalHumanPlayer().onHumanTakeInput = () => {
                    humanControlls.startPlayerTurn();
                };

            TCPMessageQueue.readMsgInstant = ((msg) => {
				TCPMessageQueue.popMessage();
				readTCPMsg(msg);
			});
		}
		#endregion

		public override void onOutgoingLocalMsg (string msg, Game.PlayerColor color){
			sendServerMsg(msg, color, (short)CommProtocol.MsgType.move);
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

		#region Bunch of GameLogic communication handlers
		public void requestMove(NetworkMessage boardMsg){
			byte[] bytes = boardMsg.reader.ReadBytesAndSize ();
			CommProtocol.StringMessage msg = Deserialize<Game.CommProtocol.StringMessage> (bytes);

			string formattedBoard = formatBoard (msg.msg, msg.color);
			ClientPlayersHandler.onReceiveServerMsg (formattedBoard, msg.color);
		}
		public void initSettings(NetworkMessage initMsg){
			byte[] bytes = initMsg.reader.ReadBytesAndSize ();
			GameInfo msg = Deserialize<GameInfo> (bytes);

			ClientPlayersHandler.initPlayerColor (msg.username, msg.myColor);
            isListeningForTCP = true;
		}
		public void handleRPCMove(NetworkMessage RPCMsg){
			byte[] bytes = RPCMsg.reader.ReadBytesAndSize ();
			RPCMove msg = Deserialize<RPCMove> (bytes);
			Game.PlayerColor color = msg.color;
			int move = msg.move;

            localRenderer.dropPiece (move, color == Game.PlayerColor.Yellow ? Piece.Yellow : Piece.Red);
		}
		public void handleGameStatus(NetworkMessage gameStatusMsg){
			byte[] bytes = gameStatusMsg.reader.ReadBytesAndSize ();
			GameInfo msg = Deserialize<GameInfo> (bytes);
			
			if (msg.gameOver) {
                string gameOverString = TCP_API.APIStandardConstants.Fields.gameOver;
                string winner = "0";
                if (msg.winnerColor == PlayerColor.Yellow)
                    winner = "1";
                else if (msg.winnerColor == PlayerColor.Red)
                    winner = "-1";
				TCPLocalConnection.sendMessage (gameOverString + ": " + winner);
                localRenderer.onGameOver (msg.winnerColor == PlayerColor.Yellow ? Piece.Yellow : Piece.Red);
				//gameOver ();

				string gameOverMsg;
				if (msg.winnerColor == Game.PlayerColor.None)
					gameOverMsg = "It's a draw!";
				else
					//gameOverMsg = msg.winnerColor == myColor ? "You won!" : "You lost!";
					gameOverMsg = msg.winnerColor + " won";

                CurrentGame.gameOver(gameOverMsg);
            }
		}
		#endregion
		// In my old variation the server only sent the board as 0 = empty, 1 = yellow, 2 = red
		// So here i do a conversion to the local player to get  0 = empty, 1 = you, -1 = enemy
		public string formatBoard(string rawBoard, Game.PlayerColor color){
			string tempBoard = "";
			
			if (color == Game.PlayerColor.Red)
				tempBoard = rawBoard.Replace ("2", "-1");
			else {
				tempBoard = rawBoard.Replace ("1", "-1");
				tempBoard = tempBoard.Replace ("2", "1");
			}

			string rotatedBoard = rotateBoard(tempBoard);
			JSONObject board = new JSONObject ();
			board.AddField ("board", rotatedBoard);
			return board.Print ();
		}

		private string rotateBoard(string rawBoard){
			string s = "";
			string[] words = rawBoard.Split (' ');
			for (int x = 0; x < 6; x++)
				for (int y = 0; y < 7; y++)
					s += words [y * 6 + x] + " ";
			return s.TrimEnd();
		}
	}
	
}