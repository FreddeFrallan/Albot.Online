using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlbotServer;
using Barebones.Networking;
using Barebones.MasterServer;
using Game;

namespace ClientUI {

    public class CurrentGame{

        private static PreGameSpecs gameSpecs;
        private static GameConnectorUI gameConnector;
        private static bool currentlyPlayingGame = false;

        public static void init(GameConnectorUI connector) {
            gameConnector = connector;
            TCPLocalConnection.subscribeToTCPStatus(TCPStatusChanged);
            ClientUIOverlord.onUIStateChanged += UIStateChanged;
        }
 

        public static void handleStartGame(IIncommingMessage rawMsg) {
            PreGameStartedMsg msg = rawMsg.Deserialize<PreGameStartedMsg>();
            if(matchingSpecs(msg.specs, gameSpecs) == false) {
                Debug.LogError("Got non-matching start msg to: " + msg.specs.roomID + " with new ID: " + msg.gameRoomID + " Type: " + msg.specs.type);
                return;
            }
            currentlyPlayingGame = true;
            setupLocalPlayers(msg.slots);
            gameConnector.onJoinStartedGame(msg);
        }

        private static void setupLocalPlayers(PreGameSlotInfo[] slots) {
            ClientPlayersHandler.resetLocalPLayers();
            string localUsername = ClientUIOverlord.getCurrentAcountInfo().Username;
            foreach (PreGameSlotInfo slot in slots)
                if (slot.belongsToPlayer == localUsername)
                    addLocalPlayer(slot.type);
        }

        private static void addLocalPlayer(PreGameSlotType type) {
            Debug.Log("Adding Local: " + type);

            if(type == PreGameSlotType.Player)
                ClientPlayersHandler.addSelf();
            else if (type == PreGameSlotType.TrainingBot)
                LocalTrainingBots.addBot(gameSpecs.type);
            else if (type == PreGameSlotType.SelfClone)
                ClientPlayersHandler.addClone();
            else if (type == PreGameSlotType.Human)
                ClientPlayersHandler.addHuman();
        }

        private static void UIStateChanged(ClientUIStates state) {
            if(currentlyPlayingGame && state != ClientUIStates.PlayingGame)
                Msf.Connection.SendMessage((short)ServerCommProtocl.PlayerLeftPreGame, gameSpecs.roomID);
        }

        private static void TCPStatusChanged(ConnectionStatus status) {
            if (ClientUIOverlord.currentState != ClientUIStates.PlayingGame)
                return;

            if (status == ConnectionStatus.Connected)
                restartCurrentGame();
            if (status == ConnectionStatus.Disconnected && ClientUIOverlord.currentState == ClientUIStates.PlayingGame)
                TCPLocalConnection.restartServer();
        }

        public static void restartCurrentGame() {
            UnetRoomConnector.shutdownCurrentConnection();
            AlbotDialogBox.removeAllPopups();
            Msf.Connection.SendMessage((short)ServerCommProtocl.RestartTrainingGame, gameSpecs.roomID, Msf.Helper.handleErrorResponse);
        }

        private static bool matchingSpecs(PreGameSpecs a, PreGameSpecs b) {return a.roomID == b.roomID && a.type == b.type;}
        public static void setNewCurrentPreGame(PreGameSpecs newSpecs) { gameSpecs = newSpecs; }
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