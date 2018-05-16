using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;
using Barebones.Networking;
using System.Linq;
using UnityEngine.Networking;

namespace AlbotServer{

	public class PreGame{
		public List<PreGamePlayer> players = new List<PreGamePlayer>();
		public List<IPeer> connectedPeers = new List<IPeer> ();
		public IPeer admin;
		public Game.GameType type;
		public int roomID, maxPlayers;
		public string hostName;
		public bool isTraining = false;
		private AlbotPreGameModule preGameModule;
		private List<IPeer> spectators = new List<IPeer> ();

		public PreGame(AlbotPreGameModule preGameModule, int amountOfplayers, Game.GameType type, string hostName, int id, bool isTraining, IPeer admin,int maxPlayers = 2){
			this.preGameModule = preGameModule; this.type = type; this.hostName = hostName; this.roomID = id; this.maxPlayers = maxPlayers; this.isTraining = isTraining;
			for (int i = 0; i < amountOfplayers; i++) {
				players.Add (new PreGamePlayer{ info = new PlayerInfo{}, type = PreGameSlotType.Empty, slotNumber = i});
				connectedPeers.Add (null);
			}
			this.admin = admin;
			if (isTraining)
				changePlayer (1, PreGameSlotType.TrainingBot, Game.LocalTrainingBots.StandardTrainingBotInfo, connectedPeers [0], true);
		}
			
		public void peerDissconnected(IPeer peer){
			List<int> leftID = new List<int> ();
			for (int i = 0; i < connectedPeers.Count; i++)
				if (connectedPeers [i] != null && connectedPeers [i].Id == peer.Id) {
					connectedPeers[i] = null;
					leftID.Add (i);
				}

			foreach (int i in leftID)
				removePlayer (i);

			if (connectedPeers.TrueForAll (x => x == null))
				preGameModule.removeGame (this);
			else
				preGameModule.broadcastUpdate (this);
		}


		public void changePlayer(int id, bool newIsReady){ changePlayer (id, players [id].type, players [id].info, connectedPeers[id], newIsReady);}
		private void removePlayer(int id){ changePlayer (id, PreGameSlotType.Empty, players [id].info, null, false);}
		public void changePlayer(int id, PreGameSlotType newType, PlayerInfo newInfo, IPeer peer, bool newIsReady){
			players [id] = new PreGamePlayer {
				slotNumber = id,
				info = newInfo,
				type = newType,
				isReady = newIsReady
			};
			connectedPeers [id] = peer;
		}

		public void updateSlotType(int id, PreGameSlotType type, PlayerInfo info){
			checkIfChangeKicksPlayer (id);
			if (type == PreGameSlotType.TrainingBot)
				info = Game.LocalTrainingBots.StandardTrainingBotInfo;

			bool preReady = (type == PreGameSlotType.TrainingBot || type == PreGameSlotType.AdminHuman);
			Debug.LogError ("New typ " + type + "  " + preReady);
			players [id] = new PreGamePlayer () {type = type, slotNumber = id, info = info, isReady = preReady};
			connectedPeers [id] = type == PreGameSlotType.Empty ? null : connectedPeers [0];  // If the new type is non human we asume it's the admin peer
		}

		private void checkIfChangeKicksPlayer(int id){
			if (connectedPeers [id] == null)
				return;

			int peerId = connectedPeers [id].Id;
			bool foundOtherPlayer = false;
			for (int i = 0; i < connectedPeers.Count; i++)
				if (i != id && connectedPeers [i] != null && connectedPeers [i].Id == peerId)
					foundOtherPlayer = true;

			if(foundOtherPlayer == false)
				preGameModule.handleKickPeerRequest (connectedPeers[id]);
			connectedPeers [id] = null;
		}


		public void setAdminClonesReadyState(){
			for (int i = 1; i < players.Count; i++)
				if (players [i].type == PreGameSlotType.AdminSelfClone)
					changePlayer(i,  players [0].isReady);
		}
			

		public GameInfoPacket convertToGameInfoPacket(){
			return new GameInfoPacket () {
				Type = GameInfoType.PreGame, Name = hostName,
				OnlinePlayers = players.FindAll(x => x.type != PreGameSlotType.Empty).Count,
				MaxPlayers = 2, IsPasswordProtected = false,
				Properties = new Dictionary<string, string>(){
					{MsfDictKeys.MapName, type.ToString()}, 
					{MsfDictKeys.IsPreGame, true.ToString()}, 
					{MsfDictKeys.GameType, type.ToString()}
				},

				Id = roomID, Address = "MasterIP" };
		}
		public int getMatchingPlayerID(IPeer peer){
			for (int i = 0; i < connectedPeers.Count; i++)
				if (connectedPeers [i] != null && connectedPeers [i].Id == peer.Id)
					return i;
			return -1;
		}
		public bool containsPeer(IPeer peer){return connectedPeers.Find (x => (x != null &&  x.Id == peer.Id)) != null;}
		public bool canGameStart(){return players.All (x => x.isReady);}
		public int getAmountCurrentPlayers(){return players.FindAll (x => x.type != PreGameSlotType.Empty).Count;}
		public int getFreeSlotId(){return players.Find (x => x.type == PreGameSlotType.Empty).slotNumber;}
		public List<IPeer> getPeers(){
			List<IPeer> uniqePeers = new List<IPeer> ();
			foreach (IPeer p in connectedPeers)
				if (p != null && uniqePeers.Find (x => x.Id == p.Id) == null)
					uniqePeers.Add (p);

			return uniqePeers;
		}





		#region Spectators
		public void addSpectator(IPeer newP){
			spectators.Add (newP);
		}

		public bool containsSpectator(IPeer newP){
			return spectators.Find(x => x.Id == newP.Id) != null;
		}

		public bool removeSpectator(IPeer p){return removeSpectator (p.Id);}
		public bool removeSpectator(int peerID){
			IPeer oldP = spectators.Find(x => x.Id == peerID);
			if (oldP != null)
				spectators.Remove (oldP);
			return oldP != null;
		}

		public bool hasSpectators(){return spectators.Count > 0;}
		public List<IPeer> getSpectators(){return spectators;}
		#endregion
	}



	public struct PreGamePlayer{
		public PlayerInfo info;
		public PreGameSlotType type;
		public int slotNumber;
		public bool isReady;
	}

	public enum PreGameSlotType{
		Player,
		TrainingBot,
		AdminSelfClone,
		AdminHuman,
		Empty,
	}


}