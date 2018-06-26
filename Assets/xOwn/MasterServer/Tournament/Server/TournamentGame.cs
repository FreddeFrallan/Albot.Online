using Barebones.MasterServer;
using Barebones.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tournament.Server {

    public class TournamentGame {

        private TournamentSpecsMsg specs;
        private IPeer admin;
        private string hostName, roomID;
        private List<IPeer> connectedPlayers = new List<IPeer>();
        private List<TournamentPlayer> players = new List<TournamentPlayer>();

        public TournamentGame(TournamentSpecsMsg specs, IPeer admin, string roomID) {
            this.specs = specs;
            this.admin = admin;
            this.roomID = roomID;
        }

        public void addPlayer() {

        }

        public void removePlayer(int peerID) {
       
        }

        #region Compression
        public GameInfoPacket convertToGameInfoPacket() {
            return Msf.Helper.createGameInfoPacket(GameInfoType.PreTournament, roomID, hostName, specs.maxPlayers, players.Count, specs.type);
        }
        #endregion
    }

}