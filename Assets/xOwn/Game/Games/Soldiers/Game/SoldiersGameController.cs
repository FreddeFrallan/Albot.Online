using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using UnityEngine.Networking;
using AlbotServer;

namespace Soldiers{

	public class SoldiersGameController : ClientController {


		protected SoldiersProtocol protocol;
		private SoldiersRenderer localRenderer;
		private uint lastUpdateId = 0;
		private PlayerColor localPlayerColor = PlayerColor.None;
		private bool rendererInit = false;
		private bool isGameOver = false;


		public override void initProtocol (CommProtocol protocol){this.protocol = (SoldiersProtocol)protocol;}
		public override void onOutgoingLocalMsg (string msg, PlayerColor color){}
		public override GameType getGameType (){return GameType.Soldiers;}


		//Since we assume that there is only one bot from the local player we override
		protected override void readTCPMsg (ReceivedLocalMessage msg){
			if (isGameOver)
				return;
			
			List<TCPCommand> playerCommands = SoldiersTCPProtocol.convertMsgToCommands (msg.message);
			if (playerCommands.Count == 0) {
				RealtimeTCPController.requestBoard (convertColorToTeam(localPlayerColor), true);
				return;
			}

			PlayerCommands outMsg = new PlayerCommands (localPlayerColor, playerCommands);
			sendServerMsg(outMsg, (short)SoldiersProtocol.MsgType.playerCommands);
			RealtimeTCPController.requestBoard (0, true);
		}

		protected override void initHandlers (){
			connectionToServer.RegisterHandler ((short)SoldiersProtocol.MsgType.playerInit, handleInitSettings);
			connectionToServer.RegisterHandler ((short)ServerCommProtocl.PlayerJoinedGameRoom, handlePlayerJoinedRoom);
			connectionToServer.RegisterHandler ((short)ServerCommProtocl.PlayerLeftGameRoom, handlePlayerLeftRoom);
			connectionToServer.RegisterHandler ((short)SoldiersProtocol.MsgType.boardUpdate, handleBoardUpdate);
			connectionToServer.RegisterHandler ((short)SoldiersProtocol.MsgType.boardStart, handleBoardStart);
			connectionToServer.RegisterHandler ((short)SoldiersProtocol.MsgType.gameInfo, handleGameInfo);
			StartCoroutine (findAndInitRenderer<SoldiersRenderer>((x) => localRenderer = x));
			StartCoroutine (handleNetworkMsgQueue ());
			RealtimeTCPController.resetController ();
		}


		public void handleBoardStart(NetworkMessage initMsg){
			byte[] bytes = initMsg.reader.ReadBytesAndSize ();
			BoardUpdate msg = Game.ClientController.Deserialize<BoardUpdate> (bytes);
			SoldiersBoardCompresser.compressBoardUpdate (msg);

			if(rendererInit == false)
				localRenderer.createNewUnits (msg.units, msg.teams, msg.ids, msg.time);
			rendererInit = true;
		}

		public void handleBoardUpdate(NetworkMessage initMsg){
			byte[] bytes = initMsg.reader.ReadBytesAndSize ();
			BoardUpdate msg = Game.ClientController.Deserialize<BoardUpdate> (bytes);
			if (msg.updateId < lastUpdateId)
				return;

			if (msg.updateId > lastUpdateId) {
				lastUpdateId = msg.updateId;
				localRenderer.addBoardUpdate (msg.units, msg.teams, msg.ids, msg.time, msg.deaths);
			}
			SoldiersBoardCompresser.compressBoardUpdate (msg);
		}



		public void handleInitSettings(NetworkMessage initMsg){
			byte[] bytes = initMsg.reader.ReadBytesAndSize ();
			GameInfo msg = ClientController.Deserialize<GameInfo> (bytes);
			ClientPlayersHandler.initPlayerColor (msg.username, msg.myColor);
			LocalPlayer p = ClientPlayersHandler.getPlayerFromColor (msg.myColor);
			RealtimeTCPController.registerLocalPlayer (convertColorToTeam (p.info.color), p.getTakeInputFunc (), !p.isNPC ());

			if (p.isNPC () == false)
				localPlayerColor = p.info.color;

			isListeningForTCP = true;
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

		public void handleGameInfo(NetworkMessage msg){
			byte[] bytes = msg.reader.ReadBytesAndSize ();
			GameInfo infoMsg = Game.ClientController.Deserialize<GameInfo> (bytes);;

			if (infoMsg.gameOver) {
				TCPLocalConnection.sendMessage ("GameOver");
				isGameOver = true;
				UnetRoomConnector.shutdownCurrentConnection ();

				string gameOverMsg;
				if (infoMsg.winnerColor == Game.PlayerColor.None)
					gameOverMsg = "It's a draw!";
				else
					gameOverMsg = infoMsg.winnerColor + " won";

				ClientUI.AlbotDialogBox.setGameOver ();
				ClientUI.AlbotDialogBox.activateButton (ClientUI.ClientUIStateManager.requestGotoGameLobby,  ClientUI.DialogBoxType.GameState, gameOverMsg, "Return to lobby", 70, 25);
			}
		}


		public static int convertColorToTeam(Game.PlayerColor color){
			return color == PlayerColor.Blue ? 1 : 2;
		}
	}
}