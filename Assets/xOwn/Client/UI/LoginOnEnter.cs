using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ClientUI{

	public class LoginOnEnter : MonoBehaviour {

		public Button thisButton;
		public InputField password;
        public TMP_InputField username;

		private List<bool> focusBuffer = new List<bool>();
		private int bufferSize = 2;

		void Start(){
			for (int i = 0; i < bufferSize; i++)
				focusBuffer.Add (true);
		}

		// Update is called once per frame
		void Update () {
			focusBuffer.Add (username.isFocused || password.isFocused);
			focusBuffer.RemoveAt (0);

			if (Input.GetKeyDown (KeyCode.Return)) {
				if (ClientUIOverlord.currentState == ClientUIStates.LoginMenu) {
					if(focusBuffer[0])
						thisButton.onClick.Invoke ();
				}
			}
		}
	}

}