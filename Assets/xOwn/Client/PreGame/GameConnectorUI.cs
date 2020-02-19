using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ClientUI;
using Barebones.MasterServer;
using Barebones.Networking;
using AlbotServer;

namespace ClientUI{
	/// <summary>
	///     Game creation window
	/// </summary>
	public class GameConnectorUI : MonoBehaviour{
		public CreateGameProgressUi ProgressUi;
		private PreGameBaseLobby preLobby;
        //Sends a request to the master server to join the game
        //If yes, UnetRoomConnector will run "ConnectToGame" function

        public void onJoinStartedGame(PreGameStartedMsg gameInfo) { 
			preLobby.removeHandlers ();
			SpawnRequestController controller = new SpawnRequestController(gameInfo.specs.roomID, Msf.Connection, gameInfo.specs.spawnCode);
			MsfSpawnersClient._localSpawnRequests[controller.SpawnId] = controller;
			ProgressUi.Display(controller);
		}

		public void setCurrentPreLobby(PreGameBaseLobby preLobby){this.preLobby = preLobby;}
	}
}