using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.Networking;
using UnityEngine.Networking;
using AlbotDB;
using System;
using System.Linq;
using AlbotServer;

namespace Barebones.MasterServer{

	public class AlbotSpectatorModule : ServerModuleBehaviour {

		public MatchmakerModule matchMakerModule;
		public AlbotPreGameModule preGameModule;
		public RoomsModule roomsModule; 

		private Dictionary<int, RegisteredRoom> currentRooms;
		private Dictionary<int, List<IPeer>> currentSpectators = new Dictionary<int, List<IPeer>> ();
		private Dictionary<int, int> adminToRooms = new Dictionary<int, int> ();


		public override void Initialize (IServer server){
			server.SetHandler ((short)CustomMasterServerMSG.startSpectate, handleStartSpectateGame);
			server.SetHandler ((short)CustomMasterServerMSG.stopSpectate, handleStopSpectateGame);
			server.SetHandler ((short)CustomMasterServerMSG.requestSpectatorGames, handleRequestSpectatorGames);
			server.SetHandler ((short)CustomMasterServerMSG.spectateLogUpdate, handleGamelogUpdate);
			currentRooms = roomsModule.getCurrentRooms ();
			Debug.LogError ("Spectator init");
		}


		//Update sent from the game, here the master server broadcasts to all spectators.
		private void handleGamelogUpdate(IIncommingMessage msg){
			SpectatorGameLog logMsg = msg.Deserialize<SpectatorGameLog> ();
			if (currentSpectators.ContainsKey (logMsg.id) == false) {
				msg.Respond (ResponseStatus.Failed);
				return;
			}

			foreach (IPeer p in currentSpectators[logMsg.id]) {
				try{ p.SendMessage((short)(CustomMasterServerMSG.spectateLogUpdate), logMsg);}
				catch{
					removeSpectator (logMsg.id, p);
				}
			}
		}


		public void preGameStarted(PreGame game, int roomID){
			if (game.hasSpectators () == false)
				return;

			SpectatorInfoMsg msg = new SpectatorInfoMsg (){
				gameType = game.type,
				status = SpectatorGameStatus.Running
				};
			foreach (IPeer p in game.getSpectators()) {
				subscribeUserRunningGame (p, roomID);
				p.SendMessage ((short)CustomMasterServerMSG.spectateStatus, msg);
			}
		}



		#region Start spectate game
		private void handleStartSpectateGame(IIncommingMessage msg){
			if (SpectatorAuthModule.existsAdmin (msg.Peer) == false)
				return;

			//If the admin is watching another game, we stop that subscription
			if (adminToRooms.ContainsKey (msg.Peer.Id)) {
				Debug.LogError ("User is already registred");
				removeSpectator (adminToRooms [msg.Peer.Id], msg.Peer.Id);
			}


			SpectatorSubscriptionsMsg subscribeMsg = msg.Deserialize<SpectatorSubscriptionsMsg> ();
			if (subscribeMsg.preGame)
				subscribePreGame (subscribeMsg, msg);
			else
				subscribeRunningGame (subscribeMsg, msg);

		}

		private void subscribePreGame(SpectatorSubscriptionsMsg msg, IIncommingMessage originalMsg){
			List<PreGame> preGames = preGameModule.getAllPreGames ();
			PreGame game = preGames.Find (x => x.roomID == msg.broadcastID);
	
			if (game == null) {
				originalMsg.Respond ("RoomID can't be found", ResponseStatus.Error);
				return;
			}

			if (game.containsSpectator (originalMsg.Peer))
				originalMsg.Respond ("Already subscribed to game", ResponseStatus.Error);
			else {
				game.addSpectator (originalMsg.Peer);
				adminToRooms.Add (originalMsg.Peer.Id, msg.broadcastID);
				originalMsg.Respond ("Subscribed to preGame!", ResponseStatus.Success);
			}
		}


