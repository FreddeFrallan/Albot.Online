using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientUI {

    public class GameLobbyUI : MonoBehaviour {

        public GameObject preGameLobby, preTrainingLobby, lobbyBrowser, noGameSelectedView;
        public AutoRefresh refresher;

        private bool isOpen = false;

        public void closeLobby() {
            preGameLobby.SetActive(false);
            preTrainingLobby.SetActive(false);
            lobbyBrowser.SetActive(false);
            noGameSelectedView.SetActive(true);
        }


        #region Buttons
        public void gotoLobbyBrowserButton() {ClientUIStateManager.requestGotoState(ClientUIStates.LobbyBrowser);}
        public void gotoGameLobbyButton() { ClientUIStateManager.requestGotoState(ClientUIStates.GameLobby); }
        #endregion


        private void openLobbyBrowser() {
            lobbyBrowser.SetActive(true);
            refresher.activateRefresh();

            isOpen = true;
        }

        private void closeLobbyBrowser() {
            lobbyBrowser.SetActive(false);
            refresher.deActivateRefresh();

            isOpen = false;
        }

        public void setLobbyBrowserState(bool active) {
            if (active && isOpen == false)
                openLobbyBrowser();
            else if (active == false && isOpen)
                closeLobbyBrowser();
        }

    }
}