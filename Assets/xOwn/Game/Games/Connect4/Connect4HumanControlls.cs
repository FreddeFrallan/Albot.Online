using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Connect4 {

    public class Connect4HumanControlls : MonoBehaviour {

        [SerializeField]
        private List<GameObject> piecePrefab;
        private bool isHumanTurn = false;
        private Action<int> makeMove;

        private GameObject preMovePiece;
        private Vector3 preMovePieceSpawnPos = new Vector3(0, 1, 0);
        private float[] xAxisCap = new float[] { 0, 6 };
        private Vector3 currentMousePos;

        void Update () {
            if (isHumanTurn == false)
                return;

            keyboardControlls();
            mouseControlls();
            movePreMovePiece();
        }

        private void mouseControlls() {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
                currentMousePos = hitInfo.point;

            if (Input.GetMouseButtonDown(0)) {
                float move = getClampedXPos();
                makeMove(Mathf.RoundToInt(move));
            }
        }


        private void keyboardControlls() {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                makeMove(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                makeMove(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                makeMove(2);
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                makeMove(3);
            else if (Input.GetKeyDown(KeyCode.Alpha5))
                makeMove(4);
            else if (Input.GetKeyDown(KeyCode.Alpha6))
                makeMove(5);
            else if (Input.GetKeyDown(KeyCode.Alpha7))
                makeMove(6);
        }

        private void movePreMovePiece() {
            Vector3 temp = preMovePiece.transform.position;
            temp.x = getClampedXPos();
            preMovePiece.transform.position = temp;
        }

        private float getClampedXPos() { return Mathf.Clamp(currentMousePos.x, xAxisCap[0], xAxisCap[1]); }

        public void init(Action<int> makeMove) {this.makeMove = makeMove;}
        public void endPlayerTurn() {
            Destroy(preMovePiece);
            isHumanTurn = false;
        }
        public void startPlayerTurn() {
            preMovePiece = Instantiate(piecePrefab[0], preMovePieceSpawnPos, Quaternion.identity);
            isHumanTurn = true;
        }
    }

}