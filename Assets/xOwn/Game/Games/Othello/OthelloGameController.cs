using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using UnityEngine.Networking;
using AlbotServer;
using ClientUI;

namespace Othello{

	public class OthelloGameController : Game.ClientController {

		private OthelloBoardRenderer localRenderer;
		protected CommProtocol protocol;

		#region implemented abstract members of ClientController
		public override void initProtocol (Game.CommProtocol protocol){this.protocol = (Othello.CommProtocol)protocol;}
        protected override void initHandlers (){
			connectionToServer.RegisterHandler ((short)CommProtocol.MsgType.playerInit, handleInitSettings);
			connectionToServer.RegisterHandler ((short)CommProtocol.MsgType.gameInfo, handleGameStatus);
			connectionToServer.RegisterHandler ((short)CommProtocol.MsgType.boardUpdate, requestMove);
			connectionToServer.RegisterHandler ((short)ServerCommProtocl.PlayerJoinedGameRoom, handlePlayerJoinedRoom);
			connectionToServer.RegisterHandler ((short)ServerCommProtocl.PlayerLeftGameRoom, handlePlayerLeftRoom);
			connectionToServer.RegisterHandler ((short)CommProtocol.MsgType.RPCMove, handleRPCMove);
			connectionToServer.RegisterHandler ((short)ServerCommProtocl.PlayerTimerInit, handlePlayerTimerInit);
			connectionToServer.RegisterHandler ((short)ServerCommProtocl.PlayerTimerCommand, handlePlayerTimerCommand);
			StartCoroutine (findAndInitRenderer<OthelloBoardRenderer>((x) => localRenderer = x));
			StartCoroutine (handleNetworkMsgQueue ());

            //Hotfix for Hello World Presentation
            TCPMessageQueue.readMsgInstant = readTCPMsg;
        }
        public override Game.GameType getGameType (){return Game.GameType.Othello;}
		#endregion


        

		//InProgress
		public override void onOutgoingLocalMsg (string msg, Game.PlayerColor color){
            sendServerMsg(msg, color, (short)CommProtocol.MsgType.move);
		}



		#region comm handlers
		public void handleRPCMove(NetworkMessage RPCMsg){
			byte[] bytes = RPCMsg.reader.ReadBytesAndSize ();
			RPCMove msg = Game.ClientController.Deserialize<RPCMove> (bytes);
			Game.PlayerColor color = msg.color;
			int[] move = msg.move;

			localRenderer.playMove(move[0], move[1], color == Game.PlayerColor.White);
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
		public void requestMove(NetworkMessage boardMsg){
			byte[] bytes = boardMsg.reader.ReadBytesAndSize ();
			Game.CommProtocol.StringMessage msg = Game.ClientController.Deserialize<Game.CommProtocol.StringMessage> (bytes);
			Game.ClientPlayersHandler.onReceiveServerMsg (formatBoard(msg.msg, msg.color), msg.color);
		}
		public void handleInitSettings(NetworkMessage initMsg){
			byte[] bytes = initMsg.reader.ReadBytesAndSize ();
			GameInfo msg = Game.ClientController.Deserialize<GameInfo> (bytes);
			Game.ClientPlayersHandler.initPlayerColor (msg.username, msg.myColor);
			isListeningForTCP = true;
		}
		public void handleGameStatus(NetworkMessage gameStatusMsg){
			byte[] bytes = gameStatusMsg.reader.ReadBytesAndSize ();
			GameInfo msg = Game.ClientController.Deserialize<GameInfo> (bytes);

			if (msg.gameOver) {
                string gameOverString = TCP_API.APIStandardConstants.Fields.gameOver;
                TCPLocalConnection.sendMessage (gameOverString);
				gameOver ();

				string gameOverMsg;
				if (msg.winnerColor == Game.PlayerColor.None)
					gameOverMsg = "It's a draw!";
				else
					gameOverMsg = msg.winnerColor + " won";

				AlbotDialogBox.setGameOver ();
                AlbotDialogBox.activateButton(() => { ClientUIStateManager.requestGotoState(ClientUIStates.GameLobby); }, DialogBoxType.GameState, gameOverMsg, "Return to lobby", 70, 25);
            }
		}
		#endregion


		public string formatBoard(string rawBoard, Game.PlayerColor color){
			string tempBoard = "";

			if (color == Game.PlayerColor.White)
				tempBoard = rawBoard.Replace ("2", "-1");
			else {
				tempBoard = rawBoard.Replace ("1", "-1");
				tempBoard = tempBoard.Replace ("2", "1");
			}

			return tempBoard;
		}
	}
}