using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;
using Barebones.Networking;

public class DevModeControlls : MonoBehaviour {

	// Use this for initialization
	void Start () {
	//	Msf.Connection.SetHandler ((short)CustomMasterServerMSG.spectateLogUpdate, handleLogUpdate);
	}
	
	// Update is called once per frame
	void Update () {
		/*
		if (Input.GetKeyDown (KeyCode.O)) 
			Msf.Server.Connection.SendMessage ((short)CustomMasterServerMSG.requestSpectatorGames, handleSpectatorList);


		if (Input.GetKeyDown (KeyCode.P)) {
			SpectatorSubscriptionsMsg msg = new SpectatorSubscriptionsMsg (){ broadcastID = 0 };
			Msf.Server.Connection.SendMessage ((short)CustomMasterServerMSG.startSpectate, msg, handleGameLog);
			print ("Still waiting here");
		}
		*/
	}

	private void handleGameLog(ResponseStatus status, IIncommingMessage msg){
		if (status == ResponseStatus.Error) {
			print("Shit fucked up");
			return;
		}

		SpectatorGameLog gameLog = msg.Deserialize<SpectatorGameLog> ();
		print ("Hurray got a gameLog ----------" + gameLog.gameLog.Length);
		foreach (string log in gameLog.gameLog)
			print ("Log: " + log);
	}


	private void handleSpectatorList(ResponseStatus status, IIncommingMessage msg){
		if (status == ResponseStatus.Error) {
			print("Shit fucked up");
			return;
		}

		SpectatorGameList currentGames = msg.Deserialize<SpectatorGameList> ();
		print ("Hurray got response");
		foreach (SpectatorGameInfo game in currentGames.currentGames) {
			print ("Got game: " + game.gameId + "  " + game.mapName);

			foreach (string p in game.players)
				print ("Player: " + p);
		}
	}


	private void handleLogUpdate(IIncommingMessage msg){
		print ("Got update--------");
		SpectatorGameLog update = msg.Deserialize<SpectatorGameLog> ();

		foreach (string s in update.gameLog)
			print ("Update: " + s);
	}
}
