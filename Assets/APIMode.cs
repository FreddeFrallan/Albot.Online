using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using Barebones.Networking;
using System;

namespace TCP_API {

    public class APIMode : MonoBehaviour {

        private APIMessageRouterBase APIRouter;

  
        // Use this for initialization
        public void onSessionStarted() {
            APIRouter =  new Connect4.Connect4APIRouter();
            TCPMessageQueue.readMsgInstant = readTCPMsg;
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Space))
                CurrentAPIModeRouter.startNewSession(GameType.Connect4, onSessionStarted);
        }


        private void readTCPMsg(ReceivedLocalMessage inMsg) {
            APIMsgConclusion outMsg = APIRouter.handleIncomingMsg(inMsg.message);
            TCPLocalConnection.sendMessage(outMsg.msg);
        }
    }


    public class CurrentAPIModeRouter {
        public static GameType currentGameType;
        private static APIMessageRouterBase router;
        private static Action onStartFunc;

        public static void startNewSession(GameType type, Action startFunc, int port = 4000) {
            TCPLocalConnection.init();
            currentGameType = type;
            onStartFunc = startFunc;
            if (type == GameType.Connect4)
                router = new Connect4.Connect4APIRouter();

            TCPLocalConnection.startServer(port);
            TCPLocalConnection.subscribeToTCPStatus(onBotConnected);
        }

        private static void onBotConnected(ConnectionStatus status) {
            if (status == ConnectionStatus.Disconnected) {
                TCPLocalConnection.restartServer();
                return;
            }

            if (status != ConnectionStatus.Connected)
                return;

            onStartFunc();
            TCPLocalConnection.sendMessage(currentGameType + " API session started.");
        }

        public static APIMessageRouterBase getCurrentRouter() { return router; }
    }
}