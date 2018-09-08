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
                List<TournamentRound> newLayer = createLayer((int)Mathf.Pow(2, layers - col - 1), col, gameSpecs);
                treeStructure.Add(newLayer);
                if (col == 0) 
                    continue;

                fullyLinkLayers(treeStructure[col - 1], newLayer); //Link previous layer
            }
            return treeStructure;
        }

        private static List<TournamentRound> createLayer(int size, int col, PreGameSpecs gameSpecs) {
            List<TournamentRound> layer = new List<TournamentRound>();
            for (int row = 0; row < size; row++)  //Create new game Row
                layer.Add(new TournamentRound(col, row, gameSpecs));

            return layer;
        }

        private static void fullyLinkLayers(List<TournamentRound> previous, List<TournamentRound> next) {
            for (int i = 0; i < next.Count; i++) {
                previous[i * 2].setNextGame(next[i]);
                previous[i * 2 + 1].setNextGame(next[i]);
            }
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

        //Creates 
        public static List<List<TournamentRound>> generateLoserBrackets(List<List<TournamentRound>> tree, PreGameSpecs specs) {
            int firstLayerSize = tree[0].Count / 2;
            List<List<TournamentRound>> loserTree = new List<List<TournamentRound>>();
            loserTree.Add(createLayer(firstLayerSize, 0, specs));

            int treeCounter = 1, loserCounter = 1;
            while(treeCounter < tree.Count) {
                List<TournamentRound> treeLayer = tree[treeCounter];
                List<TournamentRound> loserLayer = loserTree[loserCounter - 1];

                float combinedSize = (float)loserLayer.Count / 2 + (float)treeLayer.Count / 2;
                if (Mathf.Log(combinedSize, 2) % 1 == 0) { //Check if our new layer can be integrated with the normal tree
                    List<TournamentRound> mergedLayer = createLayer((int)combinedSize, loserCounter, specs);
                    loserTree.Add(mergedLayer);

                    for (int i = 0; i < loserLayer.Count; i++) { //Link the previous layer but expect the losing players to join.
                        loserLayer[i].setNextGame(mergedLayer[i]);
                        treeLayer[i].setNextLoserGame(mergedLayer[i]);
                    }

                    treeCounter++; loserCounter++;
                    continue;
                }
                //Since we can't just combine with the normal trees layer we need to create another layer that's just internat to our loser bracket

                List<TournamentRound> newLayer = createLayer(loserLayer.Count / 2, loserCounter, specs);
                loserTree.Add(newLayer);
                fullyLinkLayers(loserLayer, newLayer);
                loserCounter++; // Since we did not merge with original tree we only increment losers

                if (newLayer.Count == 1)//If we have ghost layer with size 1, we have reached the pre final game
                    break;
            }

            //Add Final game that combines the two trees
            List<TournamentRound> finalLayer = createLayer(1, loserTree.Count, specs);
            tree[tree.Count - 1][0].setNextGame(finalLayer[0]);
            loserTree[loserTree.Count - 1][0].setNextGame(finalLayer[0]);
            loserTree.Add(finalLayer);

            return loserTree;
        }


    }
}