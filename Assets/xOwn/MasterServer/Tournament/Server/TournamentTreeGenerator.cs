using AlbotServer;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tournament.Server {

    public class TournamentTreeGenerator {


        public static List<List<TournamentRound>> generateTreeStructure(int layers, PreGameSpecs gameSpecs) {
            List<List<TournamentRound>> treeStructure = new List<List<TournamentRound>>();

            for (int col = 0; col < layers; col++) {
                List<TournamentRound> layer = new List<TournamentRound>();
                for (int row = 0; row < Mathf.Pow(2, layers - col - 1); row++)  //Create new game Row
                    layer.Add(new TournamentRound(col, row, gameSpecs));

                treeStructure.Add(layer);
                if (col == 0) 
                    continue;

                //Link previous layer
                List<TournamentRound> prevCol = treeStructure[col - 1];
                for (int j = 0; j < layer.Count; j++) {
                    prevCol[j * 2].setNextGame(layer[j]);
                    prevCol[j * 2 + 1].setNextGame(layer[j]);
                }
            }
            return treeStructure;
        }


        public static void addBots(List<List<TournamentRound>> tree, List<TournamentPlayer> players) {
            List<TournamentRound> firstRow = tree[0];
            int totalPlayers = firstRow.Count * 2;
            int botsNeeded = totalPlayers - players.Count;

            for (int i = 0; i < botsNeeded; i++) {
                List<TournamentRound> temp = firstRow.OrderByDescending(g => g.calcRoundsUntilPossibleBotGame()).ToList(); //Calculate round to bot battle
                temp = temp.Where(g => g.stashedRoundsUntilBotGame == temp[0].stashedRoundsUntilBotGame).ToList(); //Sort

                TournamentRound round = temp[Random.Range(0, temp.Count)]; //Pick random game
                TournamentPlayer bot = new TournamentPlayer(true, i);
                players.Add(bot);
                round.addPlayer(bot);
            }
        }


        public static void insertPlayersRandomly(List<List<TournamentRound>> tree, List<TournamentPlayer> players) {
            List<TournamentRound> firstRow = tree[0];
            foreach (TournamentPlayer p in players.Where(p => p.getIsNPC() == false)) {
                TournamentRound game = firstRow.OrderBy(g => g.getPlayers().Count).ToList()[0];
                game.addPlayer(p);
            }
        }

        public static void insertPlayersLinear(List<List<TournamentRound>> tree, List<TournamentPlayer> players) {
            for (int i = 0; i < players.Count; i++) 
                tree[0][i / 2].addPlayer(players[i]);
        }
    }
}