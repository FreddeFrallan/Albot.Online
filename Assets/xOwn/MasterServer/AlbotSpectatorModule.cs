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
		private Dictionary<int, PreGame> adminToRooms = new Dictionary<int, PreGame> ();
        private Dictionary<string, PreGame> activeGames;
        private Dictionary<string, PreGame> allGames;
        private List<PreGame> allGamesList;


        public override void Initialize (IServer server){
			server.SetHandler ((short)CustomMasterServerMSG.startSpectate, handleStartSpectateGame);
			server.SetHandler ((short)CustomMasterServerMSG.stopSpectate, handleStopSpectateGame);
			server.SetHandler ((short)CustomMasterServerMSG.spectateLogUpdate, handleGamelogUpdate);
			server.SetHandler ((short)CustomMasterServerMSG.requestSpectatorGames, handleRequestSpectatorGames);
            server.SetHandler((short)CustomMasterServerMSG.requestSpecificGameLog, handleRequestSpecificUpdates);
            Debug.LogError ("Spectator init");
		}

        public void initPreGames(Dictionary<string, PreGame> activeGames, Dictionary<string, PreGame> allGames, List<PreGame> allGamesList) {
            this.activeGames = activeGames; this.allGames = allGames; this.allGamesList = allGamesList;
        }

        #region Broadcasting
        //Update sent from the gameserver, here the master server broadcasts it to all spectators.
        private void handleGamelogUpdate(IIncommingMessage msg){
			SpectatorGameLog logMsg = msg.Deserialize<SpectatorGameLog> ();
			if (activeGames.ContainsKey (logMsg.broadcastID) == false)
				msg.Respond (ResponseStatus.Failed);
			else
                broadcastGameUpdate(logMsg);
        }
        private void broadcastGameUpdate(SpectatorGameLog logMsg) {
            foreach (IPeer p in activeGames[logMsg.broadcastID].getSpectatorsClone()) {
                try { p.SendMessage((short)(CustomMasterServerMSG.spectateLogUpdate), logMsg); } 
                catch { removeSpectator(logMsg.broadcastID, p); }
            }
        }

        //Later we should probebly store the log in the Master Server, so not to interupt the GameServer to much
        private void handleRequestSpecificUpdates(IIncommingMessage rawMsg) {
            SpectatorSpecificLogRequestMsg msg = rawMsg.Deserialize<SpectatorSpecificLogRequestMsg>();
            PreGame game;
            if (isValidAdmin(rawMsg) == false || findGame(msg.broadcastID, out game, rawMsg) == false)
                return;

            game.runningGameConnection.SendMessage((short)CustomMasterServerMSG.requestSpecificGameLog, msg, (s, m) => {
                rawMsg.Respond(m.Deserialize<SpectatorGameLog>(), s);});
        }
        #endregion

        #region PreGame handeling
        public void preGameStarted(PreGame game, RunningGameInfoMsg initMsg) {
			if (game.hasSpectators () == false)
				return;

            foreach(IPeer p in game.getSpectatorsClone())
                p.SendMessage((short)CustomMasterServerMSG.spectateGameStarted, initMsg);
		}
        public void preGameRemoved(PreGame game) {
            game.getSpectatorsClone().ForEach(p => removeSpectator(game, p));
        }
        #endregion

        #region Start spectate game
        private void handleStartSpectateGame(IIncommingMessage rawMsg){
            SpectatorSubscriptionsMsg subscribeMsg = rawMsg.Deserialize<SpectatorSubscriptionsMsg> ();
            PreGame game;
            if(isValidAdmin(rawMsg) == false || findGame(subscribeMsg.broadcastID, out game, rawMsg) == false) 
                return;

            if (adminToRooms.ContainsKey(rawMsg.Peer.Id) && adminToRooms[rawMsg.Peer.Id] != game) //If the admin is watching another game, we stop that subscription
                removeSpectator(adminToRooms[rawMsg.Peer.Id], rawMsg.Peer);

            if (game.containsSpectator(rawMsg.Peer)){
                rawMsg.Respond("Already subscribed to game", ResponseStatus.Error);
                return;
            }

            game.addSpectator(rawMsg.Peer);
            adminToRooms.Add(rawMsg.Peer.Id, game);

            rawMsg.Respond(game.storedInfoMsg, ResponseStatus.Success);
        }
        #endregion


        #region Stop spectating
        private void handleStopSpectateGame(IIncommingMessage rawMsg){
            if (isValidAdmin(rawMsg) == false)
                return;
            removeSpectator (adminToRooms[rawMsg.Peer.Id], rawMsg.Peer);
            rawMsg.Respond(ResponseStatus.Success);
		}

        private void removeSpectator(String broadcastID, IPeer peer) {
            if (allGames.ContainsKey(broadcastID))
                removeSpectator(allGames[broadcastID], peer);
        }
		private void removeSpectator(PreGame game, IPeer peer){
            if (adminToRooms.ContainsKey(peer.Id) != false)
                adminToRooms.Remove(peer.Id);
            if(game == null) 
                return;

            game.removeSpectator(peer.Id);
		}
        #endregion


        #region Lobby View
        private void handleRequestSpectatorGames(IIncommingMessage msg){
			if (SpectatorAuthModule.existsAdmin (msg.Peer) == false)
				return;
			byte[] bytes = allGamesList.Select(g => g.convertToGameInfoPacket()).Select(l => (ISerializablePacket)l).ToBytes();
			msg.Respond(bytes, ResponseStatus.Success);
		}
        #endregion


        #region Utils
        private bool findGame(string key, out PreGame game, IIncommingMessage rawMsg = null) {
            if (allGames.TryGetValue(key, out game))
                return true;

            if (rawMsg != null)
                rawMsg.Respond("Could not find matching game", ResponseStatus.Error);

            game = null;
            return false;
        }

        private bool isValidAdmin(IIncommingMessage rawMsg) {
            if (SpectatorAuthModule.existsAdmin(rawMsg.Peer) == false) {
                rawMsg.Respond("Peer ID is not a registred Admin", ResponseStatus.Error);
                return false;
            }
            return true;
        }
        #endregion
    }


    #region Messages
    public class SpectatorSubscriptionsMsg : MessageBase{
		public bool active;
		public string broadcastID;
	}

    public struct GameLogState {
        public string[] log;
        public int updateNumber;
    }
	public class SpectatorGameLog : MessageBase{
        public GameLogState[] gameLog;
        public string broadcastID;
	}
	public class SpectatorInfoMsg : MessageBase{
		public Game.GameType gameType;
		public PreGameState status;
	}

    public class SpectatorSpecificLogRequestMsg : MessageBase {
        public int[] IDs;
        public string broadcastID;
    } 
    #endregion
}
