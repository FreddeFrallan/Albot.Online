using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Barebones.Networking;
using System;

namespace ClientUI {
    public class ClientUserListTMProEnabled : MonoBehaviour {

        private class listObject {
            public Int64 chatID;
            public GameObject obj;
            public listObject(Int64 chatID, GameObject obj) { this.chatID = chatID; this.obj = obj; }
        }
        public Transform userList;
        public ClientUserListObjectTMProEnabled userPrefab;
        private List<listObject> currentUsers = new List<listObject>();

        public void addPlayerToList(string username, int iconNumber, Int64 chatID) {
            if (currentUsers.Find(x => x.chatID == chatID) != null)
                return;

            ClientUserListObjectTMProEnabled newUser = Instantiate(userPrefab, userList);
            newUser.initUser(iconNumber, username);
            newUser.gameObject.SetActive(true);
            currentUsers.Add(new listObject(chatID, newUser.gameObject));
        }
        public void removePlayerFromList(Int64 chatID) {
            listObject user = currentUsers.Find(x => x.chatID == chatID);
            if (user == null)
                return;

            Destroy(user.obj);
            currentUsers.Remove(user);
        }

        public void clearList() {
            for (int i = currentUsers.Count - 1; i >= 0; i--)
                removePlayerFromList(currentUsers[i].chatID);
        }
    }
}