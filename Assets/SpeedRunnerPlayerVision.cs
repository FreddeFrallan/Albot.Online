using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SpeedRunner {

    public class SpeedRunnerPlayerVision : MonoBehaviour {

        public static SpeedRunnerPlayerVision singleton;

        [SerializeField]
        private Transform player;
        [SerializeField]
        private GameObject sightNodePrefab;

        //Mainly used as optimization when checking coordinates
        private float maxXOffset = GameConstants.PlayerSight.COLS * GameConstants.PlayerSight.SIZE;
        private float maxYOffset = GameConstants.PlayerSight.ROWS * GameConstants.PlayerSight.SIZE;
        private string noCollisionRow;

        private void Start() {
            singleton = this;

            noCollisionRow = "";
            for (int i = 0; i < GameConstants.PlayerSight.COLS; i++)
                noCollisionRow += GameConstants.PlayerSight.EMPTY_SIGN;
        }


        private void Update() {
            if (Input.GetKeyDown(KeyCode.P))
                printSnapShot(generateSnapshot());
        }

        // Update is called once per frame
        void FixedUpdate() {
            Vector3 tempPos = transform.position;
            tempPos.x = player.position.x;
            transform.position = tempPos;
        }


        public string[] generateSnapshot() {
            char[][] snapshot = new char[GameConstants.PlayerSight.ROWS][];
            for (int y = 0; y < GameConstants.PlayerSight.ROWS; y++)
                snapshot[y] = noCollisionRow.ToCharArray();

            addCollisionObjects(ref snapshot);
            string[] temp = new string[GameConstants.PlayerSight.ROWS];
            for (int y = 0; y < GameConstants.PlayerSight.ROWS; y++)
                temp[y] = new string(snapshot[y]);

            return temp;
        }

        private void addCollisionObjects(ref char[][] snapshot) {
            Vector3 basePos = new Vector3(player.transform.position.x, 0, 0) + GameConstants.PlayerSight.START_POS;
            foreach (GroundBlock g in MapGenerator.singleton.getCurrentBlocks())
                addCoordinates(g, ref snapshot, basePos);
        }

        private void addCoordinates(GroundBlock ground, ref char[][] snapshot, Vector3 basePos) {
            Vector2 blockOffset = ground.transform.position - basePos;

            foreach (Vector2 localCoord in ground.getCoordList()) {
                Vector2 coord = blockOffset + localCoord;
                if (coordIsOutOfRange(coord))
                    continue;
                int xGrid = Mathf.RoundToInt(coord.x / GameConstants.PlayerSight.SIZE);
                int yGrid = Mathf.RoundToInt(coord.y / GameConstants.PlayerSight.SIZE);

                if (xGrid >= GameConstants.PlayerSight.COLS || yGrid >= GameConstants.PlayerSight.ROWS)
                    continue;

                snapshot[GameConstants.PlayerSight.ROWS - yGrid -1][xGrid] = GameConstants.PlayerSight.GROUND_SIGN;
            }

        }

        private bool coordIsOutOfRange(Vector2 c) {
            return c.x < 0 || c.x >= maxXOffset || c.y < 0 || c.y >= maxYOffset;
        }


        private void printSnapShot(string[] snapshot) {
            for (int y = 0; y < GameConstants.PlayerSight.ROWS; y++) {
                string s = "";
                for (int x = 0; x < GameConstants.PlayerSight.COLS; x++)
                    s += snapshot[y][x] + "  ";
                print(s);
                s += "\n";
            }
        }


    }
}