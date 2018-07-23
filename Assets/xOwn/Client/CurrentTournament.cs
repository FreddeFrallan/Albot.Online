using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;
using Barebones.Networking;

namespace ClientUI {

    public class CurrentTournament {

        private static string tournamentID = "";

        public static void init() {
            Msf.Server.SetHandler((short)CustomMasterServerMSG.startTournament, CurrentTournament.handleTournamentStarted);
            Msf.Server.SetHandler((short)CustomMasterServerMSG.closeTournament, CurrentTournament.handleTournamentClosed);
        }

        private static void handleTournamentStarted(IIncommingMessage rawMsg) {
            Debug.Log("Tournament Started");
        }

        public static void handleJoinedTournament(ResponseStatus status, IIncommingMessage rawMsg) {
            if (Msf.Helper.serverResponseSuccess(status, rawMsg) == false)
                return;
            Debug.Log("Tournament Join: " + rawMsg.AsString());
            tournamentID = rawMsg.AsString();
        }

        public static void leaveCurrentTournament() {
            Debug.Log("Leaving tournament");
            Msf.Connection.SendMessage((short)CustomMasterServerMSG.leaveTournament);
        }

        private static void handleTournamentClosed(IIncommingMessage rawMsg) {
            if (rawMsg.AsString() != tournamentID)
                return;
            Debug.Log("Tournament forcefully Closed");
            tournamentID = "";
        }
    }

}