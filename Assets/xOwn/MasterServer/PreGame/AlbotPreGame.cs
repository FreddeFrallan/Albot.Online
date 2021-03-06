﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;
using Barebones.Networking;
using System.Linq;
using UnityEngine.Networking;
using System;
using Tournament.Server;
using System.Threading;

namespace AlbotServer{

    public class PreGame {
        public PreGamePlayerSlot[] playerSlots;
        public PreGameSpecs specs { get; private set; }
        public PreGameState state { get; private set; }

        public RunningGameInfoMsg storedInfoMsg { get; private set; }
        private int currentRound = -1;
        public IPeer runningGameConnection { get; private set; }
        private List<Action> onGameStartedFuncs = new List<Action>();

        private object playerLock = new object();
        private AlbotSpectatorModule specModule;
        private IPeer admin, host;
        private List<PreGamePeer> connectedPeers = new List<PreGamePeer>();
        private List<IPeer> spectators = new List<IPeer> ();
        


        #region Init
        public void setExtraHost(IPeer host) { this.host = host; } //Only used in tournament so quick solve a bug when changing slots

        public PreGame(IPeer admin, PreGameSpecs specs, AlbotSpectatorModule specModule){
            this.specs = specs; this.admin = admin; this.specModule = specModule;
            initPlayerslots(specs.maxPlayers);
            storedInfoMsg = new RunningGameInfoMsg() { status = PreGameState.Lobby };
		}

        private void initPlayerslots(int maxPlayers) {
            playerSlots = new PreGamePlayerSlot[maxPlayers];
            for (int i = 0; i < maxPlayers; i++) {
                PreGameSlotInfo temp = new PreGameSlotInfo() { playerInfo = new PlayerInfo { }, type = PreGameSlotType.Empty, slotID = i };
                playerSlots[i].info = temp;
            }
        }
        #endregion

        #region Join & Leave
        public bool peerJoined(IIncommingMessage rawMsg, PlayerInfo info, ref PreGameRoomMsg returnMsg) {
            if (getAmountCurrentPlayers() >= specs.maxPlayers) {
                rawMsg.Respond("Game is already full", ResponseStatus.Failed);
                return false;
            }
            if (state != PreGameState.Lobby) {
                rawMsg.Respond("Game has already started", ResponseStatus.Failed);
                return false;
            }

            addPeerToGame(rawMsg.Peer, info);
            returnMsg = newUpdate();
            return true;
        }

        private void addPeerToGame(IPeer peer, PlayerInfo info) {
            //Debug.LogError("Adding peer: " + info.username + "  to game: " + specs.roomID);
            PreGamePeer newPeer = new PreGamePeer(peer, info, peerLeft);
            lock (playerLock) connectedPeers.Add(newPeer);

            int freeSlot = getFreeSlotId();
            newPeer.addPlayerSlot(freeSlot);
            changePlayerSlot(freeSlot, PreGameSlotType.Player, info, newPeer, false);
            broadcastUpdate(peer.Id);
        }

        public void peerLeft(IPeer p) { peerLeft(getMatchingPeer(p.Id)); }
        public void peerLeft(PreGamePeer p) {
            p.playerSlots.ForEach(i => setplayerSlotEmpty(i));
            p.clearReferences();
            lock (playerLock) connectedPeers.Remove(p);

            lock (playerLock) {
                if (connectedPeers.Count == 0) //Check if Game should be terminated
                    AlbotPreGameModule.removeGame(this, specs.roomID);
                else
                    broadcastUpdate();
            }
        }
        #endregion

        #region Slots
        private void setplayerSlotEmpty(int slotID){
            playerSlots[slotID].info.type = PreGameSlotType.Empty;
            playerSlots[slotID].info.isReady = false;
        }
        private void changePlayerSlot(int slotID, PreGameSlotType newType, PlayerInfo newInfo, PreGamePeer newPeer, bool newIsReady){
            removeOldPeerFromSlot(slotID, newPeer);

            playerSlots[slotID].peer = newPeer;
            playerSlots[slotID].info.belongsToPlayer = newPeer == null ? "" : newPeer.info.username;

            playerSlots[slotID].info.slotID = slotID;
            playerSlots[slotID].info.playerInfo = newInfo;
            playerSlots[slotID].info.type = newType;
            playerSlots[slotID].info.isReady = newIsReady;
        }

