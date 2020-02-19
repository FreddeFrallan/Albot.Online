using Barebones.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SortingGame {

    public class SortingGameMaster : MonoBehaviour {

        [SerializeField]
        private SortingGameController controller;
        [SerializeField]
        private SortingAnimator theAnimator;
        [SerializeField]
        private TextMeshProUGUI TCPStatus;
        [SerializeField]
        private TMP_InputField arraySizeField;

        private SortingVisualNumber[] numbers;
        private bool gameStarted = false;


        void Start() {
            TCPLocalConnection.subscribeToTCPStatus(TCPStatusChanged);
            TCPMessageQueue.readMsgInstant = readTCPMessage;
            arraySizeField.text = "20";
        }

        private void readTCPMessage(ReceivedLocalMessage msg) {
            if (gameStarted == false)
                return;

            foreach(string c in msg.message.Trim().Split('|'))
                try {
                    string[] words = c.Trim().ToLower().Split(' ');
                    if (words.Length < 3 || (words[0] != "s" && words[0] != "c"))
                        continue;
                    ActionType type = words[0] == "s" ? ActionType.Swap : ActionType.Compare;

                    int[] param = new int[words.Length - 1];
                    for (int i = 0; i < words.Length - 1; i++) {
                        param[i] = int.Parse(words[i + 1]);
                        if (controller.isInRange(param[i]) == false)
                            continue;
                    }

                    theAnimator.addAction(new SortAction() { type = type, indexes = new List<int>() { param[0], param[1] } });
                }
                catch (Exception e) {Debug.LogError(e.Message);}
        }


        public void startButtonPressed() {
            int arraySize = int.Parse(arraySizeField.text);
            numbers = controller.startNewSession(arraySize, 0, 99);
            theAnimator.init(numbers);

            TCPLocalConnection.stopServer();
            TCPLocalConnection.startServer(4000);
            gameStarted = true;
        }
        
        private void sendStartMsg(SortingVisualNumber[] numbers) {
            string s = "";
            foreach (SortingVisualNumber n in numbers)
                s += n.value + " ";
            TCPLocalConnection.sendMessage(s);
        }


        private void TCPStatusChanged(ConnectionStatus status) {
            if (status == ConnectionStatus.Connected) {
                TCPStatus.color = Color.green;
                sendStartMsg(numbers);
            }
            else if (status == ConnectionStatus.Connecting)
                TCPStatus.color = Color.yellow;
            else
                TCPStatus.color = Color.red;

            TCPStatus.text = status.ToString();
        }
        void OnDestroy() { TCPLocalConnection.unSubscribeToTCPStatus(TCPStatusChanged); }
    }

}