using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;
using Barebones.Networking;
using System;
using AlbotServer;

public class AlbotServerChat : ServerModuleBehaviour {
	private class chatUser{
		public IPeer peer;
		public string username;
		public int icon;
		public Int64 chatID;
		public chatUser(IPeer peer, string username, int icon, Int64 chatID){this.peer = peer;this.username = username;this.icon = icon; this.chatID = chatID;}
	}
	private List<chatUser> playersInChat = new List<chatUser>();
	private Int64 chatIdCounter = 0;


	public override void Initialize (IServer server){
		base.Initialize (server);
		Debug.LogError ("Server Chat init");
		server.SetHandler ((short)ServerCommProtocl.LobbyChatMsg, handleIncomingMsg);
		server.SetHandler ((short)ServerCommProtocl.LobbyPlayerEnter, handleOnJoinedChat);
		server.SetHandler ((short)ServerCommProtocl.LobbyPlayerLeft, handleOnLeftChat);
	}


	private void handleOnJoinedChat(IIncommingMessage msg){
		if(playersInChat.Find(x => x.peer == msg.Peer) != null)return;

		//Creates a new chatUser and assigns it a "uniqe" chatId... until we have more users then a long can carry
		//I doubt this will be a problem in the near future....
		AlbotChatMsg chatMsg = msg.Deserialize<AlbotChatMsg> ();
		chatUser newUser = new chatUser (msg.Peer, chatMsg.username, chatMsg.icon, chatIdCounter++);

		//Let everone who already is in chat know!
		AlbotChatMsg joinedMsg = new AlbotChatMsg ();
		joinedMsg.icon = newUser.icon;
		joinedMsg.username = newUser.username;
		joinedMsg.chatID = newUser.chatID;
		broadcastMsg (joinedMsg, (short)ServerCommProtocl.LobbyPlayerEnter);
	
		playersInChat.Add (newUser);
		newUser.peer.Disconnected += handleDissconnect;

		//Init the new users chat with all current members, Perhaps this should be one big msg?!
		foreach (chatUser u in playersInChat) {
			joinedMsg = new AlbotChatMsg ();
			joinedMsg.icon = u.icon;
			joinedMsg.username = u.username;
			joinedMsg.chatID = u.chatID;
			newUser.peer.SendMessage ((short)ServerCommProtocl.LobbyPlayerEnter, joinedMsg);
		}
	}

	//Broadcasts the msg to all clients currently in chat
	private void handleIncomingMsg(IIncommingMessage msg){
		AlbotChatMsg chatMsg = msg.Deserialize<AlbotChatMsg> ();
		broadcastMsg (chatMsg, (short)ServerCommProtocl.LobbyChatMsg);
	}


	private void handleOnLeftChat(IIncommingMessage msg){handleDissconnect (msg.Peer);}
	//Can be called from either "handleOnLeftChat()" when a player joines a gameroom or by loging out
	//Or it can be called from the user dissconnecting from the server
	private void handleDissconnect(IPeer peer){
		chatUser user = playersInChat.Find (x => x.peer == peer);
		if(user == null){return;}

		playersInChat.Remove (user);
		user.peer.Disconnected -= handleDissconnect;

		AlbotChatMsg msg = new AlbotChatMsg ();
		msg.chatID = user.chatID;
		broadcastMsg (msg, (short)ServerCommProtocl.LobbyPlayerLeft);
	}
		

	private void broadcastMsg(AlbotChatMsg msg, short msgType){
		foreach (chatUser u in playersInChat) {
			try{
				u.peer.SendMessage (msgType, msg);
			}catch{}
		}
	}


}
