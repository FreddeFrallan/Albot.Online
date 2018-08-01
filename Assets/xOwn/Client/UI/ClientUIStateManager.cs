using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Barebones.MasterServer;
using Barebones.Networking;
using System;

namespace ClientUI{

	public class ClientUIStateManager : MonoBehaviour {

		public GameObject winPanel;
		public SceneField lobbyScene;
		private bool logingOut = false;
		private bool enteringGameLobby = false, enteringPreGame = false;
		private static ClientUIStateManager singelton;

        private static ClientUIStates targetState;
        private static Action targetLogicAction;

        private static Dictionary<ClientUIStates, string> stateToScenes = new Dictionary<ClientUIStates, string>();

        public void init(){
			SceneManager.activeSceneChanged += onSceneLoaded;
			Msf.Connection.StatusChanged += onMasterConnectionChanged;
			singelton = this;
			SceneManager.LoadScene (singelton.lobbyScene.SceneName);
            initStateToScenes();

        }
        
        private void initStateToScenes() {
            stateToScenes.Add(ClientUIStates.GameLobby, singelton.lobbyScene.SceneName);
            stateToScenes.Add(ClientUIStates.LobbyBrowser, singelton.lobbyScene.SceneName);
            stateToScenes.Add(ClientUIStates.LoginMenu, singelton.lobbyScene.SceneName);
            stateToScenes.Add(ClientUIStates.PlayingTournament, singelton.lobbyScene.SceneName);
            stateToScenes.Add(ClientUIStates.PreGame, singelton.lobbyScene.SceneName);
            stateToScenes.Add(ClientUIStates.PreTournament, singelton.lobbyScene.SceneName);
        }

		#region State Changes
		public static void requestLogout(){requestGotoState(ClientUIStates.LoginMenu, Msf.Client.Auth.LogOut);}
        public static void requestGotoState(ClientUIStates newState, Action onCompleteAction = null) {
            requestGotoState(newState, stateToScenes[newState], onCompleteAction);
        }
        public static void requestGotoState(ClientUIStates newState, string targetScene, Action onCompleteAction = null) {
            if (ClientUIOverlord.currentState == newState && ClientUIOverlord.currentState != ClientUIStates.PlayingGame) {
                onCompleteAction();
                return;
            }

            targetState = newState;
            targetLogicAction = onCompleteAction;

            singelton.winPanel.SetActive(false);
            if (currentScene() != targetScene || newState == ClientUIStates.PlayingGame)
                loadNewScene(targetScene);
            else
                onEnterNewState();
        }
        private static void onEnterNewState() {
            ClientUIOverlord.setUIState(targetState);
            if(targetLogicAction != null)
                targetLogicAction();
        }
        #endregion


        #region Loading Scenes
        private static void loadNewScene(string newScene){
			AlbotDialogBox.setIgnoreConnectionLostMsg (1);
			SceneManager.LoadScene (newScene);
		}

        private void onSceneLoaded(Scene oldScene, Scene newScene){onEnterNewState();}
        private static string currentScene() { return SceneManager.GetActiveScene().name; }
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