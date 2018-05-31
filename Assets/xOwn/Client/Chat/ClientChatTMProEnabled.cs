﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Barebones.MasterServer;
using Barebones.Networking;
using System;
using AlbotServer;
using TMPro;

namespace ClientUI {
    public class ClientChatTMProEnabled : MonoBehaviour {
        public TMP_InputField InputField;
        public LayoutGroup MessagesList;
        public TextMeshProUGUI MessagePrefab;
        public ClientUserListTMProEnabled usersList;

        private bool _allowFocusOnEnter = true;
        private bool FocusOnEnterClick = true;
        private bool isCurrentlyInChat = false;
        private NetworkConnection serverConnection;


        public void initChat() {
            Msf.Connection.SetHandler((short)ServerCommProtocl.LobbyChatMsg, onIncominChatgMsg);
            Msf.Connection.SetHandler((short)ServerCommProtocl.LobbyPlayerEnter, onPlayerJoinedChat);
            Msf.Connection.SetHandler((short)ServerCommProtocl.LobbyPlayerLeft, onPlayerleftChat);
            ClientUIOverlord.onUIStateChanged += onUiStateChanged;
        }

        private void onUiStateChanged(ClientUIStates state) {
            bool exitedChat = false, enteredChat = false;
            if (state == ClientUIStates.GameLobby || state == ClientUIStates.PreGame) {
                if (isCurrentlyInChat == false) {
                    enteredChat = true;
                    isCurrentlyInChat = true;
                }
            } else if (isCurrentlyInChat) {
                exitedChat = true;
                isCurrentlyInChat = false;
            }

            AccountInfoPacket currentAcountInfo = ClientUIOverlord.getCurrentAcountInfo();
            AlbotChatMsg msg = new AlbotChatMsg() { icon = int.Parse(currentAcountInfo.Properties["icon"]), username = currentAcountInfo.Username };

            if (enteredChat) {
                for (int i = MessagesList.transform.childCount; i > 1; i--)
                    Destroy(MessagesList.transform.GetChild(i - 1).gameObject);

                Msf.Connection.SendMessage((short)ServerCommProtocl.LobbyPlayerEnter, msg);
            } else if (exitedChat) {
                Msf.Connection.SendMessage((short)ServerCommProtocl.LobbyPlayerLeft, msg);
                clearList();
            }

        }

        private void clearList() { usersList.clearList(); }

        #region incoming msg from server
        private void onIncominChatgMsg(IIncommingMessage msg) {
            AlbotChatMsg chatMsg = msg.Deserialize<AlbotChatMsg>();
            TextMeshProUGUI newMsg = Instantiate(MessagePrefab, MessagesList.transform);

            string timeStamp = generateTimeStamp();
            newMsg.text = timeStamp + " " + chatMsg.username + ": " + chatMsg.textMsg;
            newMsg.gameObject.SetActive(true);

            adjustContentViewSize();
        }

        #region Timestamp
        private string generateTimeStamp() {
            int hour = DateTime.Now.Hour;
            int minute = DateTime.Now.Minute;
            return formatTime(hour) + ":" + formatTime(minute) + " ";
        }
        private string formatTime(int t) {
            if (t < 10)
                return "0" + t.ToString();
            return t.ToString();
        }
        #endregion

        private void adjustContentViewSize() {
            int nrOfMessages = MessagesList.transform.childCount;
            float heightOfPreFab = MessagePrefab.GetComponent<RectTransform>().sizeDelta.y;

            RectTransform contentContainer = MessagesList.GetComponent<RectTransform>();
            Vector2 currentMin = contentContainer.offsetMin;
            contentContainer.offsetMin = new Vector2(contentContainer.offsetMin.x, nrOfMessages * heightOfPreFab * -1);
        }


        private void onPlayerJoinedChat(IIncommingMessage msg) {
            AlbotChatMsg chatMsg = msg.Deserialize<AlbotChatMsg>();
            usersList.addPlayerToList(chatMsg.username, chatMsg.icon, chatMsg.chatID);
        }
        private void onPlayerleftChat(IIncommingMessage msg) {
            AlbotChatMsg chatMsg = msg.Deserialize<AlbotChatMsg>();
            usersList.removePlayerFromList(chatMsg.chatID);
        }
        private void onChatInit(IIncommingMessage msg) { //If we lated decides to send a longer Init msg
                                                         //	AlbotChatMsg chatMsg = msg.Deserialize<AlbotChatMsg> ();
        }
        #endregion





        #region UI SendMsg
        void Update() {
            if (FocusOnEnterClick && Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
                OnSendClick();
                if (_allowFocusOnEnter)
                    InputField.ActivateInputField();
            }
        }
        public virtual void OnSendClick() {
            var text = InputField.text;
            if (string.IsNullOrEmpty(text))
                return;

            AlbotChatMsg msg = new AlbotChatMsg();
            msg.textMsg = InputField.text;
            msg.username = Msf.Client.Auth.AccountInfo.Username;
            Msf.Connection.SendMessage((short)ServerCommProtocl.LobbyChatMsg, msg);

            InputField.text = "";
            InputField.DeactivateInputField();

            if (_allowFocusOnEnter)
                StartCoroutine(DontAllowFocusOnEnter());
        }
        protected IEnumerator DontAllowFocusOnEnter() {
            yield return new WaitForEndOfFrame();
            InputField.ActivateInputField();
        }
        #endregion
    }
}