using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlbotServer;
using Tournament.Server;

namespace Tournament.Client {

    public class VisualTournamentRound : MonoBehaviour {

        [SerializeField]
        private List<MeshRenderer> playerSlots;
        [SerializeField]
        private RoundState state = RoundState.Idle;
        private RoundID id;
        private List<TournamentPlayer> players;

        public List<string> playersDebugList = new List<string>();


        public void init(TournamentRound serverRound) {
            setState(serverRound.getState());
            id = serverRound.getGameID();
            players = serverRound.getPlayers();
            setPlayerSlots();

            updateDebugList();
        }

        private void updateDebugList() {
            playersDebugList.Clear();
            foreach (TournamentPlayer p in players)
                playersDebugList.Add(p.info.username);
        }

        private void setPlayerSlots() {
            for (int i = 0; i < players.Count; i++)
                playerSlots[i].material.color =  players[i].getIsNPC() ? Color.yellow : Color.blue;
        }


        public void setState(RoundState state) {
            this.state = state;

            switch (state) {
                case RoundState.Idle: setColor(Color.white); break;
                case RoundState.Lobby: setColor(Color.blue); break;
                case RoundState.Playing: setColor(Color.red); break;
                case RoundState.Over: setColor(Color.yellow); break;
            }
        }

        private void setColor(Color c) {
            //GetComponent<MeshRenderer>().material.color = c; 
        }
    }
}