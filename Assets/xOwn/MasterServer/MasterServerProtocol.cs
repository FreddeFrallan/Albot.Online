using AlbotServer;
using Barebones.MasterServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum CustomMasterServerMSG {
    login = 0,

    RunningGameInfo = 1,
    requestSpectatorGames = 2,
    spectateInfo = 3,
    gameOverResult,


    spectateLogUpdate,
    requestSpecificGameLog,
    spectateGameStarted,


    startSpectate,
    stopSpectate,
    adminLogin,
    adminLogout,

    //Tournament
    joinTournament,
    leaveTournament,
    startTournament,
    createTournament,
    closeTournament,
    preTournamentUpdate,
    runningTournamentUpdate,
    tournamentRoundPreStarted,
    tournamentRoundStarted,
    tournamentRoundForceWinner,
    tournamentRoundReconnectPlayer,

    //Player Data
    requestLoginData,
    requestGamesData,
}


public class GameOverMsg : MessageBase {
    public GameOverScore score;
    public GameLogState[] fullGameHistory;
    public string roomID;
}

public struct GameOverScore {
    public GameOverState winState;
    public string[] winOrder;
}

public enum GameOverState {
    playerWon, playerLeft, draw
}