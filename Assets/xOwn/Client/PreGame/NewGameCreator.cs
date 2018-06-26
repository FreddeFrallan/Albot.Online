using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;
using Barebones.Networking;
using AlbotServer;

namespace ClientUI{
	
	public class NewGameCreator : MonoBehaviour{
		
		public PreGameLobby preGameLobby;
		public PreTrainingLobby preTrainingLobby;
		private MapSelection currentMap;

        #region Create new Game
        public void createNewGame(MapSelection selectedMap){
			currentMap = selectedMap;
			AccountInfoPacket currentUser = ClientUIOverlord.getCurrentAcountInfo();
            PreGameSpecs msg = Msf.Helper.createGameSpecs(selectedMap.type, selectedMap.maxPlayers, currentUser.Username);
            Msf.Connection.SendMessage((short)ServerCommProtocl.CreatePreGame, msg, handleCreatedGameResponse);
        }

        private void handleCreatedGameResponse(ResponseStatus status, IIncommingMessage rawMsg) {
            if (responseSuccess(status, rawMsg) == false)
                return;
            joinPreGame(rawMsg.AsString());
        }
        #endregion


        #region Joining
        public void joinPreGame(string gameId){
			AccountInfoPacket ac = ClientUIOverlord.getCurrentAcountInfo();
			PreGameJoinRequest msg = new PreGameJoinRequest () {
				roomID = gameId,
				joiningPlayer = new PlayerInfo {
					username = ac.Username,
					iconNumber = int.Parse(ac.Properties["icon"])
				}
			};
			Msf.Connection.SendMessage((short)ServerCommProtocl.RequestJoinPreGame, msg, handleJoinPreGameMsg);
		}

		private void handleJoinPreGameMsg(ResponseStatus status, IIncommingMessage rawMsg){
            if (responseSuccess(status, rawMsg) == false)
                return;

			PreGameRoomMsg msg = rawMsg.Deserialize<PreGameRoomMsg> ();
            print("Room id: " + msg.specs.roomID + "  " + msg.specs.type);
            printPlayerSlots(msg.players);


            currentMap = GameSelectionUI.getMatchingMapSelection (msg.specs.type);
			AccountInfoPacket playerInfo = ClientUIOverlord.getCurrentAcountInfo ();
			bool isAdmin = playerInfo.Username == msg.players [0].playerInfo.username;
			ClientUIStateManager.requesGotoPreGame ();

			preGameLobby.initPreGameLobby (currentMap.picture, msg);
		}
        #endregion



        #region Deubbing
        private bool responseSuccess(ResponseStatus status, IIncommingMessage rawMsg) {
            if (status != ResponseStatus.Success) {
                Msf.Events.Fire(Msf.EventNames.ShowDialogBox, DialogBoxData.CreateError(rawMsg.AsString()));
                Debug.LogError(status + " - " + rawMsg.AsString());
                return false;
            }
            return true;
        }

        private void printPlayerSlots(PreGameSlotInfo[] slots) {
            foreach(PreGameSlotInfo slot in slots)
                print(slot.type + " " + slot.playerInfo.username);
            
        }
        #endregion
    }

}