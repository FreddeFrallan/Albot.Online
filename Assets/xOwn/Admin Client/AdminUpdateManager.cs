using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;
using Barebones.Networking;
using UnityEngine.SceneManagement;
using System.Linq;

namespace AdminUI{

	public class AdminUpdateManager : MonoBehaviour{

		private static AdminUpdateManager singleton;
		private static AdminController controller;
		private static List<SpectatorGameLog> logQueue = new List<SpectatorGameLog> ();
		private static bool isSpectating = false;
		private static string roomID;
		private static AlbotServer.PlayerInfo[] currentPlayers = new AlbotServer.PlayerInfo[0];

		void Start () {
			Msf.Connection.SetHandler ((short)CustomMasterServerMSG.spectateLogUpdate, handleUpdateMsg);
			AdminUIManager.onAdminUIStateChanged += (state) => {if(state != ClientUI.ClientUIStates.PlayingGame) stopSpectating();};
			singleton = this;
		}



		public static void handleUpdateMsg(IIncommingMessage msg){
			SpectatorGameLog logMsg = msg.Deserialize<SpectatorGameLog> ();
			logMsg = checkForInitMsg (logMsg);

			if (controller != null) 
				controller.addLogMove (logMsg.gameLog);
			else
				addToLogQueue (logMsg);
		}

		public static void handleStartLogMsg(SpectatorGameLog logMsg){
			logQueue.Clear ();
			controller = null;
			addToLogQueue (logMsg);
		}

		private static SpectatorGameLog checkForInitMsg(SpectatorGameLog logMsg){
			if (logMsg.gameLog.Length == 0 || logMsg.gameLog[0].Length == 0 || logMsg.gameLog [0] [0] != ':')
				return logMsg;

			print ("Detected init msg");
			handleInitMsg (logMsg.gameLog [0]);

			List<string> extracted = logMsg.gameLog.ToList ();
			extracted.RemoveAt (0);
			logMsg.gameLog = extracted.ToArray ();
			return logMsg;
		}

		private static void handleInitMsg(string msg){
			if (controller == null)
				singleton.startInitQueue (msg);
			else
				controller.handleGameInfoMsg (msg);
		}

		private void startInitQueue(string msg){
			StopCoroutine (initQueue(""));
			StartCoroutine (initQueue (msg));
		}

		public IEnumerator initQueue(string msg){
			while (controller == null) {
				yield return new WaitForSeconds (1);
				controller.handleGameInfoMsg (msg);
			}
		}




		public static void onGameSceneLoaded(){
			controller = GameObject.FindGameObjectWithTag ("GameController").GetComponent<AdminController>();
			if (controller != null) {
				controller.init ();
				flushQueue ();
			}
		}

		public static void requestStartSpectate(string gameID){
			isSpectating = true;
			roomID = gameID;
		}
		public static void stopSpectating(){
			print ("Stop spectatiing " + isSpectating);
			if (isSpectating)
				Msf.Connection.SendMessage ((short)CustomMasterServerMSG.stopSpectate);
			isSpectating = false;
		}



		private static void addToLogQueue(SpectatorGameLog logMsg){
			Debug.LogError ("Can't find controller, adding to queue");
			logQueue.Add (logMsg);
		}

		private static void flushQueue(){
			if (logQueue.Count == 0)
				return;
			controller.initLog (logQueue [0].gameLog);

			for(int i = 1; i < logQueue.Count; i++)
				controller.initLog (logQueue [i].gameLog);

			logQueue.Clear ();
		}
	}

}