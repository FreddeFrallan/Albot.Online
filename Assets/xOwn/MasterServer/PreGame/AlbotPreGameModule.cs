using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;
using Barebones.Networking;
using System.Linq;
using UnityEngine.Networking;
using System;

namespace AlbotServer{

	public class AlbotPreGameModule : ServerModuleBehaviour {
		private static AlbotPreGameModule singleton;
		public AlbotSpectatorModule spectatorModule;

        private Dictionary<string, PreGame> currentPreGames = new Dictionary<string, PreGame>();
        private Dictionary<string, PreGame> activeGames = new Dictionary<string, PreGame>();
        private List<Dictionary<string, PreGame>> allGameDicts = new List<Dictionary<string, PreGame>>();
        private List<PreGame> allGames = new List<PreGame>();
        private List<PreGame> allPreGames = new List<PreGame>();

        public override void Initialize (IServer server){
			singleton = this;
			server.SetHandler ((short)ServerCommProtocl.CreatePreGame, handleCreatePreGame);
			server.SetHandler ((short)ServerCommProtocl.RequestJoinPreGame, handleRequestJoinPreGame);
			server.SetHandler ((short)ServerCommProtocl.UpdatePreGame, handlePlayerReadyUpdate);
			server.SetHandler ((short)ServerCommProtocl.StartPreGame, handleStartPreGame);
			server.SetHandler ((short)ServerCommProtocl.RestartTrainingGame, handleRestartGame);
			server.SetHandler ((short)ServerCommProtocl.SlotTypeChanged, handleSlotTypeChanged);
            server.SetHandler((short)ServerCommProtocl.StartSinglePlayerGame, handleStartSingleplayerGame);
            server.SetHandler((short)ServerCommProtocl.PlayerLeftPreGame, handlePlayerLeft);
            
            allGameDicts.Add(currentPreGames);
            allGameDicts.Add(activeGames);
        }

		private void handleCreatePreGame(IIncommingMessage message){
            PreGameSpecs msg = message.Deserialize<PreGameSpecs> ();
            msg.roomID = generatePreGameID();

            PreGame newGame = new PreGame (message.Peer, msg); 
			currentPreGames.Add (msg.roomID, newGame);
            allGames.Add(newGame);
            allPreGames.Add(newGame);
            message.Respond(msg.roomID, ResponseStatus.Success);
		}
        

		private void handleRequestJoinPreGame(IIncommingMessage rawMsg){
			PreGameJoinRequest msg = rawMsg.Deserialize<PreGameJoinRequest> ();
            PreGameRoomMsg returnMsg = new PreGameRoomMsg();
            PreGame targetGame;
            if (!findGame(msg.roomID, out targetGame, rawMsg) || !targetGame.peerJoined(rawMsg, msg.joiningPlayer, ref returnMsg))
                return;
           
            rawMsg.Respond(returnMsg, ResponseStatus.Success);
        }

        #region Slot Changes
        private void handlePlayerReadyUpdate(IIncommingMessage rawMsg) {
			PreGameReadyUpdate msg = rawMsg.Deserialize<PreGameReadyUpdate> ();
            PreGame targetGame;
            if (findGame(msg.roomID, out targetGame, rawMsg) && targetGame.containsPeer(rawMsg.Peer))
                targetGame.updatePeerReady(rawMsg.Peer, msg.isReady);
		}

        private void handleSlotTypeChanged(IIncommingMessage rawMsg) {
            PreGameSlotSTypeMsg msg = rawMsg.Deserialize<PreGameSlotSTypeMsg>();
            PreGame targetGame;
            if (findGame(msg.roomID, out targetGame, rawMsg))
                targetGame.updateSlotType(msg.slot, rawMsg.Peer);
        }

        private void handlePlayerLeft(IIncommingMessage rawMsg) {
            PreGame targetGame;
            if (findGame(rawMsg.AsString(), out targetGame, rawMsg) && targetGame.containsPeer(rawMsg.Peer))
                targetGame.peerLeft(rawMsg.Peer);
        }
        #endregion


