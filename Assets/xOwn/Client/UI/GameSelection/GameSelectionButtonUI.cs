using AlbotServer;
using Barebones.MasterServer;
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
        [SerializeField]
        private bool isSinglePlayer;
        [SerializeField]
        private GameObject singlePlayerLobby, gameSelectionLobby;


        //Quick hacks for Anne 

        public void onClick() {

            if (isSinglePlayer)
                activateSinglePlayerLobby();
            else
                gameCreator.createNewGame(gameMap, trainingMode.isOn);
        }


        private void activateSinglePlayerLobby() {
            gameSelectionLobby.SetActive(false);
            singlePlayerLobby.SetActive(true);

            PreGameBaseLobby lobby = singlePlayerLobby.GetComponent<PreGameBaseLobby>();
            AccountInfoPacket currentUser = ClientUI.ClientUIOverlord.getCurrentAcountInfo();

            PlayerInfo playerInfo = new PlayerInfo() { iconNumber = int.Parse(currentUser.Properties["icon"]), username = currentUser.Username };
            PreGamePlayer[] players = new PreGamePlayer[] {
                new PreGamePlayer() { info = playerInfo, slotNumber = 0 },
                  new PreGamePlayer() { info = playerInfo, slotNumber = 1, isReady = true}
            };

            AnneHacks.startSinglePlayerLobby(lobby, gameMap);
            lobby.initPreGameLobby(gameMap.Name, gameMap.picture, players, true, 0, gameMap.type);
        }
    }
}