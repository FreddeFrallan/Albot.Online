using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;
using Game;
using System;
using UnityEngine.SceneManagement;
using ClientUI;

namespace AdminUI{

	public class AdminUIManager : MonoBehaviour {

		public AdminMainMenu mainMenu;
		public GameObject authPanel, gameSelectionPanel, lobbyPanels;
		private static AdminUIManager singleton;
		private static List<Action> onLoadActions = new List<Action>();
		public static event AdminUIStateChanged onAdminUIStateChanged;
		public delegate void AdminUIStateChanged(ClientUIStates newState);
        private static Dictionary<ClientUIStates, string> statesToScenes = new Dictionary<ClientUIStates, string>();

        public SceneField tournamentScene, lobbyScene, statsScene;

		void Start () {
			singleton = this;
			SceneManager.sceneLoaded += (arg0, arg1) => {foreach(Action a in onLoadActions) a();};
			Msf.Connection.StatusChanged += ((status) => {if(status == Barebones.Networking.ConnectionStatus.Disconnected) requestLogout();});
			mainMenu.init ();
            initScenes();
        }
	    private void initScenes() {
            statesToScenes.Add(ClientUIStates.LoginMenu, lobbyScene.SceneName);
            statesToScenes.Add(ClientUIStates.GameLobby, lobbyScene.SceneName);
            statesToScenes.Add(ClientUIStates.LobbyBrowser, lobbyScene.SceneName);
            statesToScenes.Add(ClientUIStates.PlayingTournament, tournamentScene.SceneName);
            statesToScenes.Add(ClientUIStates.Stats, statsScene.SceneName);
        }


		private void setPanelStates(bool auth, bool gameSelect){
			authPanel.SetActive (auth);
			gameSelectionPanel.SetActive (gameSelect);
		}



		#region request handlers
		public static void requestLogin(){ requestGotoState(ClientUIStates.GameLobby); }
		public static void requestLogout(){
			Msf.Connection.SendMessage ((short)CustomMasterServerMSG.adminLogout);
            requestGotoState(ClientUIStates.LoginMenu);
		}
        public static void requestGotoState(ClientUIStates newState, Action onLoad = null) {
			onLoadActions.Clear ();
            if (onLoad != null)
                onLoadActions.Add(onLoad);

            singleton.setUIState(newState);
            if (SceneManager.GetActiveScene().name != statesToScenes[newState])
                SceneManager.LoadScene(statesToScenes[newState]);
        }

		public static void requestGotoGame(GameType type, Action onLoad){
			singleton.setUIState (ClientUIStates.PlayingGame);
		    onLoadActions.Clear ();
			onLoadActions.Add (onLoad);
			SceneManager.LoadScene (getAdminGameSceneName (type));
		}
		#endregion


		private void setUIState(ClientUIStates state){
			singleton.lobbyPanels.SetActive (state != ClientUIStates.PlayingGame);
			if (state != ClientUIStates.PlayingGame)
				AdminUpdateManager.stopSpectating ();

			switch (state) {
			case ClientUIStates.GameLobby:
				singleton.setPanelStates (false, true);
				break;

			case ClientUIStates.LoginMenu:
				singleton.setPanelStates (true, false);
				break;

            case ClientUIStates.PlayingTournament:
            case ClientUIStates.Stats:
                singleton.setPanelStates(false, false);
            break;
            }


			if (onAdminUIStateChanged != null)
				onAdminUIStateChanged.Invoke (state);
		}

		private static string getAdminGameSceneName(GameType type){return type.ToString () + "Game";}
	}

}