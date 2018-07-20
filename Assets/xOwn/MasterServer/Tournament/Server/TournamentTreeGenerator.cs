using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tournament.Server {

    public class TournamentTreeGenerator {


        public static List<List<TournamentRound>> generateTreeStructure(int layers) {
            List<List<TournamentRound>> treeStructure = new List<List<TournamentRound>>();

            for (int i = 0; i < layers; i++) {
                List<TournamentRound> col = new List<TournamentRound>();
                for (int j = 0; j < Mathf.Pow(2, layers - i - 1); j++)  //Create new game Row
                    col.Add(new TournamentRound(i, j));

                treeStructure.Add(col);
                if (i == 0) 
                    continue;

                //Link previous layer
                List<TournamentRound> prevRow = treeStructure[treeStructure.Count - 2];
                for (int j = 0; j < col.Count; j++) {
                    prevRow[j * 2].setNextGame(col[j]);
                    prevRow[j * 2 + 1].setNextGame(col[j]);
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
    }
}