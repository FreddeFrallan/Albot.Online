using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;
using Barebones.Networking;

namespace ClientUI{
	
	public class NewGameCreator : MonoBehaviour{
		
		public PreGameLobby preGameLobby;
		public PreTrainingLobby preTrainingLobby;
		private MapSelection currentMap;

		//Enter preGameLobby
		public void createNewGame(MapSelection selectedMap, bool isTraining){
			currentMap = selectedMap;
            print(selectedMap.picture);
            print(currentMap);

			AccountInfoPacket currentUser = ClientUI.ClientUIOverlord.getCurrentAcountInfo();
			AlbotServer.PreGameCreateMSg msg = new AlbotServer.PreGameCreateMSg(){type = selectedMap.type, 
				isTraining = isTraining,
				mainPlayer = new AlbotServer.PlayerInfo{
					username = currentUser.Username,
					iconNumber = int.Parse(currentUser.Properties["icon"]),
				}};
			Msf.Connection.SendMessage((short)AlbotServer.ServerCommProtocl.CreatePreGame, msg);
		}

		public void joinPreGame(int gameId){
			AccountInfoPacket ac = ClientUIOverlord.getCurrentAcountInfo();
			AlbotServer.PreGameJoinRequest msg = new AlbotServer.PreGameJoinRequest () {
				roomID = gameId,
				joiningPlayer = new AlbotServer.PlayerInfo {
					username = ac.Username,
					iconNumber = int.Parse(ac.Properties["icon"])
				}
			};
			Msf.Connection.SendMessage((short)AlbotServer.ServerCommProtocl.RequestJoinPreGame, msg);
		}


		public void handleJoinPreGameMsg(IIncommingMessage message){
			AlbotServer.PreGameRoomMsg msg = message.Deserialize<AlbotServer.PreGameRoomMsg> ();

			if (msg.errorMsg != "") {
				Msf.Events.Fire(Msf.EventNames.ShowDialogBox, DialogBoxData.CreateError(msg.errorMsg));
				Debug.LogError (msg.errorMsg);
				return;
			}
			currentMap = GameSelectionUI.getMatchingMapSelection (msg.type);
			AccountInfoPacket playerInfo = ClientUI.ClientUIOverlord.getCurrentAcountInfo ();
			bool isAdmin = playerInfo.Username == msg.players [0].info.username;

			ClientUIStateManager.requesGotoPreGame ();

            print(msg.type.ToString());

            if (msg.isTraining)
				preTrainingLobby.initPreGameLobby (msg.type.ToString(), currentMap.picture, msg.players, true, msg.roomID, msg.type);
			else
				preGameLobby.initPreGameLobby (msg.type.ToString(), currentMap.picture, msg.players, isAdmin, msg.roomID, msg.type);
		}

	}

}