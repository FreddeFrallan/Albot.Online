using System.Collections.Generic;
using System.ComponentModel;
using Barebones.Networking;
using Barebones.Utils;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using Barebones.MasterServer;
using AlbotServer;

namespace ClientUI{

	public class GameSelectionUI : MonoBehaviour{
        private GenericUIList<GameInfoPacket> _items;
		private static GameSelectionUI localSingleton;

        public Button GameJoinButton;
        public GamesListUiItem ItemPrefab;
        public LayoutGroup LayoutGroup;
		public Dropdown newGameDropdown, newTrainingDropdown;
		public NewGameCreator gameCreator;
		public List<MapSelection> maps;
        protected IClientSocket Connection = Msf.Connection;
		private string selectedID = "";
		public Text currentGames, totalGames;

        // Use this for initialization
        protected virtual void Awake(){
			localSingleton = this;
			_items = new GenericUIList<GameInfoPacket>(ItemPrefab.gameObject, LayoutGroup);
			ClientUIOverlord.onUIStateChanged += (ClientUIStates newState) => {if (newState == ClientUIStates.GameLobby)resetSelection ();};
			Connection.SetHandler ((short)ServerCommProtocl.LobbyGameStats, handleGameStatsUpdate);
		}
			
        public void Setup(IEnumerable<GameInfoPacket> data){
            _items.Generate<GamesListUiItem>(data, (packet, item) => { item.Setup(packet); });
            UpdateGameJoinButton();
        }


        public void Select(GamesListUiItem gamesListItem){
			_items.Iterate<GamesListUiItem>(item => { item.SetIsSelected(!item.IsSelected && (gamesListItem == item)); });
			if (GetSelectedItem () != null)
				selectedID = GetSelectedItem ().GameId;
            UpdateGameJoinButton();
        }
			
        protected virtual void RequestRooms(){
            if (!Connection.IsConnected){
                Logs.Error("Tried to request rooms, but no connection was set");
                return;
            }
            Msf.Client.Matchmaker.FindGames(games =>{Setup(games);});
        }

		private void UpdateGameJoinButton(){
			GamesListUiItem item = GetSelectedItem ();
			if (item == null && string.IsNullOrEmpty(selectedID) == false) { item = _items.FindObject<GamesListUiItem> ((x) => {return x.GameId == selectedID; });
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
			GamesListUiItem selected = GetSelectedItem();
			if (selected == null)
				return;
			gameCreator.joinLobbyGame (selected);
		}
			
		public void newGameSelected(bool isTraining){
			if ((newGameDropdown.value == 0 && !isTraining) || (isTraining && newTrainingDropdown.value == 0))
				return;

			MapSelection selectedMap = getSelectedMap (isTraining ? newTrainingDropdown.value : newGameDropdown.value);
			resetSelection ();
			gameCreator.createNewGame (selectedMap);
		}
		private void handleGameStatsUpdate(IIncommingMessage message){
			LobbyGameStatsMsg msg = message.Deserialize<LobbyGameStatsMsg> ();
			currentGames.text = msg.currentActiveGames.ToString();
			totalGames.text = msg.totalGamesPlayed.ToString ();
		}

		private void resetSelection(){
			newGameDropdown.value = 0;
			newGameDropdown.Hide ();
			newTrainingDropdown.value = 0;
			newTrainingDropdown.Hide ();
		}


		private MapSelection getSelectedMap(int selectedValue){return maps [selectedValue - 1];}
		public static MapSelection getMatchingMapSelection(Game.GameType type){return localSingleton.maps.Find (x => x.type == type);}
		private void OnEnable(){if (Connection.IsConnected) RequestRooms();}
		public void OnRefreshClick(){RequestRooms();}
		public GamesListUiItem GetSelectedItem(){return _items.FindObject<GamesListUiItem>(item => item.IsSelected);}
    }

	[Serializable]
	public class MapSelection{
		public int maxPlayers;
		public string Name;
		public SceneField Scene;
		public Sprite picture;
		public Game.GameType type;
		public bool isRealTime = false;
	}
}