using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using AlbotServer;
using Barebones.Networking;

namespace Tournament.Server {

    public class TournamentTree {
        private List<TournamentRound> currentGames = new List<TournamentRound>();
        private List<List<TournamentRound>> treeStructure = new List<List<TournamentRound>>();
        private List<TournamentPlayer> players;
        private int layers;

        public TournamentTree(List<TournamentPlayer> players) {
            this.players = players;
            layers = (int)Mathf.Ceil(Mathf.Log(players.Count, 2));

            treeStructure = TournamentTreeGenerator.generateTreeStructure(layers);
            TournamentTreeGenerator.addBots(treeStructure, players);
            TournamentTreeGenerator.insertPlayersRandomly(treeStructure, players);
        }

        public void playRow(int row) {
            foreach (TournamentRound g in treeStructure[row])
                g.startGame();
        }

        public void playGame(int col, int row) {
            treeStructure[col][row].startGame();
        }

        public List<List<TournamentRound>> getTree() { return treeStructure; }
    }





    public class TournamentPlayer {
        public IPeer peer;
        public PlayerInfo info;
        private bool isNPC = false;

        public TournamentPlayer(bool isNPC = false, int botNumber = 0) {
            this.isNPC = isNPC;
            if (isNPC)
                info = new PlayerInfo() { username = "Bot: " + botNumber.ToString() };
        }

        public bool getIsNPC() { return isNPC; }
    }

    public enum GameState {
        Playing,
        Lobby,
        Empty,
        Over,
    }
}