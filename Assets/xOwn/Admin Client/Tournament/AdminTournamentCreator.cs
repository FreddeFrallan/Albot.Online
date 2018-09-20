using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlbotServer;
using Barebones.MasterServer;
using Barebones.Networking;
using UnityEngine.SceneManagement;
using Tournament;
using Tournament.Client;

namespace AdminUI {
    public class AdminTournamentCreator : MonoBehaviour {

        private bool waitingForResponse = false;
        private TournamentState state = TournamentState.None;
        private string currentTournamentID;
  
        #region Create
        public void createButtonPressed() {
            if (waitingForResponse)
                return;
            TournamentInfoMsg msg = new TournamentInfoMsg() { type = Game.GameType.Snake, maxPlayers = 128, doubleElimination = true };
            Msf.Connection.SendMessage((short)CustomMasterServerMSG.createTournament, msg, handleTournamentCreated);
            waitingForResponse = true;
        }

        private void handleTournamentCreated(ResponseStatus status, IIncommingMessage rawMsg) {
            string msg = rawMsg.AsString();
            print("Tournament Created: " + status + "  :" + msg);
            if (waitingForResponse == false)
                return;

            waitingForResponse = false;
            if (status == ResponseStatus.Success) {
                currentTournamentID = msg;
                state = TournamentState.Lobby;
            }
        }
        #endregion


        #region Start
        public void startTournamentClicked() {
            if (state != TournamentState.Lobby)
                return;
            Msf.Connection.SendMessage((short)CustomMasterServerMSG.startTournament, currentTournamentID, handleTournamentStarted);
        }

        private void handleTournamentStarted(ResponseStatus status, IIncommingMessage msg) {
            if (status != ResponseStatus.Success) {
                Debug.LogError(msg.AsString());
                return;
            }

            TournamentInfoMsg specMsg = msg.Deserialize<TournamentInfoMsg>();
            AdminUIManager.requestGotoState(ClientUI.ClientUIStates.PlayingTournament, () => {
                AdminRunningTournamentManager.onTournamentStarted(specMsg);
            });
        }

        #endregion

        private enum TournamentState {
            Running, Lobby, None
        }
    }
}