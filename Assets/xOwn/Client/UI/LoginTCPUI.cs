using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Barebones.Networking;
using TMPro;

namespace ClientUI{
	
	public class LoginTCPUI : MonoBehaviour {
		public TMP_InputField portNumber;
		public TextMeshProUGUI infoText;
        public Color errorColor, connectingColor;

		private bool canOpenPort = true;

		public GameObject startButton, stopButton;
		public TextMeshProUGUI abortText;
		public LocalBotConnectionStatusUi connectionUI;
		public Color connected, connecting, offline;
        public Image statusDot;
		private ConnectionStatus newStatus;

        private bool errorOpeningPort = false;

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
            errorOpeningPort = status != LocalConnectionStatus.connecting;
            if (status != LocalConnectionStatus.connecting)
                displayPortError(status);
		}


		public void stopServerClicked(){TCPLocalConnection.stopServer ();}
		public void listenForLocalTCPChange(ConnectionStatus status){newStatus = status;}

		private void setLocalTCPStatus(){
			setStatusIcon (newStatus);
			settButtons (newStatus);
			portNumber.interactable = newStatus == ConnectionStatus.Disconnected || newStatus == ConnectionStatus.None;
			canOpenPort = newStatus == ConnectionStatus.Disconnected || newStatus == ConnectionStatus.None;
			string dropdownPortNumber = newStatus == ConnectionStatus.Connected || newStatus == ConnectionStatus.Connecting ? portNumber.text : "";
			connectionUI.updateExtendedInfo (dropdownPortNumber);

            displayConnectionStatus(newStatus);
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

        #region InfoText

        private void displayConnectionStatus(ConnectionStatus status) {
            if (errorOpeningPort)
                return;

            infoText.color = connectingColor;
            if (status == ConnectionStatus.Connecting)
                infoText.text = "Waiting for bot to connect...";
            else if(status == ConnectionStatus.Disconnected)
               infoText.text = "Open port to connect your bot...";
            else
                infoText.text = "Waiting for game to start...";
        }


        private void displayPortError(LocalConnectionStatus status) {
            infoText.color = errorColor;
            if (status == LocalConnectionStatus.portIsBusy)
                infoText.text = "This port is already in use! Try another...";
            else if (status == LocalConnectionStatus.otherPortError)
                infoText.text = "Could not start a TCP connection on this port!";
        }

        #endregion
    }

}