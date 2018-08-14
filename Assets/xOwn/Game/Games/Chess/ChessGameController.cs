using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using AlbotServer;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using ClientUI;

namespace Chess{
	public class ChessGameController :  Game.ClientController{

		private ChessRenderer localRenderer;
		private Game.PlayerColor myColor;
		protected CommProtocol protocol;

		public override Game.GameType getGameType (){return Game.GameType.Chess;}
		public override void initProtocol (Game.CommProtocol protocol){this.protocol = (CommProtocol)protocol;}
        protected override void initHandlers (){
			connectionToServer.RegisterHandler ((short)CommProtocol.MsgType.playerInit, handleInitSettings);
			connectionToServer.RegisterHandler ((short)CommProtocol.MsgType.gameInfo, handleGameStatus);
			connectionToServer.RegisterHandler ((short)CommProtocol.MsgType.boardUpdate, requestMove);
			connectionToServer.RegisterHandler ((short)ServerCommProtocl.PlayerJoinedGameRoom, handlePlayerJoinedRoom);
			connectionToServer.RegisterHandler ((short)ServerCommProtocl.PlayerLeftGameRoom, handlePlayerLeftRoom);
			connectionToServer.RegisterHandler ((short)CommProtocol.MsgType.RPCMove, handleRPCMove);
			StartCoroutine (findAndInitLocalGameController ());
			StartCoroutine (handleNetworkMsgQueue ());
		}
			

		IEnumerator findAndInitLocalGameController(){
			while (localRenderer == null) {
				GameObject foundObj = GameObject.FindGameObjectWithTag ("GameController");
				if (foundObj != null) {
					localRenderer = foundObj.GetComponent<ChessRenderer> ();	
					localGameUI = foundObj.GetComponent<GameUI> ();	
				}

				yield return new WaitForEndOfFrame ();
			}
		}


		//InProgress
		public override void onOutgoingLocalMsg (string msg, Game.PlayerColor color){
			sendServerMsg(msg, color, (short)CommProtocol.MsgType.move);
		}

	
		public void handleRPCMove(NetworkMessage RPCMsg){
			byte[] bytes = RPCMsg.reader.ReadBytesAndSize ();
			RPCMove msg = Game.ClientController.Deserialize<RPCMove> (bytes);
			int[] start = msg.start;
			int[] target = msg.target;
			localRenderer.makeMove (start, target);
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
			string msg = Game.ClientController.Deserialize<Game.CommProtocol.StringMessage> (bytes).msg;
			TCPLocalConnection.sendMessage (msg);
		}
		public void handleInitSettings(NetworkMessage initMsg){
			byte[] bytes = initMsg.reader.ReadBytesAndSize ();
			myColor = Game.ClientController.Deserialize<GameInfo> (bytes).myColor;
			isListeningForTCP = true;
		}
		public void handleGameStatus(NetworkMessage gameStatusMsg){
			byte[] bytes = gameStatusMsg.reader.ReadBytesAndSize ();
			GameInfo msg = Game.ClientController.Deserialize<GameInfo> (bytes);

			if (msg.gameOver) {
                string gameOverString = TCP_API.APIStandardConstants.Fields.gameOver;
                TCPLocalConnection.sendMessage (gameOverString);

				string gameOverMsg;
				if (msg.winnerColor == Game.PlayerColor.None)
					gameOverMsg = "It's a draw!";
				else
					gameOverMsg = msg.winnerColor == myColor ? "You won!" : "You lost!";

                CurrentGame.gameOver(gameOverMsg);
            }
		}
	}

}