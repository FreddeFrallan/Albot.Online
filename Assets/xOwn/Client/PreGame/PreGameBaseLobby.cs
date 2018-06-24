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
		public TMP_Dropdown p2Settings;
		public LoginTCPUI loginTCPUI;

        protected bool isAdmin;
        protected string roomId;
        protected int playerId;
		protected GameType type;
		private PreGameSlotInfo[] currentPlayers;

		protected List<IPacketHandler> handlers = new List<IPacketHandler>();


        #region Visuals
        public virtual void initPreGameLobby(Sprite gameSprite, PreGameRoomMsg roomInfo){
			gameObject.SetActive (true);
			gameCreator.setCurrentPreLobby (this);

			this.type = roomInfo.specs.type;
			this.roomId = roomInfo.specs.roomID;
			this.currentPlayers = roomInfo.players;
			this.gameImage.sprite = gameSprite;
            this.gameTitle.SetText(roomInfo.specs.type.ToString());
            isAdmin = roomInfo.specs.hostName == ClientUIOverlord.getCurrentAcountInfo().Username;

            setPlayerSlots (roomInfo.players);
			setAdminValues (isAdmin);	
			extractLocalPlayerSlotId (roomInfo.players);
			loginTCPUI.startServerClicked (true);
			handlers.Add(Msf.Connection.SetHandler ((short)ServerCommProtocl.UpdatePreGame, updatePreGameLobby));
            CurrentGame.setNewCurrentPreGame(roomInfo.specs);
		}
			
	
		private void setPlayerSlots(PreGameSlotInfo[] players){
			for (int i = 0; i < players.Length; i++)
				initUserPanel (players [i], i);
		}
		private void initUserPanel(PreGameSlotInfo p, int index){
			if (p.type != PreGameSlotType.Empty)
				playerSlots [index].setUserPanel (p.playerInfo.iconNumber, p.playerInfo.username, p.isReady);
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
			startButton.interactable = new List<PreGameSlotInfo> (currentPlayers).TrueForAll (x => x.isReady);
		}
		private void extractLocalPlayerSlotId(PreGameSlotInfo[] players){
			AccountInfoPacket p = ClientUIOverlord.getCurrentAcountInfo ();
			for (int i = 0; i < players.Length; i++)
				if (players [i].playerInfo.username == p.Username)
					playerId = i;
		}
		#endregion



		protected void updatePreGameLobby(IIncommingMessage rawMsg){
            PreGameRoomMsg msg = rawMsg.Deserialize<PreGameRoomMsg> ();
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
			Msf.Connection.SendMessage((short)ServerCommProtocl.PlayerLeftPreGame, roomId);
			removeHandlers ();
			ClientUIStateManager.requestGotoGameLobby ();
			gameObject.SetActive (false);
		}

		protected void localBotStatusChanged(ConnectionStatus status){
			if (ClientUIOverlord.currentState != ClientUIStates.PreGame)
				return;

            bool isReady = status == ConnectionStatus.Connected;
            Msf.Connection.SendMessage((short)ServerCommProtocl.UpdatePreGame, new PreGameReadyUpdate(){isReady = isReady, roomID = roomId});
		}

		public void removeHandlers(){
			foreach (IPacketHandler h in handlers)
				Msf.Connection.RemoveHandler (h);
			handlers.Clear ();
		}


		//Called from "onJoinStartedGame" in GameConnectorUI
		public virtual void setLocalPreGamePlayers (){ ClientPlayersHandler.resetLocalPLayers ();}
        public virtual void onStartClick() {
            print("Sending from base Lobby");
            Msf.Connection.SendMessage((short)ServerCommProtocl.StartPreGame, roomId, handleStartGameResponse);
        }
        private void handleStartGameResponse(ResponseStatus status, IIncommingMessage rawMsg) {
            if (status != ResponseStatus.Success) {
                Debug.LogError(rawMsg.AsString());
                Msf.Events.Fire(Msf.EventNames.ShowDialogBox, DialogBoxData.CreateError(rawMsg.AsString()));
            } 
        }


        #region AnneHacks

        public void setPlayerReady(bool value) {
            currentPlayers[0].isReady = value;
            setAdminStartButton();
        }

        #endregion
    }

}