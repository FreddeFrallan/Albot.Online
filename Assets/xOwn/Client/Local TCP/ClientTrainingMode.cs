using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.Networking;
using Barebones.MasterServer;
using ClientUI;
using UnityEngine.UI;
using Game;

public class ClientTrainingMode : MonoBehaviour {
	public GameObject gameList, createGamesWindow;

	void Awake(){
		if (StaticClientTrainingMode.isReady == false)
			StaticClientTrainingMode.init (gameList, createGamesWindow);
		Destroy (gameObject);
	}

}
	


public class StaticClientTrainingMode{

	public static bool isReady = false;
	private static ClientController currentClientController;

	private static bool trainingActivated = false;

	private static bool playingGame = false;
	private static ClientTrainingMode singleton;

	#region tempGameSettings 
	public static string lastRoomId;
	public static Game.PlayerColor currentColor = PlayerColor.Yellow;
	#endregion
	
	public static void init (GameObject gl, GameObject cgw) {
		isReady = true;
		TCPLocalConnection.subscribeToTCPStatus(TCPStatusChanged);
		ClientUIOverlord.onUIStateChanged += onUiStateChanged;
	}
	

	private static void TCPStatusChanged(ConnectionStatus status){
		if (trainingActivated == false)
			return;

        /*
		if (status == ConnectionStatus.Connected) {
			UnetRoomConnector.shutdownCurrentConnection ();
			AlbotDialogBox.removeAllPopups ();
			Msf.Connection.SendMessage ((short)AlbotServer.ServerCommProtocl.RestartTrainingGame, lastRoomId);
			playingGame = true;
		}
		if (status == ConnectionStatus.Disconnected && ClientUIOverlord.currentState == ClientUIStates.PlayingGame) {
			if (playingGame == false)
				return;
			playingGame = false;
			TCPLocalConnection.restartServer ();
			currentClientController.stopGameTimers ();
		}
        */
	}


	// If we are playing a game and we reconnect a bot it will first load the "GameLobby" scene.
	// This function will be called when the reloading is finished.
	// When then start a new training game
	private static void onUiStateChanged(ClientUIStates newState){
		if (trainingActivated == false)
			return;

		if (newState == ClientUIStates.GameLobby)
			trainingActivated = false;
	}


	public static void setTrainingActivated(bool state, string roomID = ""){
		trainingActivated = state;
		lastRoomId = roomID;
		playingGame = state;
	}

	public static void setCurrentClientController(Game.ClientController cc){currentClientController = cc;}
}