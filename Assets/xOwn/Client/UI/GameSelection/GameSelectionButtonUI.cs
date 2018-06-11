using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClientUI {
    public class GameSelectionButtonUI : MonoBehaviour{
        [SerializeField]
        private MapSelection gameMap;
        [SerializeField]
        private NewGameCreator gameCreator;
        [SerializeField]
        private Toggle trainingMode;

        public void onClick() {
            gameCreator.createNewGame(gameMap, trainingMode.isOn);
        }
    }
}