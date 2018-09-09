using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlbotServer;
using Tournament.Server;
using AdminUI;
using System;

namespace Tournament.Client {

    public class VisualTournamentRound : MonoBehaviour, TournamentUIObject {

        [SerializeField]
        private List<VisualTournamentPlayerSlot> playerSlots;
        [SerializeField]
        private RoundState state = RoundState.Empty;
        private RoundID id;
        private List<TournamentPlayer> players;
        private TournamentRound serverRound;
        private Action leftClick, rightClick;
        private string preGameRoomID;

        public void init(TournamentRound serverRound) {
            this.serverRound = serverRound;
            id = serverRound.getGameID();
            players = serverRound.getPlayers();
            state = serverRound.getState();
            preGameRoomID = serverRound.preGameRoomID;
            setPlayerSlots();
            initClicks();
        }

        private void initClicks() {
            if (AdminRunningTournamentManager.isAdmin() || TournamentTest.isTraining) {
                leftClick = adminLeftClick; rightClick = adminRightClick;
            }
            else {
                leftClick = clientLeftClick; rightClick = clientRightClick;
            }
        }

        private void setPlayerSlots() {
            for (int i = 0; i < players.Count; i++)
                playerSlots[i].setPlayer(players[i], state);
        }


        public void onLeftClick() {leftClick();}
        public void onRightClick() {rightClick();}


        #region Client
        private void clientLeftClick() {}
        private void clientRightClick() { }
        #endregion


        #region Admin
        private void adminLeftClick() {
            if (state == RoundState.Lobby || state == RoundState.Playing)
                AdminUpdateManager.startSpectateGame(preGameRoomID);
        }
        private void adminRightClick() {
            if (TournamentTest.isTraining) {
                TournamentTest.playGame(id.col, id.row);
                return;
            }
            if (state == RoundState.Lobby) {
                if (serverRound.canStartGame())
                    AdminRunningTournamentManager.startRoundGame(id);
            }
            else if (state == RoundState.Idle && serverRound.hasAllPlayers())
                AdminRunningTournamentManager.startRoundLobby(id);
        }
        #endregion
    }

}