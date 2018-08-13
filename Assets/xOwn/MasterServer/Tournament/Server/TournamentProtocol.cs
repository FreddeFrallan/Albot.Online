using AlbotServer;
using ClientUI;
using Game;
using System.Collections;
using System.Collections.Generic;
using Tournament.Server;
using UnityEngine;
using UnityEngine.Networking;

namespace Tournament {

    public class TournamentTreeUpdate : MessageBase {
        public TournamentRoundDTO[] rounds;
    }

    public struct TournamentRoundDTO {
        public PlayerInfo[] players;
        public RoundState state;
        public GameScore score;
        public RoundID ID;
    }

    public class TournamentInfoMsg : MessageBase {
        public GameType type;
        public string tournamentID;
        public int maxPlayers;
        public PlayerInfo[] players;
    }

    public class TournamentPreGameInfo : MessageBase {
        public string tournamentID;
        public RoundID roundID;
    }
}