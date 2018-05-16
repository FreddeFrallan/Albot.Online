using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClientUI{
	public class AlbotDialogBox : MonoBehaviour {

		private class dialogEvent{
			public Action callback;
			public string infoText, buttonText;
			public int infoSize, buttonTextSize;
			public DialogBoxType eventType;
			public dialogEvent(Action callback, string infoText, string buttonText, int infoSize, int buttonTextSize, DialogBoxType type){
				this.callback = callback; this.infoText = infoText; this.buttonText = buttonText; this.infoSize = infoSize; this.buttonTextSize = buttonTextSize;
				this.eventType = type;
			}
		}
		//Quick hack for not showing dissconnect msg on GameOver server
		private static bool ignoreConnectionLostMsg = false;
		private static bool gameOver = false;
		private static float ignoreEndtime;

		private static AlbotDialogBox singleton;
		private List<dialogEvent> eventQueue = new List<dialogEvent> ();
		private bool showingDialogBox = false;
		private Action currentCallback;

		public DialogBoxPanel regularPanel, gameOverPanel;
		private DialogBoxPanel currentPanel;

		public void init(){
			singleton = this;
			ClientUIOverlord.onUIStateChanged += onUIChanged;
		}

		private void onUIChanged(ClientUIStates state){
			if (state != ClientUIStates.PlayingGame)
				gameOver = false;
		}

		void Update(){
			if (showingDialogBox || eventQueue.Count == 0)
				return;

			showEvent ();
		}
			
		private void showEvent(){
			showingDialogBox = true;
			dialogEvent theEvent = eventQueue [0];

			currentCallback = theEvent.callback;
		
			currentPanel = theEvent.eventType == DialogBoxType.GameState ? gameOverPanel : regularPanel;
			currentPanel.showPanel (theEvent.buttonText, theEvent.infoText, theEvent.infoSize, theEvent.buttonTextSize);
			eventQueue.RemoveAt (0);
		}
			
		public void onButtonClicked(){
			showingDialogBox = false;
			currentPanel.disable ();

			currentCallback.Invoke ();
			if (eventQueue.Count > 0)
				showEvent ();
		}

		public static void removeAllPopups(){
			singleton.eventQueue.Clear ();
			singleton.showingDialogBox = false;
			if(singleton.currentPanel != null)
				singleton.currentPanel.disable ();
		}

		public static void activateButton(Action buttonCallback, DialogBoxType type, string infoText, string buttonText, int infoSize = 30,  int buttonTextSize = 25){
			if (ignoreConnectionLostMsg && (type == DialogBoxType.GameServerConnLost || type == DialogBoxType.MasterServerConnLost))
				return;
			if (gameOver && type == DialogBoxType.GameServerConnLost)
				return;

			addToEventQ(new dialogEvent (buttonCallback, infoText, buttonText, infoSize, buttonTextSize, type));
		}

		//Currently we only allow one of each typ to be queued.
		private static void addToEventQ(dialogEvent newEvent){
			if (singleton.eventQueue.Find (x => x.eventType == newEvent.eventType) != null)
				return;

			singleton.eventQueue.Add (newEvent);
		}



		public static void setIgnoreConnectionLostMsg(float ignoreTime){
			ignoreEndtime = Time.time + ignoreTime;
			if (ignoreConnectionLostMsg == false)
				singleton.StartCoroutine (singleton.ignoreTimer ());
		}

		public static void setGameOver(){gameOver = true;}

		public IEnumerator ignoreTimer(){
			ignoreConnectionLostMsg = true;
			while (ignoreConnectionLostMsg) {
				yield return new WaitForSeconds (0.1f);

				if (Time.time >= ignoreEndtime)
					ignoreConnectionLostMsg = false;
			}
		}
	}

	public enum DialogBoxType{
		GameState,
		GameServerConnLost,
		MasterServerConnLost,
		BotConnectionError
	}
}