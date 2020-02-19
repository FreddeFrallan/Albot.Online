using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClientUI{


	public class MenuBar : MonoBehaviour {
		[SerializeField]
		private Button menuButton = null, infoButton = null, training = null;
		[SerializeField]
		private GameObject masterConnection;
		[SerializeField]
		private GameObject localConnection;

		public MaskDropdown localDropdown, serverDropdown;
		public static GameObject spawnedMenuBar;

		void Awake(){
			if (ClientUI.ClientUIOverlord.hasLoaded)return;
			ClientUIOverlord.onUIStateChanged += UIStateChanged;
		}
		void Start(){
			if (ClientUI.ClientUIOverlord.hasLoaded)return;

			if (spawnedMenuBar == null) {
				spawnedMenuBar = this.gameObject;
				DontDestroyOnLoad (this.gameObject);
			}
			if (spawnedMenuBar != this.gameObject)
				Destroy (this.gameObject);
		}


		public void rollUpDropdowns(){
			localDropdown.turnOff ();
		}

		public void UIStateChanged(ClientUIStates newState){
			if (newState == ClientUIStates.LoginMenu) stateSwitch (MenuBarState.noInfoNorProfile);
			else if (newState == ClientUIStates.GameLobby) stateSwitch (MenuBarState.noInfo);
			else stateSwitch (MenuBarState.allActive);
		}

		private void stateSwitch(MenuBarState newState){
			if (newState == MenuBarState.allActive)
				setState (true, true, true);
			else if (newState == MenuBarState.noButtons)
				setState (false, false, false);
			else if (newState == MenuBarState.noProfile)
				setState (true, true, false);
			else if (newState == MenuBarState.noInfo)
				setState (true, false, true);
			else if (newState == MenuBarState.noInfoNorProfile)
				setState (true, false, false);
		}
		private void setState(bool mainMenu, bool info, bool train){
			menuButton.interactable = (mainMenu);
			infoButton.interactable = (false);
			training.interactable = (train);
		}

	}




	public enum MenuBarState{
		allActive,
		noButtons,
		noProfile,
		noInfo,
		noInfoNorProfile
	}




}