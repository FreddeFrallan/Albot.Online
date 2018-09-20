using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tournament.Server;
using AlbotServer;
using System.Linq;

namespace Tournament.Client {

    public class VisualTournamentTree : MonoBehaviour {

        [SerializeField]
        private GameObject gamePrefab;
        private List<List<VisualTournamentRound>> tree = new List<List<VisualTournamentRound>>();
        private TournamentTree tournament;

        private float firstRowSpacing = 3;
        private float colSpacing = 8;
        private int rowCounter = 0;

        public void init(PlayerInfo[] playerOrder, PreGameSpecs gameSpecs, bool doubleElimination = false) {
            generateTree(playerOrder, gameSpecs, doubleElimination);
        }

        private void generateTree(PlayerInfo[] playerOrder, PreGameSpecs gameSpecs, bool doubleElimination) {
            List<TournamentPlayer> players = new List<TournamentPlayer>();
            for(int i = 0; i < playerOrder.Length; i+= 2) {
                players.Add(new TournamentPlayer() {info = playerOrder[i], slotIndex = 0});
                players.Add(new TournamentPlayer() { info = playerOrder[i+1], slotIndex = 1});
            }

            tournament = new TournamentTree(players, gameSpecs, false, doubleElimination);
            renderVisualTree();
        }



        public void updateRounds(TournamentRoundDTO[] rounds, bool doDisplay) {
            foreach (TournamentRoundDTO r in rounds)
                tournament.getRound(r.ID).setToTournamentDTO(r);
            if(doDisplay)
                renderVisualTree();
        }

        #region Rendering
        public void renderVisualTree() {
            clearOldTree();

            List<List<TournamentRound>> serverTree = tournament.getTree();
            float roundHeight = 3f; //Should not have to be hardcoded here

            //Create appropirate y spacing in the rows
            int biggestLayerSize = serverTree.OrderByDescending(l => l.Count).ToArray()[0].Count;
            Dictionary<int, float> layerSizeToYIncrement = new Dictionary<int, float>();
            int counter = 0;
            for(int i = biggestLayerSize; i > 0; i /= 2) {
                layerSizeToYIncrement.Add(i, Mathf.Pow(2, counter) * firstRowSpacing);
                counter++;
            }


            for (int col = 0; col < serverTree.Count; col++) {
                float yIncrement = layerSizeToYIncrement[serverTree[col].Count];
                List<VisualTournamentRound> visualCol = new List<VisualTournamentRound>();
                float rowHeight = (serverTree[col].Count - 1) * yIncrement + roundHeight;
                float startY = -rowHeight / 2;

                for (int i = 0; i < serverTree[col].Count; i++) {
                    Vector3 spawnPos = new Vector3(col * colSpacing, startY + yIncrement * i, 0);
                    GameObject tempObj = Instantiate(gamePrefab, spawnPos, Quaternion.identity);

                    VisualTournamentRound game = tempObj.GetComponent<VisualTournamentRound>();
                    game.init(serverTree[col][i]);
                    visualCol.Add(game);
                }
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