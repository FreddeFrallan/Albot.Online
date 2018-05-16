using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Game;
using System;
using System.Linq;

namespace AlbotServer{

	public class ConnectionManager : MonoBehaviour {
		
		private class readyClient{
			public NetworkConnection conn;
			public List<PlayerInfo> players; 
			public readyClient(NetworkConnection conn, List<PlayerInfo> players){
				this.conn = conn; this.players = players;
			}
		}

		private GameType currentGameType;
		private GameMaster gameController;
		private List<readyClient> readyConnections = new List<readyClient>();
		public static event Action<ConnectedPlayer> onPlayerJoined;

		public void init(GameMaster gameController, GameType currentGameType){
			this.currentGameType = currentGameType;
			this.gameController = gameController;
		}

		public void OnServerAddClient(NetworkConnection conn){
			conn.RegisterHandler ((short)ServerCommProtocl.GameRoomInitMsgChannel, handleGameRoomInit);
			conn.RegisterHandler ((short)ServerCommProtocl.ClientReadyChannel, handleConnectionReady);
		}

		//Will receive a msg from newly connected client, this message contains what players will be playing from that client, and their info.
		public void handleConnectionReady(NetworkMessage msg){
			ClientReadyMsg readyMsg = msg.ReadMessage<ClientReadyMsg> ();
			readyConnections.Add (new readyClient(msg.conn, readyMsg.players.ToList()));
			if (readyConnections.Count == 1)//If count == 1, means that the coroutine is NOT currently running.
				StartCoroutine (handleConnectionReadyIem ());
		}

		//A little hack to add ready players!
		//Sometimes it seems that the connection does not respond early in the init stage, so we try until it works!
		//We only add the new clients players if he is registred in "GameRoomClients". 
		//This reg happens in AlbotGameRoom "handleHandleReceivedAccess".
		private IEnumerator handleConnectionReadyIem(){
			while (readyConnections.Count > 0) {
				yield return new WaitForEndOfFrame ();
				List<readyClient> workingConnections = new List<readyClient> ();

				foreach(readyClient r in readyConnections){
					ConnectedClient c = GameRoomClients.getMatchingClient (r.conn);
					if (c == null)
						continue;

					foreach(PlayerInfo p in r.players){
						//Reference does not survive Dll files it seems...
						//We let the gamecontroller give the newPlayer some game settings.. (color) change this later to only return a new Class gamesettings
						ConnectedPlayer changedPlayer = gameController.onPlayerJoined (new ConnectedPlayer(p.username, c));

						//Add the newplayer to matching client
						c.players.Add (new ConnectedPlayer(c, changedPlayer.roomID, p.username, changedPlayer.color, p.iconNumber));


						//Invoke event on newly added player
						if(onPlayerJoined != null)
							onPlayerJoined.Invoke (c.players[c.players.Count-1]);

						Debug.LogError (p.username + " has joined and is ready!");
					}
					workingConnections.Add (r);
				}
				foreach (readyClient r in workingConnections)
					readyConnections.Remove (r);
			}
		}


		//Handle requst that someone wishes to know the currentGametype. 
		public void handleGameRoomInit(NetworkMessage msg){
			GameRoomInitMsg response = new GameRoomInitMsg (){type = currentGameType};
			msg.conn.Send ((short)ServerCommProtocl.GameRoomInitMsgChannel, response);
			Debug.LogError ("Sending response msg");
		}
	}

}