		private void subscribeRunningGame(SpectatorSubscriptionsMsg msg, IIncommingMessage originalMsg){
			if (currentRooms.ContainsKey (msg.broadcastID) == false) {
				originalMsg.Respond ("RoomID can't be found", ResponseStatus.Error);
				return;
			}
				
			subscribeUserRunningGame (originalMsg.Peer, msg.broadcastID);
			adminToRooms.Add (originalMsg.Peer.Id, msg.broadcastID);
			msg.active = true;

			currentRooms[msg.broadcastID].Peer.SendMessage((short)CustomMasterServerMSG.requestFullGameLog, msg, ((status, response) => {
				if(status != ResponseStatus.Success || response == null){
					originalMsg.Respond(status);
					return;
				}
				Debug.LogError("Success subscribing to running game");
				SpectatorGameLog logMsg = response.Deserialize<SpectatorGameLog> ();
				originalMsg.Respond(logMsg, status);
			}));
		}
			
		private void subscribeUserRunningGame(IPeer p, int broadcastID){
			if (currentSpectators.ContainsKey (broadcastID) == false)
				currentSpectators.Add (broadcastID, new List<IPeer> ());

			List<IPeer> spectators = currentSpectators [broadcastID];
			if (spectators.Any (x => x.Id == p.Id) == false)
				spectators.Add (p);
		}
		#endregion

	
		private void handleStopSpectateGame(IIncommingMessage msg){
			Debug.LogError ("Player wishes to stop spectating");
			if (SpectatorAuthModule.existsAdmin (msg.Peer) == false)
				return;
			if (adminToRooms.ContainsKey (msg.Peer.Id) == false)
				return;

			removeSpectator (adminToRooms[msg.Peer.Id], msg.Peer);
		}


		private void removeSpectator(int broadcastID, int peerID){
			if(adminToRooms.ContainsKey(peerID))
				adminToRooms.Remove (peerID);

			if (currentSpectators.ContainsKey(broadcastID)){ //running game
				List<IPeer> specRoom = currentSpectators [broadcastID];
				IPeer p;

				try{p = specRoom.Find(x => x.Peer.Id == peerID);} //Room is acting weird, remove it   HOTFIX
				catch{
					currentSpectators.Remove(broadcastID);
					return;
				}

				if(p != null)
					removeSpectator (broadcastID, p);
			}
			else { //Pregame
				List<PreGame> preGames = preGameModule.getAllPreGames ();
				PreGame game = preGames.Find (x => x.roomID == broadcastID);
				if (game != null)
					game.removeSpectator (peerID);
			}

		}
		private void removeSpectator(int broadcastID, IPeer peer){
			if(adminToRooms.ContainsKey(peer.Id))
				adminToRooms.Remove (peer.Id);

			if (currentSpectators.ContainsKey (broadcastID) == false)
				return;
			if (currentRooms.ContainsKey (broadcastID) == false)
				return;

			List<IPeer> spectators = currentSpectators[broadcastID];
			IPeer p = spectators.Find (x => x.Id == peer.Id);
			if (p != null)
				spectators.Remove (p);

			
			if (spectators.Count == 0) {
				SpectatorSubscriptionsMsg subMsg = new SpectatorSubscriptionsMsg (){ active = false, broadcastID = broadcastID };
				currentRooms [broadcastID].Peer.SendMessage ((short)CustomMasterServerMSG.spectateStatus, subMsg);

				currentSpectators.Remove (broadcastID);
			}
		}


		private void handleRequestSpectatorGames(IIncommingMessage msg){
			if (SpectatorAuthModule.existsAdmin (msg.Peer) == false)
				return;

			try{
				List<GameInfoPacket> games = matchMakerModule.getCurrentSpectatorGames(msg.Peer);
				byte[] bytes = games.Select(l => (ISerializablePacket)l).ToBytes();
				msg.Respond(bytes, ResponseStatus.Success);
			}catch{msg.Respond (ResponseStatus.Error);}
		}
			

	}


	public enum SpectatorGameStatus{
		Running,
		PreGameLobby,
		GameOver,
		Crashed,
	}


	public class SpectatorSubscriptionsMsg : MessageBase{
		public bool active, preGame;
		public int broadcastID = -1;
	}

	public class SpectatorGameLog : MessageBase{
		public string[] gameLog;
		public int id, updateNumber;
		public bool initLog;
	}
	public class SpectatorInfoMsg : MessageBase{
		public Game.GameType gameType;
		public SpectatorGameStatus status;
	}


	public class SpectatorGameList : MessageBase{
		public SpectatorGameInfo[] currentGames;
	}
	public struct SpectatorGameInfo{
		public int gameId;
		public string[] players;
		public string gameType, mapName;
	}
}
