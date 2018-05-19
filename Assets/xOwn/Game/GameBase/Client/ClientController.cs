﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Barebones.MasterServer;
using AlbotServer;

namespace Game{

	public abstract class ClientController : NetworkBehaviour{
		public abstract void initProtocol (CommProtocol protocol);
		public abstract void onOutgoingLocalMsg (string msg, PlayerColor color);
		public void onOutgoingLocalMsgObj (object msg, short type){
			sendServerMsg (msg, type);
		}

		protected virtual void initHandlers(){
			connectionToServer.RegisterHandler ((short)ServerCommProtocl.PlayerJoinedGameRoom, handlePlayerJoinedRoom);
			connectionToServer.RegisterHandler ((short)ServerCommProtocl.PlayerLeftGameRoom, handlePlayerLeftRoom);
		}
		public abstract GameType getGameType();
		protected NetworkConnection serverConnection;
		protected bool isListeningForTCP = false, canSendServerMsg = true;
		protected GameWrapper wrapper  = new GameWrapper ();
		protected GameUI localGameUI;


		public void initController(Game.GameType currentType){
			if (currentType != getGameType()) {
				Destroy (this);
				return;
			}
			ClientPlayersHandler.init (this);
			StaticClientTrainingMode.setCurrentClientController (this);

			base.OnStartAuthority();
			initHandlers ();
			ClientReadyMsg msg = new ClientReadyMsg (){players = ClientPlayersHandler.generatePlayersInfoArray()};
			connectionToServer.Send ((short)ServerCommProtocl.ClientReadyChannel, msg);
			//StartCoroutine (listenForTCP ());
		}
        
        //LEGACY, will soon be obselete
		protected IEnumerator listenForTCP(){
			while (true) {
				yield return new WaitForEndOfFrame ();
				//If we have receveid some Local TCP msg. Listen to them all
                
				while (isListeningForTCP && TCPMessageQueue.hasUnread)
					readTCPMsg(TCPMessageQueue.popMessage ());
			}
		}
			

		//This will be replaced when we move the renderer to seperate project!!!
		//Coroutine that we run until we find a localGameController in the scene we are in
		protected IEnumerator findAndInitRenderer<T>(Action<T> found) where T : Component{
			GameObject foundObj = GameObject.FindGameObjectWithTag ("GameController");;
			while (foundObj == null) {
				foundObj = GameObject.FindGameObjectWithTag ("GameController");
				yield return new WaitForEndOfFrame ();
			}
			T renderType = foundObj.GetComponent<T>();	
			localGameUI = foundObj.GetComponent<GameUI> ();	
			found (renderType);
		}


			
		#region Message Handling
		//For some fucking reason this has to be Static?!?!?!?!? *#"*#*!
		//Took me about 3 hours to find that out....
		private static List<OutMsg> networkMsgQueue = new List<OutMsg> ();

		//Try to send the msg now, if it fails it adds the msg to networkMsgQueue
		public void sendServerMsg(string msg, Game.PlayerColor color, short msgType){
			object stringMsg = (object)new Game.CommProtocol.StringMessage(msg, color);
			sendServerMsg (stringMsg, msgType);
		}

		//Create a generic type of this
		public void sendServerMsg(object objMsg, short msgType){
			if (canSendServerMsg == false)
				return;

			byte[] bytes;
			using (MemoryStream stream = new MemoryStream()){
				new BinaryFormatter().Serialize(stream, objMsg);
				bytes = stream.ToArray();
			}
			if (bytes.Length == 0)
				return;

			OutMsg serverMsg = new OutMsg (bytes, msgType);
			try{sendMsg(serverMsg);}
			catch{networkMsgQueue.Add(serverMsg);}
		}


		//Actuall outgoing msg
		//Gets sent to "listenForClientMsg" here all types currentely needs to manually added
		private void sendMsg(OutMsg msg){
			ByteMessage bMsg = new ByteMessage (){msg = msg.msg};
			connectionToServer.Send (msg.msgType, bMsg);
		}

		//Runs every frame and checks if there are unsent msges, if so sends them
		protected IEnumerator handleNetworkMsgQueue(){
			while (true) {
				if (networkMsgQueue.Count > 0) {
					try{
						foreach (OutMsg m in networkMsgQueue){
							sendMsg (m);
							networkMsgQueue.Remove(m);
						}
					}
					catch{}
				}
				yield return new WaitForEndOfFrame ();
			}
		}

	
		protected virtual void readTCPMsg (ReceivedLocalMessage msg){
			string[] words = msg.message.Split (' ');
			if (words.Length != 1)
				return;

			try{Game.ClientPlayersHandler.onReceiveLocalTCPMsg(words[0]);
			}catch{return;}
		}
		public static T Deserialize<T>(byte[] param){
			using (MemoryStream ms = new MemoryStream(param)){
				IFormatter br = new BinaryFormatter();
				return (T)br.Deserialize(ms);
			}
		}
		public class ByteMessage : MessageBase {public byte[]  msg;}
		public class OutMsg{
			public byte[] msg;
			public short msgType;
			public OutMsg(byte[] msg, short msgType){this.msg = msg;  this.msgType = msgType;}
		}

		#endregion

		public void gameOver(){
			UnetRoomConnector.shutdownCurrentConnection ();
			localGameUI.stopAllTimers ();
			ClientPlayersHandler.killBots ();
		}

		//For in editor
		void OnApplicationQuit(){ClientPlayersHandler.killBots ();}
		public void stopGameTimers(){localGameUI.stopAllTimers ();}


		#region universal handlers
		public void handlePlayerTimerInit(NetworkMessage msg){
			byte[] bytes = msg.reader.ReadBytesAndSize ();
			Game.PlayerTimerMsg initMsg = Game.ClientController.Deserialize<Game.PlayerTimerMsg> (bytes);
			localGameUI.initTimer (initMsg.color, initMsg.maxTime);
		}
		public void handlePlayerTimerCommand(NetworkMessage msg){
			byte[] bytes = msg.reader.ReadBytesAndSize ();
			Game.PlayerTimerMsg timeCommandMsg = Game.ClientController.Deserialize<Game.PlayerTimerMsg> (bytes);
			localGameUI.startTimer (timeCommandMsg.color, timeCommandMsg.maxTime);
		}

		public virtual void handlePlayerLeftRoom(NetworkMessage msg){
			PlayerInfoMsg readyMsg = msg.ReadMessage<PlayerInfoMsg> ();
			PlayerInfo p = readyMsg.player;

			localGameUI.removeConnectedPlayer (p.color, p.username, p.iconNumber);
		}
		public virtual void handlePlayerJoinedRoom(NetworkMessage msg){
			PlayerInfoMsg readyMsg = msg.ReadMessage<PlayerInfoMsg> ();
			PlayerInfo p = readyMsg.player;

			localGameUI.initPlayerSlot (p.color, p.username, p.iconNumber);
		}
		#endregion
	}
		

}