using AlbotServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Game;

public class ConnectedClient{
	public NetworkConnection conn;
	public int peerID;
	public List<ConnectedPlayer> players = new List<ConnectedPlayer> ();
	public ConnectedClient (NetworkConnection conn, int peerID){
		this.conn = conn;
		this.peerID = peerID;
	}
}

public class ConnectedPlayer{
	public ConnectedClient client;
	public int roomID;
	public string username;
	public int iconNumber = -1;
	public PlayerColor color = PlayerColor.None;
	public bool isReady = false;
	
	public ConnectedPlayer(string username, ConnectedClient client){
		this.username = username;
		this.client = client;
	}
	
	public ConnectedPlayer(ConnectedClient client, int id, string username, Game.PlayerColor color = Game.PlayerColor.None, int iconNumber = 0){
		this.client = client;
		this.roomID = id;
		this.username = username;
		this.iconNumber = iconNumber;
		this.color = color;
	}

    //Icon number is added at the MasterServer In the preGame
    public PlayerInfo getPlayerInfo() {
        return new PlayerInfo() {
            username = username,
            color = color,
        };
    }
}

