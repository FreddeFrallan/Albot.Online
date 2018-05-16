using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
	public Game.PlayerColor color = Game.PlayerColor.None;
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
}

