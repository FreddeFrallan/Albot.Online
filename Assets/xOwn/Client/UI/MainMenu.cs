using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ClientUI{

	public class MainMenu : MonoBehaviour {
		[SerializeField]
		private Button gameLobbyButton = null, logoutButton = null;
		[SerializeField] private GameObject fadePanel;

		public void init(){
			ClientUIOverlord.onUIStateChanged += UIStateChanged;
			UIStateChanged (ClientUIStates.LoginMenu);
		}


		#region Logout
		public void onLogoutPressed(){
			ClientUIStateManager.requestLogout ();
			setActive (false);
		}

		public void onGameLobbyPressed(){
			ClientUIStateManager.requestGotoGameLobby ();
			setActive (false);
		}
		#endregion

		public void onExitPressed(){Application.Quit ();}
		public void setActive(bool active){
			this.gameObject.SetActive (active);
			fadePanel.SetActive (active);
		}
		
		private void UIStateChanged(ClientUIStates state){
			logoutButton.gameObject.SetActive(state != ClientUIStates.LoginMenu);
			gameLobbyButton.gameObject.SetActive(state == ClientUIStates.PlayingGame);
		}
	}

}