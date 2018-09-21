using System.Collections;
using System.Collections.Generic;
using Barebones.MasterServer;
using Barebones.Networking;
using UnityEngine;
using UnityEngine.UI;

public class LocalBotConnectionStatusUi : MonoBehaviour {

	public static ConnectionStatus currentStatus = ConnectionStatus.Disconnected;
	public ClientConnectionStatusUi ClientConnColors;

    public Image dotImage;
	public Image waitingImage;
	public Image connectedImage;
	public Text text;
	public static LocalBotConnectionStatusUi instance;


	[SerializeField]
	private GameObject extendedObject;
	[SerializeField]
	private Text extendedText;


	void Start(){		
		if (ClientUI.ClientUIOverlord.hasLoaded)return;
		instance = this;
		//TCPLocalConnection.TCPStatusChanged += listenForTCPUpdate;
		TCPLocalConnection.subscribeToTCPStatus (UpdateStatusView);
		UpdateStatusView (ConnectionStatus.Disconnected);
	}

	private void UpdateStatusView(ConnectionStatus status){
		currentStatus = status;
        print(currentStatus);
        switch (status) {
            case ConnectionStatus.Connected:
                toggleWaitingImage(false);
                if (instance.dotImage != null)
                    instance.dotImage.color = ClientConnColors.OnlineColor;
                break;
            case ConnectionStatus.Disconnected:
            case ConnectionStatus.None:
                toggleWaitingImage(true);
                if (instance.dotImage != null)
                    instance.dotImage.color = ClientConnColors.OfflineColor;
                break;
            case ConnectionStatus.Connecting:
                toggleWaitingImage(true);
                if (instance.dotImage != null)
                    instance.dotImage.color = ClientConnColors.ConnectingColor;
                break;
            default:
                toggleWaitingImage(true);
                if (instance.dotImage != null)
                    instance.dotImage.color = ClientConnColors.UnknownColor;
                break;
        }
    }

    public void toggleWaitingImage(bool onOff) {
        if (instance.waitingImage != null && instance.connectedImage != null) {
            instance.waitingImage.enabled = onOff;
            instance.connectedImage.enabled = !onOff;
        }
    }

	public void toggleExtendedInfo(){
		extendedObject.SetActive (!extendedObject.activeSelf);
	}

	public void updateExtendedInfo(string portNumber){
		extendedText.text = "Port:  " + portNumber;
	}

}






