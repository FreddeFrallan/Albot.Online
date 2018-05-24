using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;
using Barebones.Networking;
using System.Linq;
using UnityEngine.Networking;

namespace AlbotServer{

	public class AlbotPreGameModule : ServerModuleBehaviour {
		public static AlbotPreGameModule singleton;
		public AlbotSpectatorModule spectatorModule;

		public List<PreGame> currentPreGames = new List<PreGame>();
		public List<PreGame> currentTrainingGames = new List<PreGame> ();
		private int idCounter = 0;


		public override void Initialize (IServer server){
			singleton = this;
			base.Initialize (server);
			server.SetHandler ((short)ServerCommProtocl.CreatePreGame, handleCreatePreGame);
			server.SetHandler ((short)ServerCommProtocl.RequestJoinPreGame, handleRequestJoinPreGame);
			server.SetHandler ((short)ServerCommProtocl.UpdatePreGame, handlePlayerReadyUpdate);
			server.SetHandler ((short)ServerCommProtocl.StartPreGame, handleStartPreGame);
			server.SetHandler ((short)ServerCommProtocl.RestartTrainingGame, handleRestartTrainingGame);
			server.SetHandler ((short)ServerCommProtocl.PlayerLeftPreGame, handlePeerLeft);
			server.SetHandler ((short)ServerCommProtocl.SlotTypeChanged, handleSlotTypeChanged);
		}

        
		private void handleCreatePreGame(IIncommingMessage message){
			PreGameCreateMSg msg = message.Deserialize<PreGameCreateMSg> ();
            Debug.LogError("Creating pre game: " + msg.type);

            PreGame newGame = new PreGame (this, 2, msg.type, msg.mainPlayer.username, idCounter++, msg.isTraining, message.Peer);
			newGame.changePlayer (0, PreGameSlotType.Player, msg.mainPlayer, message.Peer, false);
			currentPreGames.Add (newGame);

			message.Peer.Disconnected += handlePlayerDissconnect;
			message.Peer.SendMessage((short)AlbotServer.ServerCommProtocl.RequestJoinPreGame, new PreGameRoomMsg(){players = newGame.players.ToArray(), type = newGame.type, roomID = newGame.roomID, isTraining = msg.isTraining,});
		}
        

		private void handleRequestJoinPreGame(IIncommingMessage message){
			PreGameJoinRequest msg = message.Deserialize<PreGameJoinRequest> ();
			PreGame targetGame = currentPreGames.Find (x => x.roomID == msg.roomID);

			if (targetGame == null) {
				message.Peer.SendMessage ((short)AlbotServer.ServerCommProtocl.RequestJoinPreGame, new PreGameRoomMsg (){ errorMsg = "Selected game was not found!" });
				return;
			} else if (targetGame.getAmountCurrentPlayers () == targetGame.maxPlayers) {
				message.Peer.SendMessage((short)AlbotServer.ServerCommProtocl.RequestJoinPreGame, new PreGameRoomMsg(){errorMsg = "Game is already full"});
				return;
			}

			//Is allowed to join
			int freeId = targetGame.getFreeSlotId();
			targetGame.changePlayer(freeId ,PreGameSlotType.Player,  msg.joiningPlayer, message.Peer, false);
			message.Peer.SendMessage((short)AlbotServer.ServerCommProtocl.RequestJoinPreGame, new PreGameRoomMsg(){players = targetGame.players.ToArray(), type = targetGame.type, roomID = targetGame.roomID});
			message.Peer.Disconnected += handlePlayerDissconnect;
			broadcastUpdate (targetGame, freeId);
		}

		private void handlePlayerReadyUpdate(IIncommingMessage message){
			PreGameReadyUpdate msg = message.Deserialize<PreGameReadyUpdate> ();
			PreGame targetGame = currentPreGames.Find (x => x.roomID == msg.roomID);
			if (targetGame == null)
				return;

			int playerID = targetGame.getMatchingPlayerID (message.Peer);
			if (playerID < 0)
				return;
			
			targetGame.changePlayer (playerID, msg.isReady);
			broadcastUpdate (targetGame, playerID);
		}

		private void handleStartPreGame(IIncommingMessage message){
			PreGameStartMsg msg = message.Deserialize<PreGameStartMsg> ();
				
			//Quick Hack
			if (msg.isSinglePlayer) {
				handleSinglePlayerGame ();
				return;
			}

			PreGame targetGame = currentPreGames.Find (x => x.roomID == msg.roomID);
			if (targetGame == null)
				return;
			else if (targetGame.canGameStart () == false) {
				message.Peer.SendMessage ((short)ServerCommProtocl.StartPreGame, new PreGameStartMsg (){ errorMsg = "Not all players are ready" });
				return;
			} else if (targetGame.getMatchingPlayerID (message.Peer) != 0) {
				message.Peer.SendMessage ((short)ServerCommProtocl.StartPreGame, new PreGameStartMsg (){ errorMsg = "Only the Admin can start the game" });
				return;
			}	
				
			if (targetGame.isTraining) {
				Debug.LogError ("Saving: " + targetGame.roomID);
				currentTrainingGames.Add (targetGame);
			}
			removeGame (targetGame);
			startGame (targetGame);
		}

