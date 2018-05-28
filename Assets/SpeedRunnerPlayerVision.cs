using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SpeedRunner {

    public class SpeedRunnerPlayerVision : MonoBehaviour {

        [SerializeField]
        private Transform player;
        [SerializeField]
        private GameObject sightNodePrefab;

        private SpeedRunnerSightNode[,] sightNodes;

        private void Start() {
            sightNodes = new SpeedRunnerSightNode[GameConstants.PlayerSight.COLS, GameConstants.PlayerSight.ROWS];
            generateSightNodes();
        }

        private void generateSightNodes() {
            float startX = GameConstants.PlayerSight.START_X_POS; float startY = GameConstants.PlayerSight.START_Y_POS;
            float space = GameConstants.PlayerSight.SPACE;
            traverseSightNodes(
                (x, y) => {
                    Vector3 spawnPos = new Vector3(startX + x * space, startY + y * space, 0);
                    GameObject temp = Instantiate(sightNodePrefab, transform);
                    temp.transform.localPosition = spawnPos;
                    sightNodes[x, y] = temp.GetComponent<SpeedRunnerSightNode>();
                    sightNodes[x, y].init(Camera.main);
                });
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.P))
                printSnapShot(generateSnapshot());
            if (Input.GetKeyDown(KeyCode.L))
                print(sightNodes[0, 0].raycastNode());
        }

        // Update is called once per frame
        void FixedUpdate() {
            Vector3 tempPos = transform.position;
            tempPos.x = player.position.x;
            transform.position = tempPos;
        }


        public string[,] generateSnapshot() {
            string[,] snapshot = new string[GameConstants.PlayerSight.COLS, GameConstants.PlayerSight.ROWS];
            traverseSightNodes((x, y) => { snapshot[x, y] = sightNodes[x, y].raycastNode(); });
            return generateSnapshot();
        }

        private void printSnapShot(string[,] snapshot) {
            for (int y = 0; y < GameConstants.PlayerSight.ROWS; y++) {
                string s = "";
                for (int x = 0; x < GameConstants.PlayerSight.COLS; x++)
                    s += snapshot[x, y] + " ";
                s += "\n";
            }
        }


        private void traverseSightNodes(Action<int, int> a) {
            for (int y = 0; y < GameConstants.PlayerSight.ROWS; y++)
                for (int x = 0; x < GameConstants.PlayerSight.COLS; x++)
                    a(x, y);
        }
    }
}