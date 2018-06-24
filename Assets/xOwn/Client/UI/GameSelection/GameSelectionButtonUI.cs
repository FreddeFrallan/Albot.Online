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
            PreGameSlotInfo[] players = new PreGameSlotInfo[] {
                new PreGameSlotInfo() { playerInfo = playerInfo, slotID = 0 },
                  new PreGameSlotInfo() { playerInfo = playerInfo, slotID = 1, isReady = true}
            };
            
            PreGameSpecs specs = new PreGameSpecs() { type = gameMap.type, hostName = ClientUIOverlord.getCurrentAcountInfo().Username };
            PreGameRoomMsg roomInfo = new PreGameRoomMsg() { players = players, specs = specs};

            AnneHacks.startSinglePlayerLobby(lobby, gameMap);
            lobby.initPreGameLobby(gameMap.picture, roomInfo);
        }
    }
}