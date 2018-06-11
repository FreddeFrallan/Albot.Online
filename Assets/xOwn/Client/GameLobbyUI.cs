using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLobbyUI : MonoBehaviour {

	public GameObject preGameLobby, preTrainingLobby, lobbyBrowser, noGameSelectedView;

	public void closeLobby(){
		preGameLobby.SetActive (false);
		preTrainingLobby.SetActive (false);
        lobbyBrowser.SetActive(false);
        noGameSelectedView.SetActive(true);
    }

    public void openLobbyBrowser() {
        preGameLobby.SetActive(false);
        preTrainingLobby.SetActive(false);
        noGameSelectedView.SetActive(false);
        lobbyBrowser.SetActive(true);
    }

    public void closeLobbyBrowser() {
        lobbyBrowser.SetActive(false);
        noGameSelectedView.SetActive(true);
    }
}
