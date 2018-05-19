using System.Collections;
using System.Collections.Generic;
using Barebones.MasterServer;
using Barebones.Networking;
using UnityEngine;
using UnityEngine.UI;

public class ClientConnectionStatusUi : MonoBehaviour {

    protected static ConnectionStatus LastStatus;

    private IClientSocket _connection;

    public Image Image;

    public Text Text;

    public Color UnknownColor = new Color(90/255f, 90/255f, 90/255f, 1);
    public Color OnlineColor = new Color(82 / 255f, 231 / 255f, 254 / 255f, 1);
    public Color ConnectingColor = new Color(254 / 255f, 237 / 255f, 82 / 255f, 1);
    public Color OfflineColor = new Color(254 / 255f, 82 / 255f, 82 / 255f, 1);

    public bool ChangeTextColor = true;

    protected virtual void Start(){
        _connection = GetConnection();
        _connection.StatusChanged += UpdateStatusView;

        UpdateStatusView(_connection.Status);
    }

    protected virtual void UpdateStatusView(ConnectionStatus status){
        LastStatus = status;

        switch (status){
            case ConnectionStatus.Connected:
                if (Image != null) Image.color = OnlineColor;
                //if (ChangeTextColor) Text.color = OnlineColor;
                //Text.text = "Connected";
                break;
            case ConnectionStatus.Disconnected:
                if (Image != null) Image.color = OfflineColor;
                //if (ChangeTextColor) Text.color = OfflineColor;

                //Text.text = "Offline";
                break;
            case ConnectionStatus.Connecting:
                if (Image != null) Image.color = ConnectingColor;
                //if (ChangeTextColor) Text.color = ConnectingColor;

                //Text.text = "Connecting";
                break;
            default:
                if (Image != null) Image.color = UnknownColor;
                //if (ChangeTextColor) Text.color = UnknownColor;

                //Text.text = "Unknown";
                break;
        }
    }

    protected virtual IClientSocket GetConnection(){
        return Msf.Connection;
    }

    protected virtual void OnDestroy(){
        _connection.StatusChanged -= UpdateStatusView;
    }
}
