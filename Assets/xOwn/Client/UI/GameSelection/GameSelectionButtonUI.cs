using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientUI {
    public class GameSelectionButtonUI : MonoBehaviour{
        [SerializeField]
        private MapSelection gameMap;
        [SerializeField]
        private NewGameCreator gameCreator;

        public void onClick() {
            gameCreator.createNewGame(gameMap, false);
        }
    }
}