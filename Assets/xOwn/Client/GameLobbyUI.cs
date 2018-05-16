using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLobbyUI : MonoBehaviour {

	public GameObject preGameLobby, preTrainingLobby;

	public void closeLobby(){
		preGameLobby.SetActive (false);
		preTrainingLobby.SetActive (false);
	}
}
