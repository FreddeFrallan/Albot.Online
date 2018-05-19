using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Barebones.Networking;
using TMPro;

namespace ClientUI{
	
	public class LoginTCPUI : MonoBehaviour {
		public TMP_InputField portNumber;
		public TextMeshProUGUI errorText;

		private bool canOpenPort = true;

		public GameObject startButton, stopButton;
		public TextMeshProUGUI abortText;
		public LocalBotConnectionStatusUi connectionUI;
		public Color connected, connecting, offline;
        public Image statusDot;
		private ConnectionStatus newStatus;

		void Start(){
			TCPLocalConnection.subscribeToTCPStatus( listenForLocalTCPChange);
		}

		void Update(){
			newStatus = TCPLocalConnection.currentState;
			setLocalTCPStatus ();
		}

		public void startServerClicked(bool forceOpen = false){
			if (canOpenPort == false && forceOpen == false)
				return;

			LocalConnectionStatus status = TCPLocalConnection.startServer (int.Parse (portNumber.text));
			if (status == LocalConnectionStatus.portIsBusy)
				errorText.text = "This port is already in use! Try another...";
			else if(status == LocalConnectionStatus.otherPortError)
				errorText.text = "Could not start a TCP connection on this port!";
		}


		public void stopServerClicked(){TCPLocalConnection.stopServer ();}
		public void listenForLocalTCPChange(ConnectionStatus status){newStatus = status;	}

		private void setLocalTCPStatus(){
			setStatusIcon (newStatus);
			settButtons (newStatus);
			portNumber.interactable = newStatus == ConnectionStatus.Disconnected || newStatus == ConnectionStatus.None;
			canOpenPort = newStatus == ConnectionStatus.Disconnected || newStatus == ConnectionStatus.None;
			errorText.text = newStatus == ConnectionStatus.Connecting ? "" : errorText.text;
			string dropdownPortNumber = newStatus == ConnectionStatus.Connected || newStatus == ConnectionStatus.Connecting ? portNumber.text : "";
			connectionUI.updateExtendedInfo (dropdownPortNumber);
		}

		private void settButtons(ConnectionStatus state){
			stopButton.SetActive (state == ConnectionStatus.Connected || state == ConnectionStatus.Connecting);
			startButton.SetActive (state == ConnectionStatus.Disconnected || state == ConnectionStatus.None);
			abortText.text = state == ConnectionStatus.Connected ? "Close Port" : "Abort";
		}

		private void setStatusIcon(ConnectionStatus state){
            if (state == ConnectionStatus.Connected)
                statusDot.color = connected;

            if(state == ConnectionStatus.Connecting)
                statusDot.color = connecting;

            if (state == ConnectionStatus.Disconnected || state == ConnectionStatus.None)
                statusDot.color = offline;
        }
	}

}