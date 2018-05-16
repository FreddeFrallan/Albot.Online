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

		private static readonly string emptySceneName = "EmptyScene";

		// Use this for initialization
		void Start () {
			singleton = this;
			SceneManager.sceneLoaded += (arg0, arg1) => {foreach(Action a in onLoadActions) a();};
			Msf.Connection.StatusChanged += ((status) => {if(status == Barebones.Networking.ConnectionStatus.Disconnected) requestLogout();});
			mainMenu.init ();
		}
	

		private void setPanelStates(bool auth, bool gameSelect){
			authPanel.SetActive (auth);
			gameSelectionPanel.SetActive (gameSelect);
		}



		#region request handlers
		public static void requestLogin(){singleton.setUIState (ClientUIStates.GameLobby);}
		public static void requestLogout(){
			onLoadActions.Clear ();
			Msf.Connection.SendMessage ((short)CustomMasterServerMSG.adminLogout);
			singleton.setUIState (ClientUIStates.LoginMenu);
		}
		public static void requestGotoGameLobby(){
			onLoadActions.Clear ();
			singleton.setUIState (ClientUIStates.GameLobby);

			if (SceneManager.GetActiveScene ().name != emptySceneName)
				SceneManager.LoadScene (emptySceneName);
		}

		public static void requestGotoGame(GameType type, Action onLoad){
			singleton.setUIState (ClientUIStates.PlayingGame);
			if (SceneManager.GetActiveScene ().name == getAdminGameSceneName (type))
				onLoad ();
			else {
				onLoadActions.Clear ();
				onLoadActions.Add (onLoad);
				SceneManager.LoadScene (getAdminGameSceneName (type));
			}
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
			}


			if (onAdminUIStateChanged != null)
				onAdminUIStateChanged.Invoke (state);
		}

		private static string getAdminGameSceneName(GameType type){return type.ToString () + "Admin";}
	}

}