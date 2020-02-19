using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using AlbotServer;
using Barebones.Networking;
using UnityEngine.Networking;
using AlbotServer;
using System;
using Game;

namespace Tournament.Server {

    public class TournamentTree{
        private List<TournamentRound> currentGames = new List<TournamentRound>();
        private List<List<TournamentRound>> treeStructure = new List<List<TournamentRound>>();
        private List<List<TournamentRound>> losersTree = new List<List<TournamentRound>>();
        public List<TournamentPlayer> players { get; private set; }
        public int layers { get; private set; }

        public TournamentTree(List<TournamentPlayer> players, PreGameSpecs gameSpecs, bool insertRandomly = true, bool doubleElimination = false) {
            this.players = players;
            layers = (int)Mathf.Ceil(Mathf.Log(players.Count, 2));
            layers = layers <= 0 ? 1 : layers;

            treeStructure = TournamentTreeGenerator.generateTreeStructure(layers, gameSpecs);
            TournamentTreeGenerator.addBots(treeStructure, this.players);

            if (doubleElimination && players.Count >= 4) {
                losersTree = TournamentTreeGenerator.generateLoserBrackets(treeStructure, gameSpecs);
                for (int i = 0; i < losersTree.Count; i++)// Add losers bracket to treeStructure
                    treeStructure.Add(losersTree[losersTree.Count - 1 - i]);
                resetRoundIDs();
            }

            if (insertRandomly)
                TournamentTreeGenerator.insertPlayersRandomly(treeStructure, this.players);
            else
                TournamentTreeGenerator.insertPlayersLinear(treeStructure, this.players);
        }

        private void resetRoundIDs() {
            for (int col = 0; col < treeStructure.Count; col++)
                for (int row = 0; row < treeStructure[col].Count; row++)
                    treeStructure[col][row].setNewId(col, row);
        }

        public void playGame(int col, int row) {treeStructure[col][row].startGame();}
        public void playRow(int row) {
            foreach (TournamentRound g in treeStructure[row])
                g.startGame();
        }


        public TournamentRoundDTO getRoundDTO(RoundID id) {return getRound(id).createDTO();}
        public TournamentRound getRound(RoundID ID) { return getRound(ID.col, ID.row); }
        public TournamentRound getRound(int col, int row) {return treeStructure[col][row];}
        public List<List<TournamentRound>> getTree() { return treeStructure; }
        public List<List<TournamentRound>> getLosersTree() { return losersTree; }
        public PlayerInfo[] getPlayerOrder() {
            List<PlayerInfo> players = new List<PlayerInfo>();
            foreach (TournamentRound r in treeStructure[0])
                players.AddRange(r.getPlayers().Select(p => p.info));
            return players.ToArray();
        }


        public void traverseRounds(Action<TournamentRound> a) {
            foreach (List<TournamentRound> col in treeStructure)
                foreach (TournamentRound round in col)
                    a(round);
        }
    }






    public class TournamentPlayer {
        public IPeer peer;
        public PlayerInfo info;
        public bool isReady;
        public bool isWinning = false;
        public int slotIndex;

        public TournamentPlayer(bool isNPC = false, int botNumber = 0) {
            if (isNPC) {
                info = new PlayerInfo() {
                    username = LocalTrainingBots.botName,
                    iconNumber = LocalTrainingBots.botIconNumber,
                    isNPC = true
                };
                isReady = true;
            }
        }

        public bool getIsNPC() { return info.isNPC; }
    }

    public enum RoundState {
        Playing,
        Lobby,
        Idle,
        Empty,
        Over,
    }

    public enum RoundType {
        normal,
        final,
        loser
    }
}