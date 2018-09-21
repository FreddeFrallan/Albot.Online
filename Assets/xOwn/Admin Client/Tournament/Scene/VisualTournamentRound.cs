using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlbotServer;
using Tournament.Server;
using AdminUI;
using System;
using System.Linq;

namespace Tournament.Client {

    public class VisualTournamentRound : MonoBehaviour, TournamentUIObject {

        [SerializeField]
        private List<VisualTournamentPlayerSlot> playerSlots;
        [SerializeField]
        private RoundState state = RoundState.Empty;
        public RoundType type = RoundType.normal;
        private RoundID id;
        private List<TournamentPlayer> players;
        private TournamentRound serverRound;
        private Action leftClick, rightClick;
        private string preGameRoomID;

        public void init(TournamentRound serverRound, RoundType type) {
            this.serverRound = serverRound;
            this.type = type;
            id = serverRound.getGameID();
            players = serverRound.getPlayers();
            state = serverRound.getState();
            preGameRoomID = serverRound.preGameRoomID;
            initPlayerSlotTypes();
            setPlayerSlots();
            initClicks();
            initPlayerSlots();
        }

        private void initPlayerSlots() {
            if (players.Where(p => p.slotIndex == 0).ToList().Count > 1)
                players[1].slotIndex = 1;
            if (players.Where(p => p.slotIndex == 1).ToList().Count > 1)
                players[0].slotIndex = 0;
        }


        private void initClicks() {
            if (AdminRunningTournamentManager.isAdmin() || TournamentTest.isTraining) {
                leftClick = adminLeftClick; rightClick = adminRightClick;
            }
            else {
                leftClick = clientLeftClick; rightClick = clientRightClick;
            }
        }

        private void initPlayerSlotTypes() {
            foreach (VisualTournamentPlayerSlot p in playerSlots) {
                p.setRoundType(type);
            }
        }

        private void setPlayerSlots() {
            foreach(TournamentPlayer p in players)
                playerSlots[p.slotIndex].setPlayer(p, state);
        }


        public void onLeftClick() {leftClick();}
        public void onRightClick() {rightClick();}


        #region Client
        private void clientLeftClick() {}
        private void clientRightClick() { }
        #endregion


        #region Admin
        private void adminLeftClick() {
            if (state == RoundState.Lobby || state == RoundState.Playing) {
                AdminRunningTournamentManager.onStartSpectating();
                AdminUpdateManager.startSpectateGame(preGameRoomID);
            }
        }
        private void adminRightClick() {
            if (TournamentTest.isTraining) {
                TournamentTest.playGame(id.col, id.row);
                return;
            }

            if (panicCommand())
                return;

            if (state == RoundState.Lobby) {
                if (serverRound.canStartGame())
                    AdminRunningTournamentManager.startRoundGame(id);
            }
            else if (state == RoundState.Idle && serverRound.hasAllPlayers())
                AdminRunningTournamentManager.startRoundLobby(id);
        }

        //Allows for special commands that can be used by holding one of these keys down while right clicking.
        private bool panicCommand() {
            if (Input.GetKey(KeyCode.X)) {//Cool force restart
                AdminRunningTournamentManager.forceRestartRoundLobby(id);
                return true;
            }

            if (Input.GetKey(KeyCode.Alpha1)) {
                AdminRunningTournamentManager.forceIndexWinner(id, 0);
                return true;
            }
            if (Input.GetKey(KeyCode.Alpha2)) {
                AdminRunningTournamentManager.forceIndexWinner(id, 1);
                return true;
            }

            return false;
        }
        #endregion
    }


}