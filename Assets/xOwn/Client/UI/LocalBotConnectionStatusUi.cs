using System.Collections;
using System.Collections.Generic;
using Barebones.MasterServer;
using Barebones.Networking;
using UnityEngine;
using UnityEngine.UI;

public class LocalBotConnectionStatusUi : MonoBehaviour {

	public static ConnectionStatus currentStatus = ConnectionStatus.Disconnected;
	public ClientConnectionStatusUi ClientConnColors;

	public Image image;
	public Image extendedImage;
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

        switch (status){
            case ConnectionStatus.Connected:
				if (instance.image != null)
					instance.image.color = ClientConnColors.OnlineColor;
			
				//instance.text.color = ClientConnColors.OnlineColor;
				//instance.text.text = "Connected";
                break;
            case ConnectionStatus.Disconnected:
			case ConnectionStatus.None:
				if (instance.image != null) 
					instance.image.color = ClientConnColors.OfflineColor;
			
				//instance.text.color = ClientConnColors.OfflineColor;
				//instance. text.text = "Offline";
                break;
            case ConnectionStatus.Connecting:
				if (instance.image != null) 
					instance.image.color = ClientConnColors.ConnectingColor;
			
				//instance.text.color = ClientConnColors.ConnectingColor;
				//instance.text.text = "Connecting";
                break;
            default:
				if (instance.image != null) 
					instance.image.color = ClientConnColors.UnknownColor;
			
				//instance.text.color = ClientConnColors.UnknownColor;
				//instance.text.text = "Unknown";
                break;
        }

		instance.extendedImage.color = instance.image.color;
    }

	public void toggleExtendedInfo(){
		extendedObject.SetActive (!extendedObject.activeSelf);
	}

	public void updateExtendedInfo(string portNumber){
		extendedText.text = "Port:  " + portNumber;
	}

}






