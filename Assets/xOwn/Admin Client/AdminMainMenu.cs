using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ClientUI;
using Barebones.MasterServer;
using UserData;

namespace AdminUI{

	public class AdminMainMenu : MonoBehaviour {

		[SerializeField]
		private Button gameLobbyButton = null, logoutButton = null, statsButton = null;
		[SerializeField] private GameObject fadePanel;

		public void init(){
			AdminUIManager.onAdminUIStateChanged += UIStateChanged;
			UIStateChanged (ClientUIStates.LoginMenu);
		}


		#region Logout
		public void onLogoutPressed(){
			AdminUIManager.requestLogout ();
			setActive (false);
		}

		public void onGameLobbyPressed(){
            AdminUIManager.requestGotoState(ClientUIStates.GameLobby);
			setActive (false);
		}
        #endregion

        #region Stats
        public void onStatsPressed() {
            Msf.Connection.SendMessage((short)CustomMasterServerMSG.requestLoginData, (s, r) => {
                print(s);
                
                LoginDataODT odt = r.Deserialize<LoginDataODT>();
                print(odt.entries.Length);
                foreach (UserLoginEntryStruct l in odt.entries) {
                    print(l.username + " " + l.time);
                }
            });
            //AdminUIManager.requestGotoState(ClientUIStates.Stats);
            setActive(false);
        }
        #endregion


        public void onExitPressed(){Application.Quit ();}
		public void setActive(bool active){
			this.gameObject.SetActive (active);
			fadePanel.SetActive (active);
		}

		private void UIStateChanged(ClientUIStates state){
			logoutButton.gameObject.SetActive(state != ClientUIStates.LoginMenu);
			gameLobbyButton.gameObject.SetActive(state == ClientUIStates.PlayingGame || state == ClientUIStates.Stats);
            statsButton.gameObject.SetActive(state != ClientUIStates.LoginMenu || state == ClientUIStates.Stats);
        }

    }
}