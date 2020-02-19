using Barebones.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MonteCarlo {

    public class TwitterGameMaster : MonoBehaviour {

        public MonteCarloController gameController;
        public TextMeshProUGUI botStatus;
        // Use this for initialization
        void Start() {
            TCPLocalConnection.subscribeToTCPStatus(TCPStatusChanged);
            TCPMessageQueue.readMsgInstant = readTCPMessage;
        }


        private void readTCPMessage(ReceivedLocalMessage msg) {
            try {
                string[] words = msg.message.Split(' ');
                int x = int.Parse(words[0]);
                int y = int.Parse(words[1]);

                if(gameController.isInRange(x, y)) {
                    bool hit = gameController.samplePixel(x, y);
                    TCPLocalConnection.sendMessage(hit ? "HIT" : "MISS");
                }
                else
                    TCPLocalConnection.sendMessage("OUT OF BOUNDS");
            } catch(Exception e) {
                Debug.LogError(e.Message);
                TCPLocalConnection.sendMessage("Invalid Message");
            }
        }

        public void restartPressed() {
            TCPLocalConnection.stopServer();
            TCPLocalConnection.startServer(4000);
            gameController.restartGame();
        }

        private void TCPStatusChanged(ConnectionStatus status) {
            if (status == ConnectionStatus.Connected)
                botStatus.color = Color.green;
            else if (status == ConnectionStatus.Connecting)
                botStatus.color = Color.yellow;
            else
                botStatus.color = Color.red;

            botStatus.text = status.ToString();
        }
        void OnDestroy() {TCPLocalConnection.unSubscribeToTCPStatus(TCPStatusChanged);}
    }
}