using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using UnityEngine.Networking;
using AlbotServer;
using System;
using TCP_API;
using TCP_API.Snake;
using Barebones.Networking;

namespace Snake{

	public class SnakeClientController : ClientController {


		protected SnakeProtocol protocol;
		private SnakeRenderer localRenderer;
		private bool isGameOver = false, gotInitMsg = false;
		private uint lastUpdateID = 0;
		private PlayerColor localPlayerColor = PlayerColor.None, localHumanColor = PlayerColor.None;
		private SnakeTCPFormater[] TCPFormater = new SnakeTCPFormater[2];
        private SnakeAPIRouter APIRouter = new SnakeAPIRouter();
		private bool hasLocalBluePlayer = false, hasLocalRedPlayer = false;


		public override void initProtocol (CommProtocol protocol){this.protocol = (SnakeProtocol)protocol;}
		public override void onOutgoingLocalMsg (string msg, PlayerColor color){
			int dir = -1;
			if(parseCommandMsg(msg, out dir))
				sendServerMsg(new GameCommand(color, dir), (short)SnakeProtocol.MsgType.playerCommands);

			RealtimeTCPController.requestBoard (convertColorToTeam(color));
		}
		public override GameType getGameType (){return GameType.Snake;}


		protected override void initHandlers (){
            base.initHandlers();
			connectionToServer.RegisterHandler ((short)SnakeProtocol.MsgType.playerInit, handleInitSettings);
			connectionToServer.RegisterHandler ((short)SnakeProtocol.MsgType.boardUpdate, handleBoardUpdate);
			connectionToServer.RegisterHandler ((short)SnakeProtocol.MsgType.gameInfo, handleGameInfo);

			StartCoroutine (findAndInitRenderer<SnakeRenderer>((x) => localRenderer = x));
			StartCoroutine (handleNetworkMsgQueue ());
			RealtimeTCPController.resetController ();

            TCPMessageQueue.readMsgInstant = readTCPMsg;
		}


		public void handleInitSettings(NetworkMessage initMsg){
			GameInfo msg = Deserialize<GameInfo> (initMsg.reader.ReadBytesAndSize ());
			ClientPlayersHandler.initPlayerColor (msg.username, msg.myColor);
			LocalPlayer p = ClientPlayersHandler.getPlayerFromColor (msg.myColor);

			int playerIndex = convertColorToTeam (msg.myColor);
			TCPFormater [playerIndex] = new SnakeTCPFormater (playerIndex);
			Action<string> sendBoardFunc = p.getTakeInputFunc ();
			Action test = TCPFormater [playerIndex].newBoardSent; 


			RealtimeTCPController.registerLocalPlayer (playerIndex, 
				(string s) => {  test();
				sendBoardFunc(s);}
				, !p.isNPC () && !p.Human);

			if (p.isMainPlayer())
				localPlayerColor = p.info.color;
			if(p.Human)
				localHumanColor = p.info.color;

			if (p.info.color == PlayerColor.Blue)
				hasLocalBluePlayer = true;
			else
				hasLocalRedPlayer = true;

			isListeningForTCP = true;
		}


		void Update(){
			if (isGameOver || localHumanColor == PlayerColor.None)
				return;

			int direction = -1;
			if (Input.GetKeyDown (KeyCode.DownArrow))
				direction = 3;
			else if (Input.GetKeyDown (KeyCode.LeftArrow))
				direction = 2;
			else if (Input.GetKeyDown (KeyCode.UpArrow))
				direction = 1;
			else if (Input.GetKeyDown (KeyCode.RightArrow))
				direction = 0;


			if (direction == -1)
				return;

			GameCommand outMsg = new GameCommand (localHumanColor, (short)direction);
			sendServerMsg(outMsg, (short)SnakeProtocol.MsgType.playerCommands);
		}


