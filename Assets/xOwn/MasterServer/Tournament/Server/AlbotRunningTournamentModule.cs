using Barebones.Networking;
using System.Collections;
using System.Collections.Generic;
using Tournament;
using Tournament.Server;
using UnityEngine;

namespace Barebones.MasterServer {

    public class AlbotRunningTournamentModule : ServerModuleBehaviour {

        private static AlbotRunningTournamentModule singleton;
        private Dictionary<string, RunningTournamentGame> runningTournaments = new Dictionary<string, RunningTournamentGame>();

        public override void Initialize(IServer server) {
            singleton = this;
            server.SetHandler((short)CustomMasterServerMSG.tournamentRoundPreStarted, handlePreGameStarted);
            server.SetHandler((short)CustomMasterServerMSG.tournamentRoundStarted, handleRoundStarted);
        }

        private void handlePreGameStarted(IIncommingMessage rawMsg) {
            TournamentPreGameInfo info = rawMsg.Deserialize<TournamentPreGameInfo>();
            RunningTournamentGame game;
            if (findGame(info.tournamentID, out game, rawMsg) && SpectatorAuthModule.existsAdmin(rawMsg.Peer))
                game.startRoundPreGame(info.roundID);
        }

        private void handleRoundStarted(IIncommingMessage rawMsg) {
            TournamentPreGameInfo info = rawMsg.Deserialize<TournamentPreGameInfo>();
            RunningTournamentGame game;
            if (findGame(info.tournamentID, out game, rawMsg) && SpectatorAuthModule.existsAdmin(rawMsg.Peer))
                game.startRound(info.roundID);
        }

        private void updateRound(string tournamentID, RoundID roundID) {
            RunningTournamentGame game;
            if (findGame(tournamentID, out game))
                game.updateRound(roundID);
        }

        public void closeTournament(string roomID) {
            RunningTournamentGame game;
            if (findGame(roomID, out game) == false) 
                return;

            Debug.LogError("Close tournament is not implemented");
        }
        


        private bool findGame(string roomID, out RunningTournamentGame game, IIncommingMessage rawMsg = null, string error = "Could not find game: ") {
            if(runningTournaments.TryGetValue(roomID, out game))
                return true;
            if (rawMsg != null)
                rawMsg.Respond(error + roomID, ResponseStatus.Error);
            return false;
        }


        #region Static Utils
        public static void addNewRunningGame(string roomID, RunningTournamentGame runningGame) {
            Debug.LogError("Register tournament: " + roomID);
            singleton.runningTournaments.Add(roomID, runningGame);
        }
        public static void handleGameResult(string tournamentID, RoundID roundID, GameOverMsg result) {
            RunningTournamentGame game;
            if (singleton.findGame(tournamentID, out game))
                game.reportRoundResult(roundID, result);
        }
        public static void handleUpdateRound(string tournamentID, RoundID roundID) {singleton.updateRound(tournamentID, roundID);}
        public static void handleCloseTournament(string roomID) {singleton.closeTournament(roomID);}
        public static bool containsKey(string roomID) { return singleton.runningTournaments.ContainsKey(roomID); }
        #endregion
    }
}