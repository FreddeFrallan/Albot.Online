using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Barebones.MasterServer;
using TMPro;

namespace AdminUI{

	public class AdminListUiItem : MonoBehaviour {
		public GameInfoPacket RawData { get; protected set; }
		public Image BgImage;
		public Color DefaultBgColor;
		public AdminGameSelection ListView;
		public GameObject LockImage;
		public TextMeshProUGUI MapName;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Online;
		public bool isPreGame;

		public Color SelectedBgColor;

		public string UnknownMapName = "Unknown";

		public string GameId { get; private set; }
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
			SetIsSelected(false);
			Name.text = data.Name;
			GameId = data.Id;
			LockImage.SetActive(data.IsPasswordProtected);

			isPreGame = bool.Parse (data.Properties [MsfDictKeys.IsPreGame]);

            /*
			if (data.MaxPlayers > 0)
				Online.text = string.Format("{0}/{1}", data.OnlinePlayers, data.MaxPlayers);
			else
				Online.text = data.OnlinePlayers.ToString();
             */
            MapName.text = "Snake";//data.Properties.ContainsKey(MsfDictKeys.MapName) ? data.Properties[MsfDictKeys.MapName] : UnknownMapName;
		}

		public void OnClick(){
			ListView.Select(this);
		}
	}
}