using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ClientUI {
    public class ClientUserListObjectTMProEnabled : MonoBehaviour {

        public Image img;
        public TextMeshProUGUI username;

        public void initUser(int iconNumber, string username) {
            img.sprite = ClientIconManager.loadIcon(iconNumber);
            this.username.text = username;
        }

    }
}