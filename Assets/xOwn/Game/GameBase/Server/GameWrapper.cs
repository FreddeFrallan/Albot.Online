using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using Game;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Barebones.MasterServer;
using Barebones.Networking;

public class GameWrapper{

	protected CommProtocol currentProtocol;
	protected Game.GameMaster currentController;
	private List<CommProtocol.AlbotHandler> currentHandlers = new List<CommProtocol.AlbotHandler> ();
	protected Action<object, ConnectedClient> trainingMoveFunc;

	public void init(CommProtocol prot, Game.GameMaster controller){
		currentProtocol = prot;
		this.currentController = controller;
		controller.getGameHistory ().initHandlers();
	}

	
	public void registerHandler(CommProtocol.AlbotHandler newHandler, int playerID){
		ConnectedClient c = GameRoomClients.getMatchingClient (playerID);
		c.conn.RegisterHandler (newHandler.msgType, listenForClientMsg);
		currentHandlers.Add (newHandler);

		Debug.LogError ("Subscribing Handler for: " + playerID + "  " + newHandler.msgType);
	}


	public void listenForClientMsg(NetworkMessage msg){
		ConnectedClient c = GameRoomClients.getMatchingClient (msg.conn);
		CommProtocol.AlbotHandler h = currentHandlers.Find (x => x.msgType == msg.msgType);

		//There should be someway to do this with generics and without reflection right?
		try{
			byte[] bytes = msg.reader.ReadBytesAndSize ();
			object objMsg = null;

			if(h.type == typeof(CommProtocol.StringMessage))
				objMsg = Game.ClientController.Deserialize<CommProtocol.StringMessage> (bytes);
			if(h.type == typeof(Soldiers.PlayerCommands))
				objMsg = Game.ClientController.Deserialize<Soldiers.PlayerCommands> (bytes);
			if(h.type == typeof(Bomberman.PlayerCommand))
				objMsg = Game.ClientController.Deserialize<Bomberman.PlayerCommand> (bytes);
			if(h.type == typeof(Snake.GameCommand))
				objMsg = Game.ClientController.Deserialize<Snake.GameCommand> (bytes);

			h.func (objMsg, c);
		}
		catch{}
	}

	//This is hijacked in Training wrapper
	public virtual void sendMsg(object msg, int targetID, short msgId){
		Type msgType = currentProtocol.getMatchingType(msgId);

		ConnectedClient c = GameRoomClients.getMatchingClient (targetID);
		if (c == null)
			throw new Exception ("Tried to send msg to a non existing player!");
		if(c.conn.isConnected == false)
			throw new Exception("Error sending message " + msgType.ToString() + ", no connection to client");

		sendMsg (msg, c.conn, msgId);
	}

	public void sendMsg(object msg, NetworkConnection c, short msgId){
		byte[] bytes;
		using (MemoryStream stream = new MemoryStream()){
			new BinaryFormatter().Serialize(stream, msg);
			bytes = stream.ToArray();
		}
		ByteMessage netMsg = new ByteMessage ();
		netMsg.msg = bytes;
		c.Send (msgId, netMsg);
	}



	public void clientLeft(ConnectedClient c){
		Debug.LogError ("Client left game");
		foreach(ConnectedPlayer p in  currentController.getClientPlayers(c))
			currentController.onPlayerLeft(p);
	}


	public class ByteMessage : MessageBase {public byte[]  msg;}
}
