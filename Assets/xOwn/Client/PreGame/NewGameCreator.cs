using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;
using Barebones.Networking;
using AlbotServer;

namespace ClientUI{
	
	public class NewGameCreator : MonoBehaviour{

        private static NewGameCreator singleton;
		public PreGameLobby preGameLobby;
		public PreTrainingLobby preTrainingLobby;
		private MapSelection currentMap;

        private void Start() {
            singleton = this;
        }

        #region Create new Game
        public void createNewGame(MapSelection selectedMap){
			currentMap = selectedMap;
			AccountInfoPacket currentUser = ClientUIOverlord.getCurrentAcountInfo();
            PreGameSpecs msg = ServerUtils.generateGameSpecs(selectedMap.type, currentUser.Username, true, true);
            Msf.Connection.SendMessage((short)ServerCommProtocl.CreatePreGame, msg, handleCreatedGameResponse);
        }

        private void handleCreatedGameResponse(ResponseStatus status, IIncommingMessage rawMsg) {
            if (Msf.Helper.serverResponseSuccess(status, rawMsg) == false)
                return;
            joineLobbyGame(new GamesListUiItem() { roomType = GameInfoType.PreGame, GameId = rawMsg.AsString() });
        }
        #endregion


        #region Joining
        public static void handleJoinPreGame(GameInfoType roomType, string roomID) { singleton.joinPreGame(roomType, roomID); }
        public void joineLobbyGame(GamesListUiItem game) { joinPreGame(game.roomType, game.GameId); }
        public void joinPreGame(GameInfoType roomType, string roomID){
            PreGameJoinRequest joinRequest = getJoinRequest(roomType, roomID);
            //print("Joining PreGame: " + roomID);

            if(roomType == GameInfoType.PreGame)
			    Msf.Connection.SendMessage((short)ServerCommProtocl.RequestJoinPreGame, joinRequest, handleJoinPreGameMsg);
            else if(roomType == GameInfoType.PreTournament)
                Msf.Connection.SendMessage((short)CustomMasterServerMSG.joinTournament, joinRequest, CurrentTournament.handleJoinedTournament);
        }




		private void handleJoinPreGameMsg(ResponseStatus status, IIncommingMessage rawMsg){
            if (Msf.Helper.serverResponseSuccess(status, rawMsg) == false)
                return;

			PreGameRoomMsg msg = rawMsg.Deserialize<PreGameRoomMsg> ();
            currentMap = GameSelectionUI.getMatchingMapSelection (msg.specs.type);
			AccountInfoPacket playerInfo = ClientUIOverlord.getCurrentAcountInfo ();
			bool isAdmin = playerInfo.Username == msg.players [0].playerInfo.username;
            ClientUIStateManager.requestGotoState(ClientUIStates.PreGame);

			preGameLobby.initPreGameLobby (currentMap.picture, msg);
		}
        #endregion

        private PreGameJoinRequest getJoinRequest(GameInfoType roomType, string roomID) {
            AccountInfoPacket ac = ClientUIOverlord.getCurrentAcountInfo();
            return new PreGameJoinRequest() {
                roomID = roomID,
                joiningPlayer = new PlayerInfo {
                    username = ac.Username,
                    iconNumber = int.Parse(ac.Properties[AlbotDictKeys.icon])
                }
            };
        }

        #region Deubbing
        private void printPlayerSlots(PreGameSlotInfo[] slots) {
            foreach(PreGameSlotInfo slot in slots)
                print(slot.type + " " + slot.playerInfo.username);
            
        }
        #endregion
    }

}