		protected override void readTCPMsg (ReceivedLocalMessage msg){
            if (isGameOver)
				return;
			if (msg.message.Length == 0) {
				RealtimeTCPController.requestBoard (convertColorToTeam(localPlayerColor), true);
				return;
			}

            APIMsgConclusion conclusion = APIRouter.handleIncomingMsg(msg.message);
            if (conclusion.target == MsgTarget.Server) {
                int dir = -1;
                if (parseCommandMsg(msg.message, out dir))
                    sendServerMsg(new GameCommand(localPlayerColor, dir), (short)SnakeProtocol.MsgType.playerCommands);

                RealtimeTCPController.requestBoard(convertColorToTeam(localPlayerColor), true);
            } else if (conclusion.status == ResponseStatus.Success && conclusion.target == MsgTarget.Player)
                ClientPlayersHandler.getPlayerFromColor(localPlayerColor).takeInput(conclusion.msg);
        }



		public void handleGameInfo(NetworkMessage msg){
			GameInfo infoMsg = Deserialize<GameInfo> (msg.reader.ReadBytesAndSize ());
			if (infoMsg.gameOver == false)
				return;

			isGameOver = true;
			canSendServerMsg = false;
			isListeningForTCP = false;
			UnetRoomConnector.shutdownCurrentConnection ();

			string gameOverMsg;
			if (infoMsg.winnerColor == Game.PlayerColor.None) {
				gameOverMsg = "It's a draw!";
				TCPLocalConnection.sendMessage ("GameOver: 0");
			}
			else {
				gameOverMsg = (infoMsg.winnerColor == PlayerColor.Blue ? "Pink" : "Yellow") + " won";
				TCPLocalConnection.sendMessage ("GameOver: " + (infoMsg.winnerColor == PlayerColor.Blue ? "1" : "-1"));
			}

			foreach (int[] crash in infoMsg.crashPos)
				localRenderer.displayCrash (new Vector2(crash[0], crash[1]));

			ClientUI.AlbotDialogBox.setGameOver ();
			ClientUI.AlbotDialogBox.activateButton (ClientUI.ClientUIStateManager.requestGotoGameLobby, ClientUI.DialogBoxType.GameState, gameOverMsg, "Return to lobby", 70, 25);
		}

		public void handleBoardUpdate(NetworkMessage msg){
			BoardUpdate updateMsg = Deserialize<BoardUpdate> (msg.reader.ReadBytesAndSize ());
			if (updateMsg.updateNumber <= lastUpdateID)
				return;
			lastUpdateID = updateMsg.updateNumber;

			localRenderer.handleBoardUpdate (updateMsg);

			List<Position2D> blocked = new List<Position2D> ();
			blocked.AddRange (updateMsg.blueCoords);
			blocked.AddRange (updateMsg.redCoords);
            Position2D bluePos = updateMsg.blueCoords [updateMsg.blueCoords.Length - 1];
            Position2D redPos = updateMsg.redCoords [updateMsg.redCoords.Length - 1];

			if(hasLocalBluePlayer)
				TCPFormater [0].addNewUpdate (blocked, updateMsg.blueDir, updateMsg.redDir, bluePos, redPos);
			if(hasLocalRedPlayer)
				TCPFormater [1].addNewUpdate (blocked, updateMsg.redDir, updateMsg.blueDir, redPos, bluePos);
		}

		#region Utils
		private bool parseCommandMsg(string msg, out int dir){
			if (int.TryParse (msg [0].ToString (), out dir))
				return true;

			dir = -1;
			string firstWord = msg.Trim ().ToLower();
			switch (firstWord) {
				case "right": dir = 0; break;
				case "up": dir = 1; 	break;
				case "left": dir = 2; 	break;
				case "down": dir = 3; 	break;
			}
				
			return dir != -1;
		}

		public static int convertColorToTeam(PlayerColor color){return color == PlayerColor.Blue ? 0 : 1;}
		#endregion
	}
}