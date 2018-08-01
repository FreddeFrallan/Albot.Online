﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;
using Barebones.Networking;
using System.Linq;
using UnityEngine.Networking;
using System;

namespace AlbotServer {

    public class AlbotPreGameModule : ServerModuleBehaviour {
        private static AlbotPreGameModule singleton;
        public AlbotSpectatorModule spectatorModule;

        private Dictionary<string, PreGame> currentPreGames = new Dictionary<string, PreGame>();
        private Dictionary<string, PreGame> activeGames = new Dictionary<string, PreGame>();
        private Dictionary<string, PreGame> allGamesDict = new Dictionary<string, PreGame>();
        private List<PreGame> allGames = new List<PreGame>();
        private List<PreGame> allPreGamesList = new List<PreGame>();
        private AlbotPreGameMaintence maintence = new AlbotPreGameMaintence();

        public override void Initialize(IServer server) {
            singleton = this;
            server.SetHandler((short)ServerCommProtocl.CreatePreGame, handleCreatePreGame);
            server.SetHandler((short)ServerCommProtocl.RequestJoinPreGame, handleRequestJoinPreGame);
            server.SetHandler((short)ServerCommProtocl.UpdatePreGame, handlePlayerReadyUpdate);
            server.SetHandler((short)ServerCommProtocl.StartPreGame, handleStartPreGame);
            server.SetHandler((short)ServerCommProtocl.RestartTrainingGame, handleRestartGame);
            server.SetHandler((short)ServerCommProtocl.SlotTypeChanged, handleSlotTypeChanged);
            server.SetHandler((short)ServerCommProtocl.StartSinglePlayerGame, handleStartSingleplayerGame);
            server.SetHandler((short)ServerCommProtocl.PlayerLeftPreGame, handlePlayerLeft);
            server.SetHandler((short)CustomMasterServerMSG.RunningGameInfo, handleRunningGamesInfoMsg);
            spectatorModule.initPreGames(activeGames, allGamesDict, allGames);
        }

        private IEnumerator removeOldgames() {
            while (true) {
                yield return new WaitForSeconds(15);
                maintence.cleanupOldPreGames();
            }
        }

        private void handleCreatePreGame(IIncommingMessage message) {
            PreGameSpecs msg = message.Deserialize<PreGameSpecs>();
            msg.roomID = generatePreGameID();

            PreGame newGame = new PreGame(message.Peer, msg, spectatorModule);
            currentPreGames.Add(msg.roomID, newGame);
            allGamesDict.Add(msg.roomID, newGame);
            allGames.Add(newGame);
            allPreGamesList.Add(newGame);
            message.Respond(msg.roomID, ResponseStatus.Success);
        }


        private void handleRequestJoinPreGame(IIncommingMessage rawMsg) {
            PreGameJoinRequest msg = rawMsg.Deserialize<PreGameJoinRequest>();
            PreGameRoomMsg returnMsg = new PreGameRoomMsg();
            PreGame targetGame;
            if (!findGame(msg.roomID, out targetGame, rawMsg) || !targetGame.peerJoined(rawMsg, msg.joiningPlayer, ref returnMsg))
                return;

            rawMsg.Respond(returnMsg, ResponseStatus.Success);
        }

        #region Slot Changes
        private void handlePlayerReadyUpdate(IIncommingMessage rawMsg) {
            PreGameReadyUpdate msg = rawMsg.Deserialize<PreGameReadyUpdate>();
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
            string errorMsg = "";

            if (findGame(roomID, out targetGame, rawMsg) == false)
                return;
            else if (targetGame.canGameStart() == false)
                errorMsg = "Not all players are ready";
            else if (targetGame.isAdmin(rawMsg.Peer) == false)
                errorMsg = "Only the Admin can start the game";

            if (string.IsNullOrEmpty(errorMsg) == false) {
                rawMsg.Respond(errorMsg, ResponseStatus.Failed);
                return;
            }


            allPreGamesList.Remove(targetGame);
            currentPreGames.Remove(roomID);
            activeGames.Add(targetGame.specs.roomID, targetGame); //Adding the game to the active pool, so it can be re-started easily.
            startGame(targetGame, rawMsg);
        }

        private void startGame(PreGame game, IIncommingMessage rawMsg) {
            string spawnCode = SpawnersModule.singleton.createNewRoomFromPreGame(game.getPeers(), game.generateGameSettings(), game.specs.roomID);
            if (string.IsNullOrEmpty(spawnCode)) { // We encountered some kind of error when spawning a new gameRoom
                rawMsg.Respond("Server error during game startup", ResponseStatus.Error);
                return;
            }

            game.specs.spawnCode = spawnCode;
            PreGameStartedMsg msg = new PreGameStartedMsg() { specs = game.specs, slots = game.getPlayerSlots() };
            Debug.LogError("Starting game: " + game.specs.roomID + " with: " + game.getPeers().Count + " peers.");

            GamesData.totallGamesPlayed++;
            game.onpreGameStarted();
            game.getPeers().ForEach(p => { p.SendMessage((short)ServerCommProtocl.GameRoomInvite, msg); });

            rawMsg.Respond(ResponseStatus.Success);
        }

        private void handleRestartGame(IIncommingMessage rawMsg) {
            if (activeGames.ContainsKey(rawMsg.AsString()) == false) {
                rawMsg.Respond("Could not find matching game", ResponseStatus.Error);
                return;
            }
            PreGame targetGame = activeGames[rawMsg.AsString()];
            if (targetGame.containsPeer(rawMsg.Peer) == false)
                return;

            targetGame.updatePeerReady(rawMsg.Peer, true);
            if (targetGame.canGameStart())
                startGame(targetGame, rawMsg);
        }
        #endregion

        #region Running Games
        private void handleRunningGamesInfoMsg(IIncommingMessage rawMsg) {
            RunningGameInfoMsg infoMsg = rawMsg.Deserialize<RunningGameInfoMsg>();
            if (activeGames.ContainsKey(infoMsg.gameID) == false)
                Debug.LogError("Game " + infoMsg.gameID + " started, but no such game was located in ActiveGames");
            else
                activeGames[infoMsg.gameID].onGameStarted(infoMsg, rawMsg);
        }
        #endregion



        #region Getters
        public static List<PreGame> getCurrentPreGames() { return singleton.allPreGamesList; }
        public static List<PreGame> getAllGames() { return singleton.allGames; }
        public static Dictionary<string, PreGame> getActiveGames() { return singleton.activeGames; }
        #endregion


        #region Keys
        private bool keyIsInUse(string key) { return allGames.Any(p => p.specs.roomID == key); }
        private string generatePreGameID() { return Msf.Helper.CreateRandomStringMatch(MasterServerConstants.KEY_LENGTH, keyIsInUse); }
        #endregion

        #region Utils
        public static void removeGame(PreGame game, string roomID) {
            Debug.LogError("Removing Game: " + roomID);
            if (game.state == PreGameState.Lobby) {
                singleton.currentPreGames.Remove(roomID);
                singleton.allPreGamesList.Remove(game);
            }
            else
                singleton.activeGames.Remove(roomID);

            singleton.allGames.Remove(game);
            singleton.allGamesDict.Remove(roomID);
            game.onRemoved();
        }
        private bool findGame(string key, out PreGame game, IIncommingMessage rawMsg = null) {
            if (allGamesDict.TryGetValue(key, out game))
                return true;

            if (rawMsg != null)
                rawMsg.Respond("Could not find matching game", ResponseStatus.Error);

            game = null;
            return false;
        }
        #endregion
    }
}