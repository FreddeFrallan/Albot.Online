using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Barebones.MasterServer;
using Barebones.Networking;

namespace ClientUI{

	public class ClientUIStateManager : MonoBehaviour {

		public GameObject winPanel;
		public Barebones.MasterServer.SceneField lobbyScene;
		private bool logingOut = false;
		private bool enteringGameLobby = false, enteringPreGame = false;
		private static ClientUIStateManager singelton;

		public void init(){
			SceneManager.activeSceneChanged += onSceneLoaded;
			Msf.Connection.StatusChanged += onMasterConnectionChanged;
			singelton = this;
			SceneManager.LoadScene (singelton.lobbyScene.SceneName);
		}


		#region Logout
		public static void requestLogout(){
			// ** HOTFIX Disable popup pannels
			singelton.winPanel.SetActive(false);

			AlbotDialogBox.setIgnoreConnectionLostMsg (1);
			singelton.logingOut = true;
			if (isInGameScene())
				loadNewScene (singelton.lobbyScene.SceneName);
			else
				singelton.logout ();
		}
		public static void requesGotoPreGame(){
			singelton.winPanel.SetActive(false);
			if (isInGameScene()) {
				singelton.enteringGameLobby = true;
				AlbotDialogBox.setIgnoreConnectionLostMsg (1);
				loadNewScene (singelton.lobbyScene.SceneName);
			} else
				singelton.onEnterPreGame ();
		}
		public static void requestGotoGameLobby(){
			StaticClientTrainingMode.setTrainingActivated (false);
			singelton.winPanel.SetActive(false);
			if (isInGameScene()) {
				singelton.enteringGameLobby = true;
				loadNewScene (singelton.lobbyScene.SceneName);
			} else
				singelton.onEnterGameLobby ();
		}
		private static void loadNewScene(string newScene){			
			AlbotDialogBox.setIgnoreConnectionLostMsg (1);
			SceneManager.LoadScene (newScene);
		}
		private static bool isInGameScene(){return SceneManager.GetActiveScene ().name != singelton.lobbyScene.SceneName;}

		private void onSceneLoaded(Scene oldScene, Scene newScene){
			if (newScene.name != lobbyScene.SceneName) {
				ClientUIOverlord.setUIState (ClientUIStates.PlayingGame);
				return;
			}
			if(oldScene.name != lobbyScene.SceneName)  // If we dissconnect from the game we kill the game connection
				UnetRoomConnector.shutdownCurrentConnection ();

			if (logingOut)
				logout ();
			else if (enteringGameLobby)
				onEnterGameLobby ();
			else if (enteringPreGame)
				onEnterPreGame ();
		}


		private void onEnterGameLobby(){
			enteringGameLobby = false;
			ClientUIOverlord.setUIState (ClientUIStates.GameLobby);
		}
		private void onEnterPreGame(){
			enteringPreGame = false;
			ClientUIOverlord.setUIState (ClientUIStates.PreGame);
		}
		private void onEnterPreSinglePlayerGame(){
			enteringPreGame = false;
			ClientUIOverlord.setUIState (ClientUIStates.PreGame);
		}
		private void logout(){
			ClientUIOverlord.setUIState (ClientUIStates.LoginMenu);
			Barebones.MasterServer.Msf.Client.Auth.LogOut ();
		}
		#endregion

		private void onMasterConnectionChanged(ConnectionStatus status){
			if (status == ConnectionStatus.Disconnected || status == ConnectionStatus.None) {
				
				if (ClientUIOverlord.currentState != ClientUIStates.LoginMenu) {
					AlbotDialogBox.activateButton (() => {}, ClientUI.DialogBoxType.MasterServerConnLost, "Connection to master server was lost!", "Close");

					if (logingOut == false)
						requestLogout ();
					else
						logingOut = false;
				}
			}				
		}

	}
}