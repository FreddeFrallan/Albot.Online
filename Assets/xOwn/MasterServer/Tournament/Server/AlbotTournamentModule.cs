using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using Tournament.Server;
using Barebones.Networking;
using UnityEngine.Networking;
using System.Linq;
using AlbotServer;
using System;

namespace Barebones.MasterServer {

    public class AlbotTournamentModule : ServerModuleBehaviour {

        private static AlbotTournamentModule singleton;
        private Dictionary<string, PreTournamentGame> currentPreTournaments = new Dictionary<string, PreTournamentGame>();
        private Dictionary<string, RunningTournamentGame> currentRunningTournaments = new Dictionary<string, RunningTournamentGame>();
        private Dictionary<int, PreTournamentGame> peerToTournaments = new Dictionary<int, PreTournamentGame>();

        public override void Initialize(IServer server) {
            singleton = this;
            server.SetHandler((short)CustomMasterServerMSG.createTournament, handleCreateTournament);
            server.SetHandler((short)CustomMasterServerMSG.startTournament, handleStartTournament);
            server.SetHandler((short)CustomMasterServerMSG.closeTournament, handleCloseTournament);
            server.SetHandler((short)CustomMasterServerMSG.joinTournament, handleJoinTournament);
            server.SetHandler((short)CustomMasterServerMSG.leaveTournament, handlePlayerLeft);
        }

        #region Create
        public void handleCreateTournament(IIncommingMessage rawMsg) {
            IPeer peer = rawMsg.Peer;
            if (SpectatorAuthModule.existsAdmin(peer) == false) {
                rawMsg.Respond("You don't have admin permission", ResponseStatus.Failed);
                return;
            }

            TournamentSpecsMsg msg =  rawMsg.Deserialize<TournamentSpecsMsg>();
            string key = generateGameKey();
            PreTournamentGame newTournament = new PreTournamentGame(msg, peer, key);

            currentPreTournaments.Add(key, newTournament);
            addPlayerToPeerToTournaments(rawMsg.Peer, newTournament);
            rawMsg.Respond(key, ResponseStatus.Success);
            newTournament.updateAdmin();
            Debug.LogError("Created Tournament: " + key + " With admin: " + rawMsg.Peer.Id);
        }
        #endregion


        #region Join
        public void handleJoinTournament(IIncommingMessage rawMsg) {
            PreGameJoinRequest msg = rawMsg.Deserialize<PreGameJoinRequest>();
            if(currentPreTournaments.ContainsKey(msg.roomID) == false) {
                Debug.LogError("Tried to join tournament " + msg.roomID + ", but no such tournament exist");
                rawMsg.Respond("Game does not exist", ResponseStatus.Error);
                return;
            }


            PreTournamentGame tournament = currentPreTournaments[msg.roomID];
            if (tournament.addPlayer(rawMsg.Peer, msg.joiningPlayer)) {
                rawMsg.Respond(msg.roomID, ResponseStatus.Success);
                addPlayerToPeerToTournaments(rawMsg.Peer, tournament);
                Debug.LogError("Adding player: " + rawMsg.Peer.Id + " to tournament: " + tournament.getRoomID());
            }
            else
                rawMsg.Respond("Could not join game", ResponseStatus.Failed);
        }


        private void addPlayerToPeerToTournaments(IPeer peer, PreTournamentGame newTournament) {
            if (peerToTournaments.ContainsKey(peer.Id) == false) {
                peerToTournaments.Add(peer.Id, newTournament);
                return;
            }

            PreTournamentGame oldTournament = peerToTournaments[peer.Id];
            if (SpectatorAuthModule.existsAdmin(peer)) { //Admin
                closeTournament(oldTournament);
                peerToTournaments.Remove(peer.Id);
            } else {//User
                if (oldTournament.getRoomID() != newTournament.getRoomID()) 
                    removePlayerFromGame(peer, oldTournament);
                else
                    return;
            }

            peerToTournaments.Add(peer.Id, newTournament);
        }
        #endregion

        #region Start

        public void handleStartTournament(IIncommingMessage rawMsg) {
            string roomID = rawMsg.AsString();
            if (currentPreTournaments.ContainsKey(roomID) == false) {
                rawMsg.Respond("Tried to start tournament " + roomID + ", but no such tournament exist", ResponseStatus.Error);
                return;
            }

            PreTournamentGame preGame = currentPreTournaments[roomID];
            RunningTournamentGame runningGame = null;
            if (preGame.startTournament(ref runningGame, rawMsg) == false)
                return;

            currentPreTournaments.Remove(roomID);
            currentRunningTournaments.Add(roomID, runningGame);
        }

        #endregion

        #region Close
        public void handleCloseTournament(IIncommingMessage rawMsg) {handleCloseTournament(rawMsg.Peer, rawMsg.AsString());}
        public void handleCloseTournament(IPeer peer, string key) {
            if(currentPreTournaments.ContainsKey(key) == false) {
                Debug.LogError("Tried to remove tournament " + key + ", but no such tournament exist");
                return;
            }
            closeTournament(currentPreTournaments[key], key);
        }
        public void closeTournament(PreTournamentGame game) { closeTournament(game, getGameKey(game)); }
        public void closeTournament(PreTournamentGame game, string key) {
            game.closeTournament();

            currentPreTournaments.Remove(key);
            peerToTournaments.Remove(game.getAdmin().Id);
            foreach (int id in game.getPeerIDs())
                peerToTournaments.Remove(id);
        }
        #endregion

        private void handlePlayerLeft(IIncommingMessage rawMsg) {handlePlayerLeft(rawMsg.Peer);}
        private void handlePlayerLeft(IPeer p) {
            if (peerToTournaments.ContainsKey(p.Id) == false) {
                Debug.LogError(p.Id + " tried to leave tournament, but no such player registred");
                return;
            }
            removePlayerFromGame(p, peerToTournaments[p.Id]);
        }

        private void removePlayerFromGame(IPeer p, PreTournamentGame tournament) {
            tournament.removePlayer(p.Id);
            peerToTournaments.Remove(p.Id);
        }

        #region Helpers
        private string getGameKey(PreTournamentGame game) { return game.getRoomID(); }
        private string generateGameKey() {
            return Msf.Helper.CreateRandomStringMatch(MasterServerConstants.KEY_LENGTH, (key) => { return currentPreTournaments.ContainsKey(key) || currentRunningTournaments.ContainsKey(key); });
        }
        #endregion

        public static List<PreTournamentGame> getCurrentTournaments() { return singleton.currentPreTournaments.Values.ToList();}
    }




    public class TournamentSpecsMsg : MessageBase {
        public GameType type;
        public string tournamentID;
        public int maxPlayers;
    }

    public class PreTournamentInfo : MessageBase {
        public PlayerInfo[] players;
    }

}
