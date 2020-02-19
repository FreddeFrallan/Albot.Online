using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientUI {

    public class ClientDevMoveControlls : MonoBehaviour {


        [SerializeField]
        private NewGameCreator gameCreator;
        [SerializeField]
        private MapSelection speedRunnerGameSelection;


        void Start() {
            Application.targetFrameRate = 60;
        }

        /*
        // Update is called once per frame
        private void Update() {
            if (Input.GetKeyDown(KeyCode.O) && ClientUIOverlord.currentState == ClientUIStates.GameLobby)
                createNewSpeedRunnerGame();
        }
        */

        private void createNewSpeedRunnerGame() {
            print("Sending Create Game");
            gameCreator.createNewGame(speedRunnerGameSelection);
        }
    }
}