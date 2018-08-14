using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;
using Barebones.Networking;
using UnityEngine.SceneManagement;
using System.Linq;
using AlbotServer;
using Game;
using ClientUI;

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
			Msf.Connection.SetHandler ((short)CustomMasterServerMSG.spectateLogUpdate, handleUpdateMsg);
            Msf.Connection.SetHandler((short)CustomMasterServerMSG.spectateGameStarted, handleSpectateStartMsg);
            AdminUIManager.onAdminUIStateChanged += onUiStateChanged;
            singleton = this;
		}

        #region Start Spectate
        public static void startSpectateGame(string roomID) {
            SpectatorSubscriptionsMsg outMsg = new SpectatorSubscriptionsMsg() { broadcastID = roomID };
            Msf.Connection.SendMessage((short)CustomMasterServerMSG.startSpectate, outMsg, ((s, m) => {
                if (s == ResponseStatus.Success) {
                    RunningGameInfoMsg infoMsg = m.Deserialize<RunningGameInfoMsg>();
                    if (infoMsg.status == PreGameState.Running)
                        singleton.startNewSpectateGame(infoMsg);
                }
                else
                    Debug.Log("Error:" + m.AsString());
            }));
        }

        private void handleSpectateStartMsg(IIncommingMessage rawMsg) {startNewSpectateGame(rawMsg.Deserialize<RunningGameInfoMsg>());}
        public void startNewSpectateGame(RunningGameInfoMsg infoMsg) {
            currentRoomID = infoMsg.gameID;
            handleLogInit(infoMsg);
            AdminUIManager.requestGotoGame(infoMsg.gameType, onGameSceneLoaded);
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
        #endregion


        #region During Spectate
        public static void handleUpdateMsg(IIncommingMessage msg){
            if (localRenderer == null)
                addToLogQueue(msg.Deserialize<SpectatorGameLog>());
            else
                localRenderer.addLogMsg(msg.Deserialize<SpectatorGameLog>());
        }

        public static void handleLogInit(RunningGameInfoMsg initMsg) {
            currentPlayers = initMsg.players;
            localRenderer = null;
            logQueue.Clear();
            singleton.startInitQueue();
            isSpectating = true;
        }
        #endregion

        #region Stop Spectating
        private static void onUiStateChanged(ClientUIStates newState) {
            if (isSpectating && newState != ClientUIStates.PlayingGame)
                stopSpectating();
        }
        public static void stopSpectating(){
			if (isSpectating)
				Msf.Connection.SendMessage ((short)CustomMasterServerMSG.stopSpectate, (s, r) =>{ print(s); });
			isSpectating = false;
		}
        #endregion

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

        public static void requestSpecificLogMessages(int[] missingUpdates) {
            Msf.Connection.SendMessage((short)CustomMasterServerMSG.requestSpecificGameLog, 
                new SpectatorSpecificLogRequestMsg() {broadcastID = currentRoomID, IDs = missingUpdates},
                (s, m) => {
                    print("Missing Response: " + s);
                    if (s == ResponseStatus.Success)
                        localRenderer.addMissingUpdates(m.Deserialize<SpectatorGameLog>().gameLog); } 
            );
        }
        #endregion
    }

}