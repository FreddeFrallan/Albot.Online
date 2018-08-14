using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tournament.Server;
using AlbotServer;

namespace Tournament.Client {

    public class VisualTournamentTree : MonoBehaviour {

        [SerializeField]
        private GameObject gamePrefab;
        private List<List<VisualTournamentRound>> tree = new List<List<VisualTournamentRound>>();
        private TournamentTree tournament;

        private float firstColSpacing = 2;
        private float rowSpacing = 8;
        private int rowCounter = 0;

        public void init(PlayerInfo[] playerOrder, PreGameSpecs gameSpecs) {
            generateTree(playerOrder, gameSpecs);
        }

        private void generateTree(PlayerInfo[] playerOrder, PreGameSpecs gameSpecs) {
            List<TournamentPlayer> players = new List<TournamentPlayer>();
            foreach(PlayerInfo p in playerOrder)
                players.Add(new TournamentPlayer() {info = p});

            tournament = new TournamentTree(players, gameSpecs, false);
            renderVisualTree();
        }



        public void updateRounds(TournamentRoundDTO[] rounds) {
            foreach (TournamentRoundDTO r in rounds)
                tournament.getRound(r.ID).setToTournamentDTO(r);
            renderVisualTree();
        }

        #region Rendering
        public void renderVisualTree() {
            clearOldTree();

            List<List<TournamentRound>> serverTree = tournament.getTree();
            float startY = 0, yIncrement = firstColSpacing;

            for (int col = 0; col < serverTree.Count; col++) {
                List<VisualTournamentRound> visualCol = new List<VisualTournamentRound>();

                for (int i = 0; i < serverTree[col].Count; i++) {
                    Vector3 spawnPos = new Vector3(col * rowSpacing, startY + yIncrement * i, 0);
                    GameObject tempObj = Instantiate(gamePrefab, spawnPos, Quaternion.identity);

                    VisualTournamentRound game = tempObj.GetComponent<VisualTournamentRound>();
                    game.init(serverTree[col][i]);
                    visualCol.Add(game);
                }

                startY += yIncrement * 0.5f;
                yIncrement *= 2;
                tree.Add(visualCol);
            }
        }

        private void clearOldTree() {
            foreach (List<VisualTournamentRound> col in tree)
                foreach (VisualTournamentRound game in col)
                    if(game != null)
                        Destroy(game.gameObject);
            tree.Clear();
        }
        #endregion
    }
}