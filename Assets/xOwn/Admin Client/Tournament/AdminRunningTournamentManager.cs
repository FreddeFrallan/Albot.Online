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


        private void Update() {
            if (runningTournament == false)
                return;

            if (Input.GetKeyDown(KeyCode.Alpha0)) {
                currentGame = new RoundID { col = 0, row = 0 };
                openRoundLobby(currentGame);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                currentGame = new RoundID { col = 0, row = 1 };
                openRoundLobby(currentGame);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                currentGame = new RoundID { col = 1, row = 0 };
                openRoundLobby(currentGame);
            }

            if (Input.GetKeyDown(KeyCode.P)) {
                startRoundGame(currentGame);
            }
        }

        private void openRoundLobby(RoundID id) {
            Msf.Connection.SendMessage((short)CustomMasterServerMSG.tournamentRoundPreStarted, new TournamentPreGameInfo() {
                tournamentID = tournamentInfo.tournamentID, roundID = id
            });
        }

        private void startRoundGame(RoundID id) {
            Msf.Connection.SendMessage((short)CustomMasterServerMSG.tournamentRoundStarted, new TournamentPreGameInfo() {
                tournamentID = tournamentInfo.tournamentID, roundID = id
            });
        }

        private void handleTournamentUpdate(IIncommingMessage rawMsg) {currentTree.updateRounds(rawMsg.Deserialize<TournamentTreeUpdate>().rounds);}
        public static void onTournamentStarted(TournamentInfoMsg tournamentInfo) {
            singleton.tournamentInfo = tournamentInfo;
            singleton.currentTree = GameObject.FindGameObjectWithTag("GameController").GetComponent<VisualTournamentTree>();
            singleton.currentTree.init(tournamentInfo.players, ServerUtils.tournamentInfoToGameSpecs(tournamentInfo));
            singleton.runningTournament = true;
        }

    }
}