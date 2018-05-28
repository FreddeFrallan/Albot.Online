using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpeedRunner {

    public class SpeedRunnerUpdater{

        private List<ConnectedPlayer> players;
        private SpeedRunnerGameMaster master;
        private SpeedRunnerProtocol protocol;

        public SpeedRunnerUpdater(List<ConnectedPlayer> players, SpeedRunnerGameMaster master, SpeedRunnerProtocol protocol) {
            this.players = players;
            this.master = master;
            this.protocol = protocol;
        }

        public void sendGameUpdate() {

        }


        private void broadcastUpdate(GameUpdate update) {
            foreach(ConnectedPlayer p in players) {
                try {protocol.sendGameUpdate(master.getMatchingPlayer(p.color).client.peerID, update);
                } catch { Debug.LogError("Error sending broadcast"); }
            }
        }
    }
}