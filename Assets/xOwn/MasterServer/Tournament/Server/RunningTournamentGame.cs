using Barebones.MasterServer;
using Barebones.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using AlbotServer;

namespace Tournament.Server {


    public class RunningTournamentGame {

        private TournamentTree gameTree;
        private TournamentGameInfo gameInfo;
        private PreGameSpecs gameSpecs;

        public RunningTournamentGame(TournamentGameInfo gameInfo, PreGameSpecs gameSpecs, bool doubleElemination) {
            this.gameInfo = gameInfo;
            this.gameSpecs = gameSpecs;
            gameTree = new TournamentTree(gameInfo.players, gameSpecs, true, doubleElemination);
            gameTree.traverseRounds(r => r.setServerVariables(gameInfo.admin, this));
            this.gameInfo.specs.players = gameTree.getPlayerOrder();
        }

        #region Starting Games
        public void startRoundPreGame(RoundID roundID, bool forceRestart) {gameTree.getRound(roundID).initAndInvite(forceRestart);}
        public void startRound(RoundID roundID) {gameTree.getRound(roundID).startGame();}
        #endregion

        #region Updates
        public void reportRoundResult(RoundID id, GameOverMsg result) {gameTree.getRound(id).reportResult(result);}
        public void updateRound(RoundID id) {updateEveryone(new List<RoundID>() {id });}
        public void updateRounds(List<RoundID> IDs) { updateEveryone(IDs); }

        private void updateEveryone(List<RoundID> IDs) {
            TournamentTreeUpdate updateMsg = new TournamentTreeUpdate() {
                rounds = IDs.Select(r => gameTree.getRoundDTO(r)).ToArray()
            };

            gameInfo.connectedPeers.ForEach(p => p.SendMessage((short)CustomMasterServerMSG.runningTournamentUpdate, updateMsg));
            gameInfo.admin.SendMessage((short)CustomMasterServerMSG.runningTournamentUpdate, updateMsg);
        }

        public void forceIndexWinner(RoundID roundID, int index) {
            gameTree.getRound(roundID).forceIndexWinner(index);
        }
        #endregion

        private void playerDissconnected(IPeer peer) {
            Debug.LogError("Peer dissconnected from running tournament: " + peer.Id); 
        }
    }

    public struct TournamentGameInfo {
        public IPeer admin;
        public string hostName, roomID;
        public TournamentInfoMsg specs;
        public List<IPeer> connectedPeers;
        public List<TournamentPlayer> players;
        public bool doubleElimination;
    }



}