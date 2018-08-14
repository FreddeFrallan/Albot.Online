using UnityEngine;
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
        public Color[] defaultColor, tournamentColor;
        private Color[] currentColor;
		public ClientUI.GameSelectionUI ListView;
        public TextMeshProUGUI MapName;
        public TextMeshProUGUI Name;
        public TextMeshProUGUI Online;

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
            SetIsSelected(false);
        }

        public void SetIsSelected(bool isSelected){
            IsSelected = isSelected;
            BgImage.color = isSelected ? currentColor[1] : currentColor[0];
        }

        public void Setup(GameInfoPacket data){
            RawData = data;
            IsLobby = data.infoType == GameInfoType.Lobby;
            roomType = data.infoType;
            Name.text = data.Name;
            GameId = data.Id;
            initBackgroundColor(data.infoType);


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

        private void initBackgroundColor(GameInfoType type) {
            if (type == GameInfoType.PreTournament)
                currentColor = tournamentColor;
            else
                currentColor = defaultColor;

            SetIsSelected(false);
        }
    }
}