using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Barebones.Networking;

public class SinglePlayerGameMaster : MonoBehaviour {

	private static bool hasInit = false;
	private static SinglePlayerGameMaster singleton;
	private Action<string> readTCPInput;
	private Action restartGame;

	void Awake(){
		if (hasInit)
			return;
		hasInit = true;

		singleton = this;
		TCPLocalConnection.subscribeToTCPStatus (TCPStatusChanged);
	}

	void Update () {
		checkTCPInput ();
	}
		
	private void checkTCPInput(){
		if (readTCPInput == null)
			return;

		while (TCPMessageQueue.hasUnread) {
			ReceivedLocalMessage m = TCPMessageQueue.popMessage ();
			readTCPInput (m.message);
		}
	}


	private void TCPStatusChanged(ConnectionStatus status){
		if (ClientUI.ClientUIOverlord.currentState != ClientUI.ClientUIStates.PlayingGame)
			return;

		if (status == ConnectionStatus.Connected) {
			if (restartGame != null) {
				restartGame ();
				print ("Activated Restart");
			}
		}
		else if(status == ConnectionStatus.Disconnected || status == ConnectionStatus.None)
			TCPLocalConnection.restartServer ();
	}


	void OnDestroy(){
		hasInit = false;
		TCPLocalConnection.unSubscribeToTCPStatus (TCPStatusChanged);
	}


	public static void init(Action<string> handleTCPInput, Action restartGame){
		singleton.readTCPInput = handleTCPInput;
		singleton.restartGame = restartGame;
	}
}