		private void startGame(PreGame targetGame){
			GameSettings constants = InduvidualGameData.games [targetGame.type];
			Dictionary<string, string> settings = new Dictionary<string, string>{
				{MsfDictKeys.MaxPlayers, constants.maxPlayers.ToString()},
				{MsfDictKeys.RoomName, targetGame.hostName},
				{MsfDictKeys.MapName, constants.mapName},
				{MsfDictKeys.SceneName, constants.sceneName},
				{MsfDictKeys.IsRealtime, constants.isRealTime.ToString()},
				{MsfDictKeys.GameType, targetGame.type.ToString()},
				{MsfDictKeys.Spectators, targetGame.hasSpectators().ToString()}
			};

			Debug.LogError (targetGame.type + " with players: " + targetGame.players.Count);
			for (int i = 0; i < targetGame.players.Count; i++) {
				settings.Add ("p" + i, targetGame.players [i].info.username);
				Debug.LogError ("p" + i + targetGame.players [i].info.username);
			}

			//Create new game room
			int newGameRoomId = SpawnersModule.singleton.createNewRoomFromPreGame(targetGame.getPeers(), settings);
			if (newGameRoomId == -1) { // We encountered some kind of error when spawning a new gameRoom
				Debug.LogError("Error spawning new game");
				return;
			}

			//Allerting pending spectators
			spectatorModule.preGameStarted(targetGame, newGameRoomId);

			foreach(IPeer p in targetGame.getPeers())
				p.SendMessage((short)ServerCommProtocl.StartPreGame, new PreGameStartMsg(){roomID = newGameRoomId, trainingRoomID = targetGame.roomID, isTraining = targetGame.isTraining});
			GamesData.totallGamesPlayed++;
		}


		private void handleRestartTrainingGame(IIncommingMessage message){
			PreGameStartMsg msg = message.Deserialize<PreGameStartMsg> ();
			Debug.LogError ("Search ID: " + msg.roomID);
			PreGame targetGame = currentTrainingGames.Find (x => x.roomID == msg.roomID);
			foreach (PreGame p in currentTrainingGames)
				Debug.LogError ("Game ID: " + p.roomID);


			if (targetGame != null  /* && targetGame.admin.Id != message.Peer.Id */)
				startGame (targetGame);
			else {
				print ("Could not find matching training game");
			}
		}

			
		private void handleSlotTypeChanged(IIncommingMessage message){
			PreGameSlotSTypeMsg msg = message.Deserialize<PreGameSlotSTypeMsg> ();
			PreGame targetGame = currentPreGames.Find (x => x.roomID == msg.roomID);
			if (targetGame == null)
				return;

			Debug.LogError ("Change: " + msg.slotID + "    " + msg.type);
			targetGame.updateSlotType (msg.slotID, msg.type, msg.newPlayerInfo);
			broadcastUpdate (targetGame);
		}

		public void handleKickPeerRequest(IPeer peer){
			if (peer != null) {
				peer.SendMessage ((short)ServerCommProtocl.PreGameKick, new PreGameKickMsg());
				removePeerDissconnectEvent (peer);
			}
		}


		private void handleSinglePlayerGame(){
			GamesData.totallGamesPlayed++;
		}


		public void removeGame(PreGame targetGame){currentPreGames.Remove (targetGame);}
		public void removePeerDissconnectEvent(IPeer p){p.Disconnected -= handlePlayerDissconnect;}
		private void handlePeerLeft(IIncommingMessage message){handlePlayerDissconnect (message.Peer);}
		private void handlePlayerDissconnect(IPeer peer){
			PreGame targetGame = currentPreGames.Find (x => x.containsPeer (peer));
			peer.Disconnected -= handlePlayerDissconnect;

			if (targetGame != null) 
				targetGame.peerDissconnected (peer);
		}
			
		public void broadcastUpdate(PreGame targetGame, int skipID = -1){
			targetGame.setAdminClonesReadyState ();
			PreGameRoomMsg msg = new PreGameRoomMsg () {players = targetGame.players.ToArray(),};
			for (int i = 0; i < targetGame.connectedPeers.Count; i++) {
				if (skipID == i || targetGame.connectedPeers [i] == null)
					continue;

				targetGame.connectedPeers [i].SendMessage ((short)ServerCommProtocl.UpdatePreGame, msg);
			}
		}


		public List<PreGame> getAllPreGames(){
			List<PreGame> totalGames = new List<PreGame> ();
			totalGames.AddRange (currentPreGames);
			totalGames.AddRange (currentTrainingGames);
			return totalGames;
		}
	}
}