using AlbotServer;
using System.Collections;
using System.Collections.Generic;
using Tournament.Client;
using Tournament.Server;
using UnityEngine;


namespace Tournament {


    public class TournamentTest : MonoBehaviour {

        private string[] names = {"Tom", "Barty", "Nike", "Swedluden", "Schmidth", "Pearly", "Gaspa", "Bytelz", "Schamanz",
        "Ubenstrauff", "David", "Erik", "Gombi", "Uluf", "Azerbajjn", "Kotlin", "Linux"};

        public VisualTournamentTree visualTree;
        private TournamentTree tree;
        public int amountOfPlayers = 16;

        // Update is called once per frame
        void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                List<TournamentPlayer> players = createFakePlayers(amountOfPlayers);
                tree = new TournamentTree(players, new PreGameSpecs(), true, true);

                visualTree.init(tree.getPlayerOrder(), new PreGameSpecs(), true);
                List<List<TournamentRound>> losersTree = tree.getLosersTree();
            }

        }

        private List<TournamentPlayer> createFakePlayers(int size) {
            List<TournamentPlayer> players = new List<TournamentPlayer>();
            for (int i = 0; i < size; i++) {
                PlayerInfo info = new PlayerInfo() {username = names[i], iconNumber = 3, isNPC = false };
                players.Add(new TournamentPlayer() { info = info});
            }
            return players;
        }
    }
}