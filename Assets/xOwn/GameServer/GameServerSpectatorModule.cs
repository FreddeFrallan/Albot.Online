using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.IO;
using Barebones.MasterServer;
using Barebones.Networking;
using Game;
using System.Linq;

namespace AlbotServer {

    public class GameServerSpectatorModule {

        private string gameHistoryFolder = "PlayedGames";
        private bool broadcastToSpectators = false;
        private string broadcastID = "none";
        private GameMaster controller;
        private GameHistory historyController;
        

        public void init(GameHistory historyController) {
            this.historyController = historyController;
        }


        public GameServerSpectatorModule() {
            initHandlers();
            MsfArgs.runAfterArgsInit(() => {
                broadcastToSpectators = bool.Parse(Msf.Args.Spectators);
                broadcastID = Msf.Args.SpawnId;
            });
        }

        private void initHandlers() {
            Msf.Connection.SetHandler((short)CustomMasterServerMSG.RunningGameInfo, handleSetSubscribeStatus);
            Msf.Connection.SetHandler((short)CustomMasterServerMSG.requestSpecificGameLog, handleRequestSpecificGameLogs);
        }

        #region handlers
        private void handleSetSubscribeStatus(IIncommingMessage rawMsg) {broadcastToSpectators = bool.Parse(rawMsg.AsString());}

        private void handleRequestSpecificGameLogs(IIncommingMessage rawMsg) {
            SpectatorSpecificLogRequestMsg msg = rawMsg.Deserialize<SpectatorSpecificLogRequestMsg>();
            try { rawMsg.Respond(new SpectatorGameLog() {gameLog = historyController.getSpecificStates(msg.IDs)}, ResponseStatus.Success);
            } catch {rawMsg.Respond(ResponseStatus.Error);}
        }

        #endregion


        #region OnGameStarted
        public void onGameStarted(List<ConnectedPlayer> players) { sendStartInitMsg(players); }
        private void sendStartInitMsg(List<ConnectedPlayer> players) {
            Msf.Connection.SendMessage((short)CustomMasterServerMSG.RunningGameInfo, new RunningGameInfoMsg() {
                gameType = Msf.Args.GameType,
                players = players.Select(p => p.getPlayerInfo()).ToArray(),
                gameID = broadcastID
            });
        }
        #endregion


        public void updateAdded(GameLogState update) {
            if (broadcastToSpectators)
                sendUpdate(update);
        }

        private void sendUpdate(GameLogState update) {
            try {Msf.Connection.SendMessage((short)CustomMasterServerMSG.spectateLogUpdate, generateLogMsg(update), handleBroadcastResponse);
            } catch { Debug.LogError("Failed to send msg");}
        }

        private SpectatorGameLog generateLogMsg(GameLogState update) {
            return new SpectatorGameLog() {
                broadcastID = broadcastID,
                gameLog = new GameLogState[] { update }
            };
        }

        private void handleBroadcastResponse(ResponseStatus status, IIncommingMessage rawMsg) {
            if (status == ResponseStatus.Error)
                broadcastToSpectators = false;
        }


    }
}