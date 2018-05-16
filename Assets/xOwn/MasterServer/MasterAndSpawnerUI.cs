using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Barebones.MasterServer;

public class MasterAndSpawnerUI : MonoBehaviour {

	public string defaultPort, defaultGameServer;
	public InputField portField, gameServerField;
	public Button masterButton, spawnerButton;

	public SpawnerBehaviour spawnerBehaviour;
	public MasterServerBehaviour masterBehaviour;

	// Use this for initialization
	void Start () {
		portField.text = defaultPort;
		gameServerField.text = defaultGameServer;
	}

	void Update(){
		if (spawnerBehaviour.IsSpawnerStarted) {
			this.enabled = false;
			spawnerButton.gameObject.SetActive(false);
		}
	}

	public void startMasterPressed(){
		masterButton.interactable = false;
		masterBehaviour.StartServer(int.Parse(portField.text));
	}

	public void startSpawnerPressed(){
		if (!Msf.Connection.IsConnected){
			Logs.Error("You must first connect to master");

			// Show a dialog box with error
			Msf.Events.Fire(Msf.EventNames.ShowDialogBox, 
				DialogBoxData.CreateError("You must first connect to master"));
			return;
		}

		// Set the default executable path
		// It's called "Default", because it's used if "-msfExe" 
		// argument doesn't override it (change it)
		spawnerBehaviour.DefaultExePath = gameServerField.text;

		// Make sure that exe path is not overriden in editor
		spawnerBehaviour.OverrideExePathInEditor = false;

		// Set default machine IP
		spawnerBehaviour.DefaultMachineIp = "127.0.0.1";

		// Start the spawner
		spawnerBehaviour.StartSpawner();
	}
}
