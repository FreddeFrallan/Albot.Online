using System.Collections.Generic;
using System.ComponentModel;
using Barebones.Networking;
using Barebones.Utils;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Linq;
using Barebones.MasterServer;
using AlbotServer;
using Game;
using TMPro;

namespace AdminUI{

	public class AdminGameSelection : MonoBehaviour{
		private GenericUIList<GameInfoPacket> items;

        public AdminUpdateManager updateManager;
        public Button GameJoinButton;
		public AdminListUiItem ItemPrefab;
		public LayoutGroup LayoutGroup;
		public TextMeshProUGUI currentGames, totalGames;
		private string selectedID = "";
		private bool waitingForPreGame = false;

		// Use this for initialization
		protected virtual void Awake(){
			items = new GenericUIList<GameInfoPacket>(ItemPrefab.gameObject, LayoutGroup);
			Msf.Connection.SetHandler ((short)ServerCommProtocl.LobbyGameStats, handleGameStatsUpdate);
		}

		public void Setup(IEnumerable<GameInfoPacket> data){
			items.Generate<AdminListUiItem>(data, (packet, item) => {item.Setup(packet); });
			UpdateGameJoinButton();
		}


		public void Select(AdminListUiItem gamesListItem){
			items.Iterate<AdminListUiItem>(item => { item.SetIsSelected(!item.IsSelected && (gamesListItem == item)); });
			if (GetSelectedItem () != null)
				selectedID = GetSelectedItem ().GameId;
			UpdateGameJoinButton();
		}

		protected virtual void RequestRooms(){
			if (!Msf.Connection.IsConnected){
				Logs.Error("Tried to request rooms, but no connection was set");
				return;
			}
			Msf.Connection.SendMessage ((short)CustomMasterServerMSG.requestSpectatorGames, ((r, m) => {
				if(r == ResponseStatus.Success)
					Setup(m.DeserializeList(() => new GameInfoPacket()).ToList());
			}));
		}

		private void UpdateGameJoinButton(){
			AdminListUiItem item = GetSelectedItem ();
			if (item == null && string.IsNullOrEmpty(selectedID) == false) { item = items.FindObject<AdminListUiItem> ((x) => {return x.GameId == selectedID; });
				if (item != null)
					Select (item);
				else {
					selectedID = "";
					GameJoinButton.interactable = false;
				}
			}else
				GameJoinButton.interactable = GetSelectedItem() != null;
		}

		public void OnJoinGameClick(){
			GameType selectedGameType = GameType.None;
			AdminListUiItem selected = GetSelectedItem();
			if (selected == null || extractSelectedGameType(selected, ref selectedGameType) == false)
				return;

            AdminUpdateManager.startSpectateGame(selected.GameId);
        }


		private bool extractSelectedGameType(AdminListUiItem item, ref GameType type){
			if (item.RawData.Properties.ContainsKey (MsfDictKeys.GameType) == false) {
				print ("Does not contain key: " + MsfDictKeys.GameType);
				return false;
			}

			type = GameUtils.stringToGameType(item.RawData.Properties [MsfDictKeys.GameType]);
			return type != GameType.None;
		}
			
		private void handleGameStatsUpdate(IIncommingMessage message){
			LobbyGameStatsMsg msg = message.Deserialize<LobbyGameStatsMsg> ();
			currentGames.text = msg.currentActiveGames.ToString();
			totalGames.text = msg.totalGamesPlayed.ToString ();
		}



			
		private void OnEnable(){if (Msf.Connection.IsConnected) RequestRooms();}
		public void OnRefreshClick(){RequestRooms();}
		public AdminListUiItem GetSelectedItem(){return items.FindObject<AdminListUiItem>(item => item.IsSelected);}
	}
		
}