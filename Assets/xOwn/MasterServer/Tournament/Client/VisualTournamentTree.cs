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

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                generateTree(13);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                generateTree(3);
            if (Input.GetKeyDown(KeyCode.Alpha4))
                generateTree(4);
            if (Input.GetKeyDown(KeyCode.Alpha5))
                generateTree(5);
            if (Input.GetKeyDown(KeyCode.Alpha6))
                generateTree(6);
            if (Input.GetKeyDown(KeyCode.Alpha7))
                generateTree(7);
            if (Input.GetKeyDown(KeyCode.Alpha8))
                generateTree(8);
            if (Input.GetKeyDown(KeyCode.Alpha9))
                generateTree(9);

            if (Input.GetKeyDown(KeyCode.Space)) {
                tournament.playRow(rowCounter++);
                renderVisualTree(tournament);
            }
        }

        private void generateTree(int amount) {
            List<TournamentPlayer> players = new List<TournamentPlayer>();
            for (int i = 0; i < amount; i++)
                players.Add(new TournamentPlayer() {
                    info = new PlayerInfo() { username = i.ToString() }
                });

            tournament = new TournamentTree(players);
            renderVisualTree(tournament);
        }



        public void renderVisualTree(TournamentTree tournament) {
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
                    Destroy(game.gameObject);
            tree.Clear();
        }
    }
}