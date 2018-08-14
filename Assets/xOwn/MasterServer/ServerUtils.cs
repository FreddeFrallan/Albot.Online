using Tournament;
using Game;
using Tournament.Server;

namespace AlbotServer {

    public class ServerUtils{

        public static PreGameSpecs tournamentInfoToGameSpecs(TournamentInfoMsg info) {
            PreGameSpecs specs = generateGameSpecs(info.type, "Admin", false, false);
            specs.isInTournament = true;
            specs.tournamentID = info.tournamentID;
            return specs;
        }
        public static PreGameSpecs generateGameSpecs(GameType type, string hostName, bool canRestart, bool showInLobby) {
            return new PreGameSpecs() { 
                type = type,
                maxPlayers = 2,
                showInLobby = showInLobby,
                canRestart = canRestart,
                hostName = hostName,
            };
        }

        public static PreGameSpecs clonePreGameSpecs(PreGameSpecs specs) {
            return new PreGameSpecs() {
                type = specs.type,
                roomID = specs.roomID,
                spawnCode = specs.spawnCode,
                maxPlayers = specs.maxPlayers,

                hostName = specs.hostName,
                showInLobby = specs.showInLobby,
                tournamentRoundID = specs.tournamentRoundID,
                isInTournament = specs.isInTournament,
                tournamentID = specs.tournamentID,
                canRestart = specs.canRestart,
            };
        }
    }
}