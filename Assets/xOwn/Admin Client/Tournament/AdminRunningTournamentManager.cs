using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tournament;
using Tournament.Client;
using AlbotServer;
using Barebones.MasterServer;
using Tournament.Server;
using Barebones.Networking;

namespace AdminUI {

    public class AdminRunningTournamentManager : MonoBehaviour {

        private static AdminRunningTournamentManager singleton;
        private VisualTournamentTree currentTree;
        private TournamentInfoMsg tournamentInfo;
        private bool runningTournament = false;
        private RoundID currentGame;

        private void Start() {
            singleton = this;
            Msf.Connection.SetHandler((short)CustomMasterServerMSG.runningTournamentUpdate, handleTournamentUpdate);
        }


        private void openRoundLobby(RoundID id) {
            if (runningTournament == false)
                return;

            Msf.Connection.SendMessage((short)CustomMasterServerMSG.tournamentRoundPreStarted, new TournamentPreGameInfo() {
                tournamentID = tournamentInfo.tournamentID, roundID = id
            });
        }

        private void startGame(RoundID id) {
            if (runningTournament == false)
                return;

            Msf.Connection.SendMessage((short)CustomMasterServerMSG.tournamentRoundStarted, new TournamentPreGameInfo() {
                tournamentID = tournamentInfo.tournamentID, roundID = id
            });
        }

        public static bool isAdmin() { return singleton != null; }
        public static void startRoundGame(RoundID id) { singleton.startGame(id); }
        public static void startRoundLobby(RoundID id) { singleton.openRoundLobby(id); }
        private void handleTournamentUpdate(IIncommingMessage rawMsg) {currentTree.updateRounds(rawMsg.Deserialize<TournamentTreeUpdate>().rounds);}
        public static void onTournamentStarted(TournamentInfoMsg tournamentInfo) {
            singleton.tournamentInfo = tournamentInfo;
            singleton.currentTree = GameObject.FindGameObjectWithTag("GameController").GetComponent<VisualTournamentTree>();
            singleton.currentTree.init(tournamentInfo.players, ServerUtils.tournamentInfoToGameSpecs(tournamentInfo), tournamentInfo.doubleElimination);
            singleton.runningTournament = true;
        }

    }
}