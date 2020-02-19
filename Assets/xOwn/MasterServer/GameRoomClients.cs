using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;
using Barebones.MasterServer;
using AlbotServer;

public class GameRoomClients{

	private static List<ConnectedClient> currentClients = new List<ConnectedClient> ();
	private static List<ConnectedClient> disconnectedClients = new List<ConnectedClient> ();
	private static int roomPlayerID = 1;


	public static void init(){
		ConnectionManager.onPlayerJoined += broadcastPlayerJoinedAndIsReady;
	}

	public static void broadcastPlayerJoinedAndIsReady(ConnectedPlayer newPlayer){
		PlayerInfoMsg msg = new PlayerInfoMsg ();
		foreach (ConnectedClient c in currentClients) {
			try{
				//Notify all the already connected Clients
				msg.player.username = newPlayer.username;
				msg.player.iconNumber = newPlayer.iconNumber;
				msg.player.color = newPlayer.color;
				c.conn.Send((short)ServerCommProtocl.PlayerJoinedGameRoom, msg);

				if(c == newPlayer.client)
					continue;

				//Init new Player to all already existing players
				foreach(ConnectedPlayer p in c.players){
					msg.player.username = p.username;
					msg.player.iconNumber = p.iconNumber;
					msg.player.color = p.color;
					newPlayer.client.conn.Send((short)ServerCommProtocl.PlayerJoinedGameRoom, msg);
				}
			}catch(Exception e){Debug.LogError (e.Data);}
		}
	}

	private static void broadcastPlayerLeftRoom(ConnectedPlayer leavingPlayer){
		PlayerInfoMsg msg = new PlayerInfoMsg ();
		foreach (ConnectedClient c in currentClients) {
			try{
				//Notify all the connnectedPlayers
				msg.player.username = leavingPlayer.username;
				msg.player.iconNumber = leavingPlayer.iconNumber;
				msg.player.color = leavingPlayer.color;
				c.conn.Send((short)ServerCommProtocl.PlayerLeftGameRoom, msg);
			}catch(Exception e){Debug.LogError (e.Data);}
		}
	}


	public static void clientConnected(ConnectedClient c){
		if (disconnectedClients.Contains (c))
			disconnectedClients.Remove (c);

		if (currentClients.Contains (c) == false)
			currentClients.Add (c);
	}
		
	public static void clientDisconnected(ConnectedClient c){
		if (currentClients.Contains (c) == false) {
			//throw new Exception ("Trying to locally remove a player that is not stored as connected!");
			Debug.LogError("Trying to locally remove a player that is not stored as connected!");
			return;
		}

		//Let all the remaining players that p just left.
		foreach(ConnectedPlayer p in c.players)
			broadcastPlayerLeftRoom (p);
        //Tell the Master that a player left the room
        string roomID = Msf.Server.Rooms.currentRoomID;
		Msf.Server.Rooms.NotifyPlayerLeft (roomID, c.peerID, (status, msg) => {});


		currentClients.Remove (c);
		if (disconnectedClients.Contains (c) == false)
			disconnectedClients.Add (c);
	}

	public static int currentClientCount(){return currentClients.Count;}

	public static ConnectedClient getMatchingClient(NetworkConnection conn){
		return currentClients.Find (f => f.conn == conn);
	}
	public static ConnectedClient getMatchingClient(int peerID){
		return currentClients.Find (f => f.peerID == peerID);
	}

	public static int getRoomPlayerID(NetworkConnection conn){
		/*
		if (currentClients.Any(f => f.conn == conn))
			return currentClients [currentClients.FindIndex (f => f.conn == conn)].roomID;

		if (disconnectedClients.Any(f => f.conn == conn))
			return disconnectedClients [disconnectedClients.FindIndex (f => f.conn == conn)].roomID;
		*/

		return roomPlayerID++;
	}

	public static List<ConnectedClient> getCurrentClients(){
		return currentClients;
	}

}
