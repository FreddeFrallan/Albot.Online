using AlbotServer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ClientUI;

namespace Game {

    public class StaticClientController : NetworkBehaviour {

        private static StaticClientController singleton;
        private List<NetworkMessage> storedPlayerJoined = new List<NetworkMessage>(), storedPlayerLeft = new List<NetworkMessage>();
        private bool broadcasting = false, playingGame = false;

        private void Start() {
            connectionToServer.RegisterHandler((short)ServerCommProtocl.Ping, handlePing);
            ClientUIOverlord.onUIStateChanged += UIStateChanged;
        }

        private void Update() {
            print("update");
        }


        public static void subscribePlayerJoined(Action<NetworkMessage> handler) {
            Debug.LogError(handler);
            Debug.LogError(singleton.storedPlayerJoined);
            singleton.subscribeHandler(handler, singleton.storedPlayerJoined);
        }
        public static void subscribePlayerLeft(Action<NetworkMessage> handler) { singleton.subscribeHandler(handler, singleton.storedPlayerLeft); }
        public void subscribeHandler(Action<NetworkMessage> handler, List<NetworkMessage> storedMessages) {
            StopCoroutine(updater(storedMessages, handler));
            broadcasting = true;
            StartCoroutine(updater(storedMessages, handler));
        }

        private IEnumerator updater(List<NetworkMessage> storage, Action<NetworkMessage> handler) {
            while (broadcasting) {
                yield return new WaitForSeconds(0.1f);
                foreach (NetworkMessage n in storage)
                    handler(n);
                storage.Clear();
            }
        }


        private void onLeftGame() {
            singleton.broadcasting = false;
            StopAllCoroutines();
            storedPlayerJoined.Clear();
            storedPlayerLeft.Clear();
        }

        #region Static handlers
        private void handlePlayerJoinedGameRoom(NetworkMessage msg) {storedPlayerJoined.Add(msg);}
        private void handlePlayerLeftGameRoom(NetworkMessage msg) {storedPlayerLeft.Add(msg);}
        private void handlePing(NetworkMessage msg) { TCPLocalConnection.sendMessage("PING Response"); }


        private void UIStateChanged(ClientUIStates state) {
            if (state != ClientUIStates.PlayingGame && playingGame)
                onLeftGame();
            else if (state == ClientUIStates.PlayingGame)
                playingGame = true;
        }
        #endregion
    }

}