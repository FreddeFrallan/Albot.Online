using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlbotServer;
using UnityEngine.UI;
using Barebones.MasterServer;
using Game;
using Barebones.Networking;
using TMPro;

namespace ClientUI{

	public abstract class PreGameBaseLobby : MonoBehaviour {
		protected static bool hasInit = false;
		public GameConnectorUI gameCreator;
		public GameSelectionUI gameSelection;
		public Image gameImage;
        public TextMeshProUGUI gameTitle;
		public PreGamePlayerSlot[] playerSlots;
		public Button startButton;
		public Dropdown p2Settings;
		public LoginTCPUI loginTCPUI;
		
		protected bool isAdmin, isReady = false;
		protected int roomId, playerId;
		protected GameType type;
		private PreGamePlayer[] currentPlayers;

		protected List<IPacketHandler> handlers = new List<IPacketHandler>();


		#region Visuals
		public virtual void initPreGameLobby(string gameTitle, Sprite gameSprite, PreGamePlayer[] players, bool isAdmin, int roomId, GameType type){
			gameObject.SetActive (true);
			gameCreator.setCurrentPreLobby (this);

			this.type = type;
			this.roomId = roomId;
			this.currentPlayers = players;
			this.gameImage.sprite = gameSprite;
            this.gameTitle.SetText(gameTitle);
			setPlayerSlots (players);
			setAdminValues (isAdmin);	
			extractLocalPlayerSlotId (players);
			loginTCPUI.startServerClicked (true);
			handlers.Add(Msf.Connection.SetHandler ((short)ServerCommProtocl.UpdatePreGame, updatePreGameLobby));
		}
			
	
		private void setPlayerSlots(PreGamePlayer[] players){
			for (int i = 0; i < players.Length; i++) {
				initUserPanel (players [i], i);
			}
		}
		private void initUserPanel(PreGamePlayer p, int index){
			if (p.type != PreGameSlotType.Empty)
				playerSlots [index].setUserPanel (p.info.iconNumber, p.info.username, p.isReady);
			else 
				playerSlots [index].startClearPanel ();
		}
		private void setAdminValues(bool isAdmin){
			this.isAdmin = isAdmin;
			p2Settings.interactable = isAdmin;
			startButton.interactable  = false;
			p2Settings.value = 0;
		}

		private void setAdminStartButton(){
			if (isAdmin == false)
				return;
			startButton.interactable = new List<PreGamePlayer> (currentPlayers).TrueForAll (x => x.isReady);
		}
		private void extractLocalPlayerSlotId(PreGamePlayer[] players){
			AccountInfoPacket p = ClientUIOverlord.getCurrentAcountInfo ();
			for (int i = 0; i < players.Length; i++)
				if (players [i].info.username == p.Username)
					playerId = i;
		}
		#endregion



		protected void updatePreGameLobby(IIncommingMessage message){
			AlbotServer.PreGameRoomMsg msg = message.Deserialize<AlbotServer.PreGameRoomMsg> ();
			currentPlayers = msg.players;
			setPlayerSlots (msg.players);
			setAdminStartButton();
		}
		protected void resetPanel(){
			isAdmin = false;
			p2Settings.value = 0;
			p2Settings.Hide ();
		}

	
		public void onExitClick(){
			Msf.Connection.SendMessage((short)ServerCommProtocl.PlayerLeftPreGame, new PlayerInfoMsg());
			removeHandlers ();
			ClientUIStateManager.requestGotoGameLobby ();
			gameObject.SetActive (false);
		}


		protected void localBotStatusChanged(ConnectionStatus status){
			if (ClientUIOverlord.currentState != ClientUIStates.PreGame)
				return;

			bool preValue = isReady;
			if (isReady && status != ConnectionStatus.Connected)
				isReady = false;
			else if (isReady == false && status == ConnectionStatus.Connected)
				isReady = true;

			if (preValue != isReady) {
				Msf.Connection.SendMessage((short)ServerCommProtocl.UpdatePreGame, new PreGameReadyUpdate(){isReady = isReady, roomID = roomId});
				currentPlayers [playerId].isReady = isReady;
				setPlayerSlots (currentPlayers);
				setAdminStartButton();
			}
		}

		public void removeHandlers(){
			foreach (IPacketHandler h in handlers)
				Msf.Connection.RemoveHandler (h);
			handlers.Clear ();
		}



		//Called from "onJoinStartedGame" in GameConnectorUI
		public virtual void setLocalPreGamePlayers (){ ClientPlayersHandler.resetLocalPLayers ();}
		abstract public void onStartClick();
	}

}