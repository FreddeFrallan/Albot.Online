using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlbotServer;
using Barebones.Networking;
using Barebones.MasterServer;

namespace ClientUI {

    public class CurrentGame{

        private static PreGameSpecs PreGameSpecs;
        private static PreGameStartedMsg lastGameStartedMsg;
        private static GameConnectorUI gameConnector;

        public static void init(GameConnectorUI connector) {
            gameConnector = connector;
            TCPLocalConnection.subscribeToTCPStatus(TCPStatusChanged);
        }
 

        public static void handleStartGame(IIncommingMessage rawMsg) {
            PreGameStartedMsg msg = rawMsg.Deserialize<PreGameStartedMsg>();
            if(matchingSpecs(msg.specs, PreGameSpecs) == false) {
                Debug.LogError("Got non-matching start msg to: " + msg.specs.roomID + " with new ID: " + msg.gameRoomID + " Type: " + msg.specs.type);
                return;
            }
            lastGameStartedMsg = msg;
            gameConnector.onJoinStartedGame(msg);
        }



        private static void TCPStatusChanged(ConnectionStatus status) {
            if (lastGameStartedMsg.specs.isTraining == false || ClientUIOverlord.currentState != ClientUIStates.PlayingGame)
                return;

            if (status == ConnectionStatus.Connected) {
                UnetRoomConnector.shutdownCurrentConnection();
                AlbotDialogBox.removeAllPopups();
                Debug.Log("Starting Training: " + lastGameStartedMsg.gameRoomID);
                Msf.Connection.SendMessage((short)ServerCommProtocl.RestartTrainingGame, lastGameStartedMsg.gameRoomID);
            }
            if (status == ConnectionStatus.Disconnected && ClientUIOverlord.currentState == ClientUIStates.PlayingGame) {
                TCPLocalConnection.restartServer();
                //currentClientController.stopGameTimers();
            }
        }

        private static bool matchingSpecs(PreGameSpecs a, PreGameSpecs b) {return a.roomID == b.roomID && a.type == b.type;}
        public static void setNewCurrentPreGame(PreGameSpecs newSpecs) { PreGameSpecs = newSpecs; }
        public static void setNewCurrentGame(PreGameStartedMsg msg) { lastGameStartedMsg = msg; }
    }

    public enum GameState {
        PreGame,
        PreGameSingleplayer,
        PreGameTraining,
        PreGameTournament,
        PlayingGame,
        PlayingTournament,
    }
}