        private void removeOldPeerFromSlot(int slotID, PreGamePeer newPeer) {
            PreGamePlayerSlot slot = playerSlots[slotID];
            if (slot.peer != null && newPeer != slot.peer)
                slot.peer.removePlayerSlot(slotID);
        }

        public void updateSlotType(PreGameSlotInfo slot,  IPeer peer){
            PreGamePlayerSlot oldSlot = playerSlots[slot.slotID];
            if (isAdminOrHost(peer) == false && oldSlot.peer != null && peer.Id != oldSlot.peer.peer.Id) //Only allow admin or the same peer to change slot
                return;

            PreGamePeer localPeer = getMatchingPeer(peer.Id);
            if (localPeer == null) {//Quick fix to wait until player joins game
                MainThread.createTimedAction(() => { updateSlotType(slot, peer); }, 0.1f);
                return;
            }

            if (slot.type == PreGameSlotType.Player) {
                localPeer.removePlayerSlot(slot.slotID);
                setplayerSlotEmpty(slot.slotID);
            } else {
                localPeer.addPlayerSlot(slot.slotID);
                changePlayerSlot(slot.slotID, slot.type, slot.playerInfo, getMatchingPeer(peer.Id), getIsPreReady(slot.type, localPeer));
            }

            broadcastUpdate();
        }

        

        public void updatePeerReady(IPeer peer, bool ready) {
            PreGamePeer localPeer = getMatchingPeer(peer.Id);
            localPeer.isReady = ready;
            foreach (int slotID in localPeer.playerSlots)
                if(playerSlots[slotID].info.type == PreGameSlotType.Player || playerSlots[slotID].info.type == PreGameSlotType.SelfClone) 
                    playerSlots[slotID].info.isReady = ready;

            broadcastUpdate();
        }

        private bool getIsPreReady(PreGameSlotType type, PreGamePeer peer) {
            if ((type == PreGameSlotType.TrainingBot || type == PreGameSlotType.Human))
                return true;
            if(type == PreGameSlotType.SelfClone)
                return peer.isReady;
            return false;
        }
        #endregion



        #region Getters
        private PreGamePeer getMatchingPeer(int id) { //Because of possible threading this needs to be try-catched for "InvalidOperationException"
            lock (playerLock) {
                List<PreGamePeer> temp = connectedPeers.ToList();
                return temp.Find(p => p.peer.Id == id);
            }
        }
        private PreGameRoomMsg newUpdate() {return new PreGameRoomMsg() {specs = specs, players = getPlayerSlots()};}
        private int getFreeSlotId(){return playerSlots.First (x => x.info.type == PreGameSlotType.Empty).info.slotID;}
        private int getAmountCurrentPlayers() { return playerSlots.Where(x => x.info.type != PreGameSlotType.Empty).Count(); }

        public bool isPlayerReady(int id) { return getMatchingPeer(id).isReady; }
		public bool canGameStart(){return playerSlots.All (x => x.info.isReady);}
        public PreGameSlotInfo[] getPlayerSlots() { return playerSlots.Select(p => p.info).ToArray(); }
        public PlayerInfo[] getPlayerInfo() { return playerSlots.Select(p => p.info.playerInfo).ToArray();}
		public bool containsPeer(IPeer peer){ lock (playerLock) return connectedPeers.Any(p => p.peer.Id == peer.Id); }
        public bool isAdmin(IPeer peer) { return admin.Id == peer.Id; }
        public bool isHost(IPeer peer) { return host != null && host.Id == peer.Id; }
        public bool isAdminOrHost(IPeer peer) { return isAdmin(peer) || isHost(peer); }
        public List<IPeer> getPeers(){ lock (playerLock) return connectedPeers.Select(p => p.peer).ToList();}
        public Dictionary<string, string> generateGameSettings() {return Msf.Helper.generateGameSettings(specs, hasSpectators(), getPlayerSlots());}
        public GameInfoPacket convertToGameInfoPacket() {
            return Msf.Helper.createGameInfoPacket(GameInfoType.PreGame, specs.roomID, specs.hostName, 2, getAmountCurrentPlayers(), specs.type);
        }
        #endregion


