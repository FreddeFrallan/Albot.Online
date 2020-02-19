using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using UnityEngine.Networking;
using AlbotServer;
using ClientRealtimeGame;

namespace FallingDebris{

	public class FallingDebrisGameController :  Game.ClientController {

		private FallingDebrisRenderer localRenderer;
		protected CommProtocol protocol;

		public override void initProtocol (Game.CommProtocol protocol){this.protocol = (FallingDebris.CommProtocol)protocol;}
        protected override void initHandlers (){
			connectionToServer.RegisterHandler ((short)CommProtocol.MsgType.boardUpdate, handleGameState);
			connectionToServer.RegisterHandler ((short)CommProtocol.MsgType.playerInit, handleInitSettings);
			connectionToServer.RegisterHandler ((short)ServerCommProtocl.PlayerJoinedGameRoom, handlePlayerJoinedRoom);
			connectionToServer.RegisterHandler ((short)ServerCommProtocl.PlayerLeftGameRoom, handlePlayerLeftRoom);
			StartCoroutine (findAndInitLocalGameController ());
			StartCoroutine (handleNetworkMsgQueue ());
		}

		public override Game.GameType getGameType (){return Game.GameType.FallingDebris;}


		IEnumerator findAndInitLocalGameController(){
			while (localRenderer == null) {
				GameObject foundObj = GameObject.FindGameObjectWithTag ("GameController");
				if (foundObj != null) {
					localRenderer = foundObj.GetComponent<FallingDebrisRenderer> ();	
					localGameUI = foundObj.GetComponent<GameUI> ();	
				}

				yield return new WaitForEndOfFrame ();
			}
			Debug.Log ("Game controller inited");
		}


		//InProgress
		public override void onOutgoingLocalMsg (string msg, Game.PlayerColor color){
			sendServerMsg(msg, color, (short)CommProtocol.MsgType.move);
		}


		public void handlePlayerLeftRoom(NetworkMessage msg){
			PlayerInfoMsg readyMsg = msg.ReadMessage<PlayerInfoMsg> ();
			PlayerInfo p = readyMsg.player;
			localGameUI.removeConnectedPlayer (p.color, p.username, p.iconNumber);
		}
		public void handlePlayerJoinedRoom(NetworkMessage msg){
	//		PlayerReadyMsg readyMsg = msg.ReadMessage<PlayerReadyMsg> ();
		//	localGameUI.initPlayerSlot (readyMsg.UISlot, readyMsg.color, readyMsg.username, readyMsg.iconNumber);
		}
		public void handleInitSettings(NetworkMessage initMsg){
			
		}
		public void handleGameState(NetworkMessage gameStateMsg){
			byte[] bytes = gameStateMsg.reader.ReadBytesAndSize ();
			GameState stateMsg =  Game.ClientController.Deserialize<GameState> (bytes);

			localRenderer.updatePlayerPosition (stateMsg.currentPos, stateMsg.currentTarget, stateMsg.time);
			TCPLocalConnection.sendMessage (stateMsg.currentPos.ToString ());
		}




	}

}