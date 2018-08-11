using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using UnityEngine.Networking;
using AlbotServer;
using ClientUI;


namespace Bomberman{

	public class BombermanClientController : ClientController {

		protected BombermanProtocol protocol;
		private BombermanRenderer localRenderer;
		private bool isGameOver = false, gotInitMsg = false;
		private int lastUpdateID = 0;
		private PlayerColor localPlayerColor = PlayerColor.None, localHumanColor = PlayerColor.None;


		public override void initProtocol (CommProtocol protocol){this.protocol = (BombermanProtocol)protocol;}
		public override void onOutgoingLocalMsg (string msg, PlayerColor color){}
		public override GameType getGameType (){return GameType.Bomberman;}
        protected override void initHandlers (){
			connectionToServer.RegisterHandler ((short)ServerCommProtocl.PlayerJoinedGameRoom, handlePlayerJoinedRoom);
			connectionToServer.RegisterHandler ((short)ServerCommProtocl.PlayerLeftGameRoom, handlePlayerLeftRoom);
			connectionToServer.RegisterHandler ((short)BombermanProtocol.MsgType.playerInit, handleInitSettings);
			connectionToServer.RegisterHandler ((short)BombermanProtocol.MsgType.gameInfo, handleGameInfoMsg);
			connectionToServer.RegisterHandler ((short)BombermanProtocol.MsgType.boardInit, handleBoardInit);
			connectionToServer.RegisterHandler ((short)BombermanProtocol.MsgType.boardUpdate, handleBoardUpdate);

			StartCoroutine (findAndInitRenderer<BombermanRenderer>((x) => localRenderer = x));
			StartCoroutine (handleNetworkMsgQueue ());
			RealtimeTCPController.resetController ();
		}

		void Update(){
			if (localHumanColor == PlayerColor.None)
				return;
			
			PlayerCommand msg = new PlayerCommand (localHumanColor, -1);
			if (Input.GetKey (KeyCode.RightArrow))msg.moveDir = 0;
			else if (Input.GetKey (KeyCode.UpArrow))msg.moveDir = 1;
			else if (Input.GetKey (KeyCode.LeftArrow))msg.moveDir = 2;
			else if (Input.GetKey (KeyCode.DownArrow))msg.moveDir = 3;
			if (Input.GetKeyDown (KeyCode.Space))
				msg.dropBomb = true;

			if (msg.moveDir >= 0 || msg.dropBomb) {
				sendServerMsg (msg, (short)BombermanProtocol.MsgType.playerCommands);
			}
		}

		protected override void readTCPMsg (ReceivedLocalMessage msg){
			if (isGameOver)
				return;
			if (msg.message.Length == 0) {
				RealtimeTCPController.requestBoard (BombermanOverlord.convertColorToTeam(localPlayerColor), true);
				return;
			}

			PlayerCommand outMsg = new PlayerCommand (localPlayerColor, -1);
			parseInputMoveDir (msg.message, ref outMsg);
			parseDropBomb(msg.message, ref outMsg);
			if(outMsg.moveDir >= 0 || outMsg.dropBomb)
				sendServerMsg(outMsg, (short)BombermanProtocol.MsgType.playerCommands);


			RealtimeTCPController.requestBoard (BombermanOverlord.convertColorToTeam(localPlayerColor), true);
		}

		private void parseInputMoveDir(string input, ref PlayerCommand outMsg){
			int moveDir = -1;
			int.TryParse (input [0].ToString (), out moveDir);
			outMsg.moveDir = moveDir;
		}
		private void parseDropBomb(string input, ref PlayerCommand outMsg){
			char bombChar = input.Length == 1 ? input [0] : input [1];
			outMsg.dropBomb = bombChar.ToString ().ToUpper () == "B";
		}



		public void handleBoardInit(NetworkMessage initMsg){
			if (gotInitMsg)
				return;
			gotInitMsg = true;
			byte[] bytes = initMsg.reader.ReadBytesAndSize ();
			localRenderer.initBoard (Deserialize<BoardUpdate> (bytes));
		}
		public void handleBoardUpdate(NetworkMessage boardMsg){
			byte[] bytes = boardMsg.reader.ReadBytesAndSize ();
			BoardUpdate msg = Deserialize<BoardUpdate> (bytes);
			if (msg.updateID < lastUpdateID)
				return;

			if (msg.updateID > lastUpdateID) {
				lastUpdateID = msg.updateID;
				localRenderer.renderNewBoard (msg);
			}

			//print ("Update : " + msg.color);

			RealtimeTCPController.gotNewBoard (BombermanOverlord.convertColorToTeam(msg.color), BombermanBoardParser.parseBoard(msg));
		}



		public void handleGameInfoMsg(NetworkMessage initMsg){
			GameInfo msg = Deserialize<GameInfo> (initMsg.reader.ReadBytesAndSize ());

			if (msg.gameOver) {
				isGameOver = true;
				canSendServerMsg = false;
				isListeningForTCP = false;


				UnetRoomConnector.shutdownCurrentConnection ();

                string gameOverString = TCP_API.APIStandardConstants.Fields.gameOver;
				string gameOverMsg;
				Debug.LogError ("Winner: " + msg.winnerColor);
				if (msg.winnerColor == Game.PlayerColor.None) {
					gameOverMsg = "It's a draw!";
					TCPLocalConnection.sendMessage (gameOverString + ": 0");
				}
				else {
					gameOverMsg = msg.winnerColor + " won";
					TCPLocalConnection.sendMessage (gameOverString + ": " + (msg.winnerColor == PlayerColor.Blue ? "1" : "-1"));
				}


                AlbotDialogBox.setGameOver();
                AlbotDialogBox.activateButton(() => { ClientUIStateManager.requestGotoState(ClientUIStates.GameLobby); }, DialogBoxType.GameState, gameOverMsg, "Return to lobby", 70, 25);
            }
		}


		public void handleInitSettings(NetworkMessage initMsg){
			GameInfo msg = Deserialize<GameInfo> (initMsg.reader.ReadBytesAndSize ());
			ClientPlayersHandler.initPlayerColor (msg.username, msg.myColor);
			LocalPlayer p = ClientPlayersHandler.getPlayerFromColor (msg.myColor);
			RealtimeTCPController.registerLocalPlayer (BombermanOverlord.convertColorToTeam (p.info.color), p.getTakeInputFunc (), !p.isNPC ());
		
			if (p.isMainPlayer())
				localPlayerColor = p.info.color;
			if(p.Human)
				localHumanColor = p.info.color;

			print (localPlayerColor);

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

	}
}