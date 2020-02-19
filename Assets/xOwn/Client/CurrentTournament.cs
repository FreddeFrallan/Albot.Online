using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;
using Barebones.Networking;
using UnityEngine.SceneManagement;
using Tournament.Client;
using Tournament;
using AlbotServer;

namespace ClientUI {

    public class CurrentTournament {

        private static TournamentInfoMsg storedInitMsg;
        private static List<TournamentTreeUpdate> storedUpdates = new List<TournamentTreeUpdate>();
        private static string tournamentID = "";
        private static VisualTournamentTree currentTree;
        public static bool isInTournament = false;

        public static void init() {
            Msf.Server.SetHandler((short)CustomMasterServerMSG.startTournament, handleTournamentStarted);
            Msf.Server.SetHandler((short)CustomMasterServerMSG.closeTournament, handleTournamentClosed);
            Msf.Server.SetHandler((short)CustomMasterServerMSG.tournamentRoundPreStarted, handleRoundStarted);
            Msf.Server.SetHandler((short)CustomMasterServerMSG.runningTournamentUpdate, handleTreeUpdate);
        }


        #region Start Tournament
        private static void handleTournamentStarted(IIncommingMessage rawMsg) {
            storedUpdates.Clear();
            ClientUIStateManager.requestGotoState(ClientUIStates.PlayingTournament, () => {
                storedInitMsg = rawMsg.Deserialize<TournamentInfoMsg>();
                initCurrentTree(storedInitMsg);
            });
        }
        private static void initCurrentTree(TournamentInfoMsg infoMsg) {
            currentTree = GameObject.FindGameObjectWithTag("GameController").GetComponent<VisualTournamentTree>();
            currentTree.init(infoMsg.players, ServerUtils.tournamentInfoToGameSpecs(infoMsg), infoMsg.doubleElimination);
            isInTournament = true;
        }

        public static void handleJoinedTournament(ResponseStatus status, IIncommingMessage rawMsg) {
            if (Msf.Helper.serverResponseSuccess(status, rawMsg) == false)
                return;
            ClientUIStateManager.requestGotoState(ClientUIStates.PreTournament);
            tournamentID = rawMsg.AsString();
        }

        public static void reOpenTournament() {
            ClientUIStateManager.requestGotoState(ClientUIStates.PlayingTournament, () => {
                initCurrentTree(storedInitMsg);
                storedUpdates.ForEach(u => currentTree.updateRounds(u.rounds, false));
                currentTree.renderVisualTree();
            });
        }
        #endregion

        #region Quit Tournament
        public static void leaveCurrentTournament() {
            isInTournament = false;
            ClientUIStateManager.requestGotoState(ClientUIStates.GameLobby);
            Msf.Connection.SendMessage((short)CustomMasterServerMSG.leaveTournament);
        }

        private static void handleTournamentClosed(IIncommingMessage rawMsg) {
            if (rawMsg.AsString() != tournamentID)
                return;
            Debug.LogError("Tournament forcefully Closed");
            ClientUIStateManager.requestGotoState(ClientUIStates.GameLobby);
            isInTournament = false;
            tournamentID = "";
        }
        #endregion

        private static void handleRoundStarted(IIncommingMessage rawMsg) {
            if (ClientUIOverlord.currentState == ClientUIStates.PlayingGame && isInTournament)
                ClientUIStateManager.requestGotoState(ClientUIStates.GameLobby, () => {
                    NewGameCreator.handleJoinPreGame(GameInfoType.PreGame, rawMsg.AsString());
                });
            else
                NewGameCreator.handleJoinPreGame(GameInfoType.PreGame, rawMsg.AsString());
        }

        private static void handleTreeUpdate(IIncommingMessage rawMsg) {addNewUpdate(rawMsg.Deserialize<TournamentTreeUpdate>());}
        private static void addNewUpdate(TournamentTreeUpdate update) {
            storedUpdates.Add(update);
            if (ClientUIOverlord.currentState == ClientUIStates.PlayingTournament)
                currentTree.updateRounds(update.rounds, true);
        }
    }

}