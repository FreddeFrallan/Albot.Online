﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Barebones.Networking;
using Barebones.MasterServer;
using UnityEngine.Networking;
using AlbotServer;

namespace ClientUI{

	public class ClientUIOverlord : MonoBehaviour {
		public static bool hasLoaded = false;
		public static ClientUIStates currentState;

		public static event UIStateChanged onUIStateChanged;
		public delegate void UIStateChanged(ClientUIStates newState);

		public List<Button> buttonsBoundToHavingBot;
		public static ClientUIOverlord singleton;
		public GameObject loginMenu, loginWindow, gameLobby, gameListWindow, preGameWindow, lobbyBrowser, preTournamentLobby;
		public GameLobbyUI lobbyUI;
		public MenuBar menuBar;
		public ClientUserPanelUI userPanel;
		public ClientChatTMProEnabled clientChat;
		public GameObject clientLobbySuperParent;
		public AlbotDialogBox dialogBox;
		public MainMenu mainMenu;
		public ClientUIStateManager stateManager;

		private static AccountInfoPacket currentAcountInfo;

		void Awake(){
			if (singleton != null && singleton != this)
				Destroy (this.gameObject);

			if (hasLoaded == true)
				return;

			StartCoroutine (setLoadedState ());
			singleton = this;
			ClientLogin.LoggedIn += onLoggedIn;
			Msf.Client.Auth.LoggedOut += onLoggedOut;
			Msf.Client.Rooms.AccessReceived += joinedGame;
			DontDestroyOnLoad (clientLobbySuperParent);
		}

		IEnumerator setLoadedState(){
			yield return new WaitForSeconds (1);
			hasLoaded = true;
		}

		void Start(){
			if (onUIStateChanged != null) 
				onUIStateChanged.Invoke (ClientUIStates.LoginMenu);

			userPanel.init ();
			clientChat.initChat ();
			dialogBox.init ();
			mainMenu.init ();
			stateManager.init ();
		}


		private void checkTCPConnection(){
			if (TCPLocalConnection.currentState != ConnectionStatus.Connected) 
				foreach (Button b in buttonsBoundToHavingBot)
					b.interactable = false;
			else
				foreach (Button b in buttonsBoundToHavingBot)
					b.interactable = true;
		}

		public static void setUIState(ClientUIStates state){
			switch (state) {
            case ClientUIStates.LobbyBrowser:
                setMenuPanels(false, true, false);
                setLobbyPanels(false, true, false);
                break;
            case ClientUIStates.GameLobby:
				setMenuPanels (false, true, false);
				setLobbyPanels (true, false, false);
				currentAcountInfo = Msf.Client.Auth.AccountInfo;
				break;
			case ClientUIStates.PreGame:
				setMenuPanels (false, true, false);
				setLobbyPanels (false, false, false);
				break;
			case ClientUIStates.LoginMenu:
				setMenuPanels (true, false, false);
				setLobbyPanels (false, false, false);
				break;
            case ClientUIStates.PreTournament:
                setMenuPanels(false, false, false);
                setLobbyPanels(false, false, true);
                break;
            case ClientUIStates.PlayingTournament:
            case ClientUIStates.PlayingGame:
				setMenuPanels (false, false, false);
				setLobbyPanels (false, false, false);
				break;
            }
        

			if (state != ClientUIStates.PlayingGame)
				dissconnectFromGameRoom ();
			
			currentState = state; 
			if (onUIStateChanged != null)
				onUIStateChanged.Invoke (state);
		}


		public static void dissconnectFromGameRoom(){
			if(FindObjectOfType<NetworkManager>() != null)
				AlbotNetworkManager.singleton.StopClient ();
		}
		private static void setMenuPanels(bool login, bool lobby, bool gameCreate){
			singleton.loginMenu.SetActive (login);
			singleton.loginWindow.SetActive (login);
			singleton.gameLobby.SetActive (lobby);

			if (lobby == false)
				singleton.lobbyUI.closeLobby ();
		}

        private static void setLobbyPanels(bool gameList, bool lobbyBrowser = false, bool preTournament = false){
			singleton.gameListWindow.SetActive (gameList);
            singleton.lobbyUI.setLobbyBrowserState(lobbyBrowser);
            singleton.preTournamentLobby.SetActive(preTournament);
        }

		private void onLoggedIn(){setUIState (ClientUIStates.GameLobby);}
		private void onLoggedOut(){setUIState (ClientUIStates.LoginMenu);}
		private void joinedGame (RoomAccessPacket p){setUIState (ClientUIStates.PlayingGame);}

		public static AccountInfoPacket getCurrentAcountInfo(){return currentAcountInfo;}
	}



	public enum ClientUIStates{
		LoginMenu,
		GameLobby,
        LobbyBrowser,
        PreTournament,
        PlayingTournament,
        PreGame,
		PlayingGame,
        Stats,
	}
}