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
        private GameState state = GameState.Empty;
        private GameID id;
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


        public void setState(GameState state) {
            this.state = state;

            switch (state) {
                case GameState.Empty: setColor(Color.white); break;
                case GameState.Lobby: setColor(Color.blue); break;
                case GameState.Playing: setColor(Color.red); break;
                case GameState.Over: setColor(Color.yellow); break;
            }
        }

        private void setColor(Color c) {
            //GetComponent<MeshRenderer>().material.color = c; 
        }
    }
}