using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClientUI{
	public class ClientChatUserListObject: MonoBehaviour {
		
		public Image img;
		public Text username;

		public void initUser(int iconNumber, string username){
			img.sprite = ClientIconManager.loadIcon (iconNumber);
			this.username.text = username;
		}

	}
}