        #region Staring Games
        private void handleStartSingleplayerGame(IIncommingMessage rawMsg) { GamesData.totallGamesPlayed++; }
		private void handleStartPreGame(IIncommingMessage rawMsg) { 
            PreGame targetGame;
            string roomID = rawMsg.AsString();
            Debug.LogError("Trying to start game: " + roomID);
            string errorMsg = "";

            if (findGame(roomID, out targetGame, rawMsg) == false)
                return;
            else if (targetGame.canGameStart() == false)
                errorMsg = "Not all players are ready";
            else if (targetGame.isAdmin(rawMsg.Peer) == false)
                errorMsg = "Only the Admin can start the game";
			
            if(string.IsNullOrEmpty(errorMsg) == false) {
                rawMsg.Respond(errorMsg, ResponseStatus.Failed);
                return;
            }

			
            currentPreGames.Remove(roomID);
            allPreGames.Remove(targetGame);
            activeGames.Add (roomID, targetGame); //Adding the game to the active pool, so it can be re-started easily.
			startGame (targetGame, rawMsg);
		}

		private void startGame(PreGame targetGame, IIncommingMessage rawMsg) {
            string newGameRoomId = SpawnersModule.singleton.createNewRoomFromPreGame(targetGame.getPeers(), targetGame.generateGameSettings());
            if (string.IsNullOrEmpty(newGameRoomId)) { // We encountered some kind of error when spawning a new gameRoom
                rawMsg.Respond("Server error during game startup", ResponseStatus.Error);
                return;
            }
            rawMsg.Respond(ResponseStatus.Success);


            PreGameStartedMsg msg = new PreGameStartedMsg() { specs = targetGame.specs, gameRoomID = newGameRoomId, slots = targetGame.getPlayerSlots()};
            Debug.LogError("Starting game: " + targetGame.specs.roomID + " with: " + targetGame.getPeers().Count + " peers.");

            GamesData.totallGamesPlayed++;
            targetGame.gameStarted();
            spectatorModule.preGameStarted(targetGame, newGameRoomId);//Allerting pending spectators
            foreach (IPeer p in targetGame.getPeers())
                p.SendMessage((short)ServerCommProtocl.GameRoomInvite, msg);
		}

		private void handleRestartGame(IIncommingMessage rawMsg){
            PreGame targetGame;
            Debug.LogError("Restart game: " + rawMsg.AsString());
            if (findGame(rawMsg.AsString(), out targetGame, rawMsg) == false || targetGame.containsPeer(rawMsg.Peer) == false)
                return;

            targetGame.updatePeerReady(rawMsg.Peer, true);
            Debug.LogError("Peer ready for : " + targetGame.specs.roomID);
            if (targetGame.canGameStart()) {
                Debug.LogError("Restaring training game: " + rawMsg.AsString());
                startGame(targetGame, rawMsg);
            }
		}
        #endregion



        #region Getters
        public static List<PreGame> getCurrentPreGames() { return singleton.allPreGames; }
        public List<PreGame> getAllGames() { return allGames; }
        #endregion


        #region Keys
        private bool keyIsInUse(string key) { return allGames.Any(p => p.specs.roomID == key); }
        private string generatePreGameID() {return Msf.Helper.CreateRandomStringMatch( MasterServerConstants.KEY_LENGTH,keyIsInUse);}
        #endregion

        #region Utils
        public static void removeGame(PreGame game, string roomID) {
            Debug.LogError("Removing game: " + game.specs.roomID);
            iterateAllDicts(d => { if (d.ContainsKey(roomID)) d.Remove(roomID); });
            if (singleton.allGames.Contains(game))
                singleton.allGames.Remove(game);
        }
        private static void iterateAllDicts(Action<Dictionary<string, PreGame>> a) {singleton.allGameDicts.ForEach(d => a(d));}
        private bool findGame(string key, out PreGame game, IIncommingMessage rawMsg = null) {
            foreach (Dictionary<string, PreGame> dict in allGameDicts)
                if (dict.TryGetValue(key, out game))
                    return true;

            if (rawMsg != null)
                rawMsg.Respond("Could not find matching game", ResponseStatus.Error);

            game = null;
            return false;
        }
        #endregion
    }
}