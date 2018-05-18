using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TCPMessageQueue{
	private static List<ReceivedLocalMessage> messages = new List<ReceivedLocalMessage> ();
	public static bool hasUnread = false;
	public static Action<ReceivedLocalMessage> readMsgInstant;


	//Could possibly be more optimized later, trying to not check for null every msg, might be benefitical
	public static void addMessage(string message, int playerID){
		message = message.Trim ();
		ReceivedLocalMessage newMsg = new ReceivedLocalMessage (message, playerID);
		messages.Add (newMsg);
		hasUnread = true;

		if (readMsgInstant != null)
			readMsgInstant (newMsg);
	}

	public static ReceivedLocalMessage popMessage(){
		ReceivedLocalMessage msg = messages [0];
		messages.RemoveAt (0);

		if (messages.Count <= 0)
			hasUnread = false;

		return msg;
	}
}


public struct ReceivedLocalMessage{
	public int playerID;
	public string message;
	public ReceivedLocalMessage(string message, int playerID){
		this.playerID = playerID;
		this.message = message;
	}
}
