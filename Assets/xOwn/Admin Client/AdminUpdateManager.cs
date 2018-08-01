using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;
using Barebones.Networking;
using UnityEngine.SceneManagement;
using System.Linq;
using AlbotServer;
using Game;

namespace AdminUI{

	public class AdminUpdateManager : MonoBehaviour{

		private static AdminUpdateManager singleton;
		private static GameRenderer localRenderer;
        private static List<GameLogState> logQueue = new List<GameLogState> ();
		private static bool isSpectating = false;
		private static string currentRoomID;
		private static PlayerInfo[] currentPlayers = new PlayerInfo[0];

        private static bool isInited = false;

		void Start () {
            print(gameObject);
			Msf.Connection.SetHandler ((short)CustomMasterServerMSG.spectateLogUpdate, handleUpdateMsg);
            Msf.Connection.SetHandler((short)CustomMasterServerMSG.spectateGameStarted, handleSpectateStartMsg);
            singleton = this;
		}



		public static void handleUpdateMsg(IIncommingMessage msg){
            if (localRenderer == null)
                addToLogQueue(msg.Deserialize<SpectatorGameLog>());
            else
                localRenderer.addLogMsg(msg.Deserialize<SpectatorGameLog>());
        }

        private void handleSpectateStartMsg(IIncommingMessage rawMsg) {startNewSpectateGame(rawMsg.Deserialize<RunningGameInfoMsg>());}
        public void startNewSpectateGame(RunningGameInfoMsg infoMsg) {
            currentRoomID = infoMsg.gameID;
            handleLogInit(infoMsg);
            AdminUIManager.requestGotoGame(infoMsg.gameType, onGameSceneLoaded);
        }

        public static void handleLogInit(RunningGameInfoMsg initMsg) {
            currentPlayers = initMsg.players;
            localRenderer = null;
            singleton.startInitQueue();
		}

		public static void onGameSceneLoaded(){
            localRenderer = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameRenderer>();
            if (localRenderer != null) {
                localRenderer.adminInit();
                localRenderer.initPlayerSlots(currentPlayers);
                flushQueue();
            }
            else
                Debug.LogError("Could not find renderer");
		}

		public static void stopSpectating(){
			print ("Stop spectatiing " + isSpectating);
			if (isSpectating)
				Msf.Connection.SendMessage ((short)CustomMasterServerMSG.stopSpectate);
			isSpectating = false;
		}

        public static void requestSpecificLogMessages(int[] missingUpdates) {
            print("Missing Updates " + missingUpdates.Length + ", Requesting...");
            Msf.Connection.SendMessage((short)CustomMasterServerMSG.requestSpecificGameLog, 
                new SpectatorSpecificLogRequestMsg() {broadcastID = currentRoomID, IDs = missingUpdates},
                (s, m) => {
                    print("Missing Response: " + s);
                    if (s == ResponseStatus.Success)
                        localRenderer.addMissingUpdates(m.Deserialize<SpectatorGameLog>().gameLog); } 
            );
        }

        #region LogQueue
        private static void addToLogQueue(SpectatorGameLog log){logQueue.AddRange (log.gameLog);}
        private void startInitQueue() { StartCoroutine(initQueue()); }
        public IEnumerator initQueue() {
            while (localRenderer == null)
                yield return new WaitForSeconds(1);
        }
		private static void flushQueue(){
            logQueue.OrderBy(g => g.updateNumber);
            localRenderer.addLogMsg(logQueue.ToArray());
			logQueue.Clear ();
		}
        #endregion
    }

}