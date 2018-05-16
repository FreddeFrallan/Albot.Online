using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Barebones.MasterServer;
using Game;
using Barebones.Networking;
using AlbotServer;
using UnityEngine.SceneManagement;

namespace ClientUI{

	public class SinglePlayerLobby : MonoBehaviour {
		private static bool hasInit = false;
		public Image gameImage;
		public PreGamePlayerSlot playerSlot;
		public Button startButton;
		public Dropdown settings;
		public LoginTCPUI loginTCPUI;

		private GameType type;
		private SceneField scene;
		private PreGamePlayer[] currentPlayers;


		void Start(){
			if (hasInit)
				return;
			hasInit = true;
			TCPLocalConnection.subscribeToTCPStatus (localBotStatusChanged);
			ClientUIOverlord.onUIStateChanged += (ClientUIStates newState) => {if(newState != ClientUIStates.PreGame)gameObject.SetActive(false);};
		}


		public virtual void initGameLobby(string gameTittle, Sprite gameSprite, GameType type, SceneField scene){
			gameObject.SetActive (true);
			initUserPanel ();
			loginTCPUI.startServerClicked (true);
			setReadyButtons ();
			gameImage.sprite = gameSprite;
			this.scene = scene;
		}

		public void startButtonClicked(){
			Msf.Connection.SendMessage((short)ServerCommProtocl.StartPreGame, new PreGameStartMsg(){isSinglePlayer = true});
			ClientPlayersHandler.resetLocalPLayers ();
			gameObject.SetActive (false);
			SceneManager.LoadScene (scene.SceneName);
		}

		private void initUserPanel(){
			AccountInfoPacket p = ClientUIOverlord.getCurrentAcountInfo ();
			int iconNumber = int.Parse (p.Properties ["icon"]);
			playerSlot.setUserPanel (iconNumber, p.Username, false);
		}

		private void setReadyButtons(){
			bool status = TCPLocalConnection.currentState == ConnectionStatus.Connected;
			startButton.interactable = status;
			playerSlot.setUserReady (status);
		}


		protected void localBotStatusChanged(ConnectionStatus status){
			if (ClientUIOverlord.currentState != ClientUIStates.PreGame)
				return;
			setReadyButtons ();
		}	

		public void onExitClick(){
			ClientUIStateManager.requestGotoGameLobby ();
			gameObject.SetActive (false);
		}
	}
}