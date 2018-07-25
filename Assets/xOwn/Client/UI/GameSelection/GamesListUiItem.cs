﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Barebones.MasterServer{
    /// <summary>
    ///     Represents a single row in the games list
    /// </summary>
    public class GamesListUiItem : MonoBehaviour{
        public GameInfoPacket RawData { get; protected set; }
        public Image BgImage;
        public GameInfoType roomType;
        public Color DefaultBgColor;
		public ClientUI.GameSelectionUI ListView;
        public TextMeshProUGUI MapName;
        public TextMeshProUGUI Name;
        public TextMeshProUGUI Online;

        public Color SelectedBgColor;

        public string UnknownMapName = "Unknown";

        public string GameId { get; set; }
        public bool IsSelected { get; private set; }
        public bool IsLobby { get; private set; }

        public bool IsPasswordProtected{
            get { return RawData.IsPasswordProtected; }
        }

        // Use this for initialization
        private void Awake(){
            BgImage = GetComponent<Image>();
            DefaultBgColor = BgImage.color;

            SetIsSelected(false);
        }

        public void SetIsSelected(bool isSelected){
            IsSelected = isSelected;
            BgImage.color = isSelected ? SelectedBgColor : DefaultBgColor;
        }

        public void Setup(GameInfoPacket data){
            RawData = data;
            IsLobby = data.infoType == GameInfoType.Lobby;
            roomType = data.infoType;
            SetIsSelected(false);
            Name.text = data.Name;
            GameId = data.Id;


            if (data.MaxPlayers > 0)
                Online.text = string.Format("{0}/{1}", data.OnlinePlayers, data.MaxPlayers);
            else
                Online.text = data.OnlinePlayers.ToString();

            MapName.text = data.Properties.ContainsKey(MsfDictKeys.MapName) 
                ? data.Properties[MsfDictKeys.MapName] : UnknownMapName;
        }

        public void OnClick(){
            ListView.Select(this);
        }
    }
}