        private void broadcastUpdate(int skipPeerID = -1) {
            if (state != PreGameState.Lobby)
                return;
            PreGameRoomMsg msg = newUpdate();
            foreach (IPeer p in connectedPeers.Select(p => p.peer))
                if (p.Id != skipPeerID)
                    p.SendMessage((short)ServerCommProtocl.UpdatePreGame, msg);

            if (specs.isInTournament)
                AlbotRunningTournamentModule.handleUpdateRound(specs.tournamentID, specs.tournamentRoundID);
        }

        #region Start Game
        private void resetPlayerReady() {lock(playerLock)connectedPeers.ForEach(p => updatePeerReady(p.peer, false));}
        public void onpreGameStarted() { updateState(PreGameState.Starting); }

        public void onGameStarted(RunningGameInfoMsg infoMsg, IIncommingMessage rawMsg) {
            updateState(PreGameState.Running);
            currentRound++;
            addIconNumbersToInfoMsg(infoMsg);
            specModule.preGameStarted(this, storedInfoMsg);
            runningGameConnection = rawMsg.Peer;
            resetPlayerReady();
            runOnGameStartedFuncs();
        }
        private void runOnGameStartedFuncs() {
            onGameStartedFuncs.ForEach(f => f.Invoke());
            onGameStartedFuncs.Clear();
        }

        private void addIconNumbersToInfoMsg(RunningGameInfoMsg infoMsg) {
            for(int i = 0; i < infoMsg.players.Length; i++)
                infoMsg.players[i].iconNumber = playerSlots.First(p => p.info.playerInfo.username == infoMsg.players[i].username).info.playerInfo.iconNumber;
            storedInfoMsg = infoMsg;
            storedInfoMsg.status = state;
        }
        #endregion

        #region remove Game
        public void onRemoved() {
            connectedPeers.Clear();
            spectators.Clear();
            specModule.preGameRemoved(this);
        }

        public void getGameOverMsg(GameOverMsg info) {
            if (specs.isInTournament == false)
                return;
            AlbotRunningTournamentModule.handleGameResult(specs.tournamentID, specs.tournamentRoundID, info);
        }
        #endregion

        private void updateState(PreGameState newState) {
            state = newState;
            if(storedInfoMsg != null)
                storedInfoMsg.status = state;
        }

        #region Spectators
        public void addSpectator(IPeer newP){
            if(hasSpectators() == false) {
                if (state == PreGameState.Running)
                    sendSpectateBroadcastStatus(true);
                else if (state == PreGameState.Starting)
                    onGameStartedFuncs.Add(() => { sendSpectateBroadcastStatus(true); });
            }

            spectators.Add (newP);
        }
		public bool containsSpectator(IPeer newP){return spectators.Find(x => x.Id == newP.Id) != null;}
		public bool removeSpectator(IPeer p){return removeSpectator (p.Id);}
		public bool removeSpectator(int peerID){
			IPeer oldP = spectators.Find(x => x.Id == peerID);
			if (oldP != null)
				spectators.Remove (oldP);

            if(hasSpectators() == false) {
                if (state == PreGameState.Running)
                    sendSpectateBroadcastStatus(false);
                else if (state == PreGameState.Starting)
                    onGameStartedFuncs.Add(() => { sendSpectateBroadcastStatus(false); });
            }
			return oldP != null;
		}

		public bool hasSpectators(){return spectators.Count > 0;}
		public List<IPeer> getSpectatorsClone(){return spectators.ToList();}
        public void sendSpectateBroadcastStatus(bool status) { runningGameConnection.SendMessage((short)CustomMasterServerMSG.RunningGameInfo, status.ToString());}
        #endregion
    }



    public class PreGameSpecs : MessageBase { 
        public Game.GameType type;
        public string roomID, spawnCode;
        public int maxPlayers;
        public string hostName;
        public bool canRestart = true, showInLobby = true;

        //Tournament
        public bool isInTournament;
        public string tournamentID;
        public RoundID tournamentRoundID;
    }

    public struct PreGameSlotInfo {
		public PlayerInfo playerInfo;
		public PreGameSlotType type;
		public int slotID;
		public bool isReady;
        public string belongsToPlayer;
    }

	public struct PreGamePlayerSlot{
        public PreGamePeer peer;
        public PreGameSlotInfo info;
    }


    public enum PreGameState{
        Lobby,
        Starting,
        Running,
        GameOver,
    }
}