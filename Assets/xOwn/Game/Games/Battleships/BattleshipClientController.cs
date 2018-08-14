using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using AlbotServer;
using UnityEngine.Networking;
using ClientUI;

namespace Battleship{

	public class BattleshipClientController :  Game.ClientController  {
		public BattleshipsRenderer localRenderer;
		protected BattleshipProtocol protocol;


		public override void initProtocol (Game.CommProtocol protocol){this.protocol = (Battleship.BattleshipProtocol)protocol;}
        protected override void initHandlers (){
			connectionToServer.RegisterHandler ((short)BattleshipProtocol.MsgType.playerInit, handleInitSettings);
			connectionToServer.RegisterHandler ((short)BattleshipProtocol.MsgType.gameInfo, handleGameStatus);
			connectionToServer.RegisterHandler ((short)BattleshipProtocol.MsgType.targetBoardUpdate, requestMove);
			connectionToServer.RegisterHandler ((short)ServerCommProtocl.PlayerJoinedGameRoom, handlePlayerJoinedRoom);
			connectionToServer.RegisterHandler ((short)ServerCommProtocl.PlayerLeftGameRoom, handlePlayerLeftRoom);
			connectionToServer.RegisterHandler ((short)BattleshipProtocol.MsgType.RPCMove, handleRPCMove);
			connectionToServer.RegisterHandler ((short)BattleshipProtocol.MsgType.boardInit, handleBoardInit);
			connectionToServer.RegisterHandler ((short)BattleshipProtocol.MsgType.boardInitRequest, handleBoardInitRequest);
			connectionToServer.RegisterHandler ((short)ServerCommProtocl.PlayerTimerInit, handlePlayerTimerInit);
			connectionToServer.RegisterHandler ((short)ServerCommProtocl.PlayerTimerCommand, handlePlayerTimerCommand);
			StartCoroutine (findAndInitRenderer<BattleshipsRenderer>((x) => localRenderer = x));
			StartCoroutine (handleNetworkMsgQueue ());
		}
		public override Game.GameType getGameType (){return Game.GameType.Battleship;}



		//InProgress
		public override void onOutgoingLocalMsg (string msg, Game.PlayerColor color){
			sendServerMsg(msg, color, (short)BattleshipProtocol.MsgType.move);
		}



		#region comm handlers
		public void handleBoardInit(NetworkMessage initMsg){
			byte[] bytes = initMsg.reader.ReadBytesAndSize ();
			BoardMsg msg = Game.ClientController.Deserialize<BoardMsg> (bytes);
			localRenderer.initBoard (msg.board, msg.color);
		}
		public void handleBoardInitRequest(NetworkMessage initMsg){
			byte[] bytes = initMsg.reader.ReadBytesAndSize ();
			Game.CommProtocol.StringMessage msg = Game.ClientController.Deserialize<Game.CommProtocol.StringMessage> (bytes);
			Game.ClientPlayersHandler.onReceiveServerMsg (msg.msg, msg.color);
		}

		public void handleRPCMove(NetworkMessage RPCMsg){
			byte[] bytes = RPCMsg.reader.ReadBytesAndSize ();
			RPCMove msg = Game.ClientController.Deserialize<RPCMove> (bytes);
			localRenderer.playMove(msg.move, msg.color, msg.targetStatus, msg.targetType, msg.startPos, msg.horizontal);
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
			BoardMsg msg = Game.ClientController.Deserialize<BoardMsg> (bytes);
			Game.ClientPlayersHandler.onReceiveServerMsg (formatBoard(msg.board), msg.color);
		}
		public void handleInitSettings(NetworkMessage initMsg){
			byte[] bytes = initMsg.reader.ReadBytesAndSize ();
			InitSettings msg = Game.ClientController.Deserialize<InitSettings> (bytes);
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

                CurrentGame.gameOver(gameOverMsg);
            }
		}
		#endregion

		public string formatBoard(char[,] charBoard){
			string temp = "";
			for (int y = 0; y < 10; y++)
				for (int x = 0; x < 10; x++)
					temp += charBoard [x, y];
			return temp;
		}

	}
}
