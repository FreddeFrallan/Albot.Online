using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ClientUI;
using Barebones.MasterServer;
using Barebones.Networking;

namespace ClientUI
{
	/// <summary>
	///     Game creation window
	/// </summary>
	public class GameConnectorUI : MonoBehaviour{
		public CreateGameProgressUi ProgressUi;
		private PreGameBaseLobby preLobby;
		//Sends a request to the master server to join the game
		//If yes, UnetRoomConnector will run "ConnectToGame" function

		public void onJoinStartedGame(IIncommingMessage message){
			AlbotServer.PreGameStartMsg msg = message.Deserialize<AlbotServer.PreGameStartMsg> ();
			if (msg.errorMsg != "") {
				Msf.Events.Fire(Msf.EventNames.ShowDialogBox, DialogBoxData.CreateError(msg.errorMsg));
				return;
			}
			preLobby.removeHandlers ();
			preLobby.setLocalPreGamePlayers ();
			SpawnRequestController controller = new SpawnRequestController(msg.roomID, Msf.Connection);
			MsfSpawnersClient._localSpawnRequests[controller.SpawnId] = controller;
			ProgressUi.Display(controller);
			StaticClientTrainingMode.setTrainingActivated (msg.isTraining, msg.trainingRoomID);
		}


		protected virtual void OnPassReceived(RoomAccessPacket packet, string errorMessage){
			if (packet == null){
				Msf.Events.Fire(Msf.EventNames.ShowDialogBox, DialogBoxData.CreateError(errorMessage));
				Logs.Error(errorMessage);
				return;
			}
			// Hope something handles the event
		}


		public void setCurrentPreLobby(PreGameBaseLobby preLobby){this.preLobby = preLobby;}
	}
}