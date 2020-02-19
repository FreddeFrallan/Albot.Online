using ClientUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.Networking;
using Barebones.MasterServer;
using AlbotServer;

public class AnneHacks : MonoBehaviour{

    private static AnneHacks singleton;

    [SerializeField]
    public SinglePlayerLobby realSinglePlayerLobby;

    private static MapSelection gameMap;
    private static PreGameBaseLobby lobby;
    public static bool playingSinglePlayerGame = false;


    private void Start() {
        singleton = this;
    }


    public static void startSinglePlayerLobby(PreGameBaseLobby theLobby, MapSelection theGame) {
        lobby = theLobby;
        gameMap = theGame;
        singleton.realSinglePlayerLobby.setScene(gameMap.Scene);

        ClientUIOverlord.onUIStateChanged += UIStateChanged;
        TCPLocalConnection.subscribeToTCPStatus(tcpStatusChanged);
        playingSinglePlayerGame = true;
    }


    private static void stopSinglePlayerGame() {
        TCPLocalConnection.unSubscribeToTCPStatus(tcpStatusChanged);
        ClientUIOverlord.onUIStateChanged -= UIStateChanged;
        playingSinglePlayerGame = false;
    }


    private static void tcpStatusChanged(ConnectionStatus status) {
        if (status == ConnectionStatus.Connected)
            lobby.setPlayerReady(true);
        else
            lobby.setPlayerReady(false);
    }


    private static void UIStateChanged(ClientUIStates state) {
        if (state == ClientUIStates.GameLobby)
            stopSinglePlayerGame();
    }
    
    public void startGameClicked() {
        if (playingSinglePlayerGame)
            realSinglePlayerLobby.startButtonClicked();
        else 
            print("Starting Training game");
    }

}
