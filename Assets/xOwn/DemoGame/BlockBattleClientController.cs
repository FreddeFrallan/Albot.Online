using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using UnityEngine.Networking;
using AlbotServer;


namespace BlockBattle{
	
	public class BlockBattleClientController : Game.ClientController {

		private BlockBattleProtocol protocol;
		private BlockBattleRenderer clientRenderer;

		public override Game.GameType getGameType (){return GameType.BlockBattle;}
		public override void initProtocol (Game.CommProtocol protocol){this.protocol = (BlockBattleProtocol)protocol;}


		public override void onOutgoingLocalMsg (string msg, Game.PlayerColor color){
			print ("Sending server msg: " + msg + "  from: " + color);
		}

		protected override void readTCPMsg (ReceivedLocalMessage msg){
			print ("Got TCP msg: " + msg.message);
		}

		protected override void initHandlers (){
			base.initHandlers ();
			connectionToServer.RegisterHandler ((short)BlockBattleProtocol.MsgType.gameInit, handleInitSettings);
			connectionToServer.RegisterHandler ((short)BlockBattleProtocol.MsgType.gameUpdate, handleGameUpdate);

			StartCoroutine (findAndInitRenderer<BlockBattleRenderer>((x) => this.clientRenderer = x));
			StartCoroutine (handleNetworkMsgQueue ());
		}
			


		public void handleInitSettings(NetworkMessage initMsg){
			GameInfo msg = Deserialize<GameInfo> (initMsg.reader.ReadBytesAndSize ());
			ClientPlayersHandler.initPlayerColor (msg.username, msg.myColor);
			LocalPlayer p = ClientPlayersHandler.getPlayerFromColor (msg.myColor);

			RealtimeTCPController.registerLocalPlayer (colorToTeam(msg.myColor), p.takeInput, p.isMainPlayer());
			if (p.isMainPlayer ())
				isListeningForTCP = true;
		}

		private int colorToTeam(PlayerColor color){return color == PlayerColor.Green ? 0 : 1;}

		public void handleGameUpdate(NetworkMessage updateMsg){
			GameUpdate msg = Deserialize<GameUpdate> (updateMsg.reader.ReadBytesAndSize ());
			clientRenderer.movePlayer (msg.blueCoords, msg.redCoords);

			RealtimeTCPController.gotNewBoard (colorToTeam (msg.myColor), formatBoard (msg));
		}
			
		private string formatBoard(GameUpdate update){
			int[] p1 = (update.myColor == PlayerColor.Green) ? update.blueCoords : update.redCoords;
			int[] p2 = (update.myColor == PlayerColor.Green) ? update.redCoords : update.blueCoords;
			return string.Format ("P1: {0}, {1}  P2: {2}, {3}", p1 [0], p1 [1], p2 [0], p2 [1]);
		}
	}
}