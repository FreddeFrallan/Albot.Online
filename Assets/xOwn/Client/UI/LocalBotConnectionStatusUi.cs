using System.Collections;
using System.Collections.Generic;
using Barebones.MasterServer;
using Barebones.Networking;
using UnityEngine;
using UnityEngine.UI;

public class LocAlbotConnectionStatusUi : MonoBehaviour {

	public static ConnectionStatus currentStatus = ConnectionStatus.Disconnected;
	public ClientConnectionStatusUi ClientConnColors;

    public Image dotImage;
	public Image waitingImage;
	public Image connectedImage;
    public Image disconnectedImage;
    public Text text;
	public static LocAlbotConnectionStatusUi instance;


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
                toggleImages(false, true, false);
                if (instance.dotImage != null)
                    instance.dotImage.color = ClientConnColors.OnlineColor;
                break;
            case ConnectionStatus.Disconnected:
            case ConnectionStatus.None:
                toggleImages(false, false, true);
                if (instance.dotImage != null)
                    instance.dotImage.color = ClientConnColors.OfflineColor;
                break;
            case ConnectionStatus.Connecting:
                toggleImages(true, false, false);
                if (instance.dotImage != null)
                    instance.dotImage.color = ClientConnColors.ConnectingColor;
                break;
            default:
                toggleImages(true, false, false);
                if (instance.dotImage != null)
                    instance.dotImage.color = ClientConnColors.UnknownColor;
                break;
        }
    }

    public void toggleImages(bool waitImg, bool connImg, bool dcImg) {
        if (instance.waitingImage != null)
            instance.waitingImage.enabled = waitImg;

        if (instance.connectedImage != null) 
            instance.connectedImage.enabled = connImg;

        if (instance.disconnectedImage != null)
            instance.disconnectedImage.enabled = dcImg;
    }

	public void toggleExtendedInfo(){
		extendedObject.SetActive (!extendedObject.activeSelf);
	}

	public void updateExtendedInfo(string portNumber){
		extendedText.text = "Port:  " + portNumber;
	}

}






