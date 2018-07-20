using Barebones.MasterServer;
using Barebones.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Tournament.Server {


    public class RunningTournamentGame {

        private TournamentTree gameTree;
        private TournamentGameInfo gameInfo;

        public RunningTournamentGame(TournamentGameInfo gameInfo) {
            this.gameInfo = gameInfo;
            gameTree = new TournamentTree(gameInfo.players);
        }

        private void updateFullTree() { for (int i = 0; i < gameTree.layers; i++) updateEveryone(i); }
        private void updateEveryone(int row) {
            TournamentTreeRow rowMsg = gameTree.createRowDTO(row);
            gameInfo.connectedPeers.ForEach(p => p.SendMessage((short)CustomMasterServerMSG.runningTournamentUpdate, rowMsg));
            gameInfo.admin.SendMessage((short)CustomMasterServerMSG.runningTournamentUpdate, rowMsg);
        }

    }

    public struct TournamentGameInfo {
        public IPeer admin;
        public string hostName, roomID;
        public TournamentSpecsMsg specs;
        public List<IPeer> connectedPeers;
        public List<TournamentPlayer> players;
    }

}