using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using Tournament.Server;
using Barebones.Networking;
using UnityEngine.Networking;
using System.Linq;
using AlbotServer;

namespace Barebones.MasterServer {

    public class AlbotTournamentModule : ServerModuleBehaviour {

        private static AlbotTournamentModule singleton;
        private Dictionary<string, TournamentGame> currentTournaments = new Dictionary<string, TournamentGame>();
        private Dictionary<int, TournamentGame> peerToTournaments = new Dictionary<int, TournamentGame>();

        public override void Initialize(IServer server) {
            singleton = this;
            server.SetHandler((short)CustomMasterServerMSG.createTournament, handleCreateTournament);
            server.SetHandler((short)CustomMasterServerMSG.closeTournament, handleCloseTournament);
            server.SetHandler((short)CustomMasterServerMSG.leaveTournament, handlePlayerLeft);
        }

        #region Create
        public void handleCreateTournament(IIncommingMessage rawMsg) {
            if (SpectatorAuthModule.existsAdmin(rawMsg.Peer) == false) {
                rawMsg.Respond("You don't have admin permission", ResponseStatus.Failed);
                return;
            }
            TournamentSpecsMsg msg =  rawMsg.Deserialize<TournamentSpecsMsg>();
            string key = generateGameKey();
            currentTournaments.Add(key, new TournamentGame(msg, rawMsg.Peer, key));

            rawMsg.Respond(key, ResponseStatus.Success);
        }
        #endregion

        #region Close
        public void handleCloseTournament(IIncommingMessage rawMsg) {handleCloseTournament(rawMsg.Peer, rawMsg.AsString());}
        public void handleCloseTournament(IPeer peer, string key) {
            if(currentTournaments.ContainsKey(key) == false) {
                Debug.LogError("Tried to remove tournament " + key + ", but no such tournament exist");
                return;
            }
            currentTournaments.Remove(key);
        }
        #endregion

        private void handlePlayerLeft(IIncommingMessage rawMsg) {handlePlayerLeft(rawMsg.Peer);}
        private void handlePlayerLeft(IPeer p) {
            if (peerToTournaments.ContainsKey(p.Id) == false) {
                Debug.LogError(p.Id + " tried to leave tournament, but no such player registred");
                return;
            }
            peerToTournaments[p.Id].removePlayer(p.Id);
            peerToTournaments.Remove(p.Id);
        }

        private string generateGameKey() {
            return Msf.Helper.CreateRandomStringMatch(MasterServerConstants.KEY_LENGTH, (key) => { return currentTournaments.ContainsKey(key); });
        }

        public static List<TournamentGame> getCurrentTournaments() { return singleton.currentTournaments.Values.ToList();}
    }




    public class TournamentSpecsMsg : MessageBase {
        public GameType type;
       // public IPeer admin;
       // public TournamentPlayer[] players;
        public int maxPlayers;
    }


}
