using AlbotServer;
using Barebones.Networking;
using Game;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tournament.Server {

    public class TournamentRound {

        public bool hasPossibleBotGame = false;
        public int stashedRoundsUntilBotGame; //Simple optimization
        public TournamentRound nextRound, nextLoserRound;
        public PreGame thePregame;
        public string preGameRoomID;

        private PreGameSpecs gameSpecs;
        private List<TournamentPlayer> players = new List<TournamentPlayer>();
        private Dictionary<string, int> playerSlotIndex = new Dictionary<string, int>();
        private GameScore score = new GameScore() {wins = new int[] {0, 0 }, draws = 0, roundsCounter = 0 };
        private GameWinRules rules = new GameWinRules() { winScore = 1, playUntilWinner = true, maxRounds = 3};
        private bool playingUntilWinner = true;

        //Server variables
        private IPeer admin, extraHost;
        private bool isServer = false;
        private RunningTournamentGame theTournament;
        private string winnerUsername = "";
        private int nextWinGameSlotIndex, nextLoseGameSlotIndex;

        private RoundState state = RoundState.Empty;
        public RoundID id;


        public TournamentRound(int col, int row, PreGameSpecs gameSpecs) {
            id = new RoundID() { col = col, row = row };
            this.gameSpecs = ServerUtils.clonePreGameSpecs(gameSpecs);
            this.gameSpecs.tournamentRoundID = id;
        }

        #region Gameplay
        public void initAndInvite(bool forceRestart) {
            if (forceRestart == false && (state != RoundState.Idle && hasAllPlayers() == false))
                return;

            thePregame = AlbotPreGameModule.createTournamentGame(gameSpecs, admin);
            if (getNonNPCPlayers().Count == 0) { //If we have no human players in the game, we automaticly select a bot to win the game
                forceRandomWinner();
                return;
            }

            initHostAndBots();
            getNonNPCPlayers().ForEach(p => p.peer.SendMessage((short)CustomMasterServerMSG.tournamentRoundPreStarted, thePregame.specs.roomID));
            setState(RoundState.Lobby);
        }

        private void initHostAndBots() {
            extraHost = players.Find(p => p.getIsNPC() == false).peer;
            thePregame.setExtraHost(extraHost);
            insertBots();
        }

        private void insertBots() {
            if (getNPCPlayers().Count == 0)
                return;

            TournamentPlayer bot = getNPCPlayers()[0];
            PreGameSlotInfo botSlot = new PreGameSlotInfo() { slotID = 1, playerInfo = LocalTrainingBots.StandardTrainingBotInfo, isReady = true, type = PreGameSlotType.TrainingBot };
            thePregame.updateSlotType(botSlot, extraHost);
        }

        public void startGame() {
            AlbotPreGameModule.startTournamentgame(thePregame);
            score.roundsCounter++;
            setState(RoundState.Playing);
        }
        #endregion

        #region GameResults
        private int extractWinningPlayerIndex(string[] winOrder) {
            return players.FindIndex(p => p.info.username == winOrder[0]);
        }
        public void reportResult(GameOverMsg result) {
            AlbotPreGameModule.removeGame(thePregame, thePregame.specs.roomID);
            if (result.score.winState == GameOverState.draw)
                score.draws++;
            else
                score.wins[extractWinningPlayerIndex(result.score.winOrder)]++;
            
            checkForGameOver();
            if (state != RoundState.Over)
                rematch();
        }
        private void rematch() { setState(RoundState.Idle); }

        private void checkForGameOver() {
            for (int i = 0; i < players.Count; i++)
                if (score.wins[i] >= rules.winScore) {
                    setGameOver(players[i]);
                    return;
                }

             if (score.roundsCounter >= rules.maxRounds)
                setGameOver(players[getRandomWinner()]);
        }

        public void forceRandomWinner() {
            int randomPlayer = getRandomWinner();
            setGameOver(players[randomPlayer]);
        }
        private List<TournamentPlayer> getLosers(TournamentPlayer winner) { return players.Where(p => p != winner).ToList(); }
        private int getRandomWinner() {return (Random.Range(0, 100) > 50) ? 0 : 1;}
        public void setGameOver(TournamentPlayer winner) {
            winner.isWinning = true;
            score.winner = winner.info.username;
            winnerUsername = winner.info.username;
            setState(RoundState.Over);

            if (nextRound != null)
                nextRound.addPlayer(winner, nextWinGameSlotIndex);
            if(nextLoserRound != null) 
                getLosers(winner).ForEach(p => nextLoserRound.addPlayer(p, nextLoseGameSlotIndex));
        }
        #endregion

        #region Setters
        private void setState(RoundState newState) {
            state = newState;
            if(theTournament != null)
                theTournament.updateRound(id);
        }
        public void setNewId(int col, int row) {
            id = new RoundID() { col = col, row = row };
            this.gameSpecs.tournamentRoundID = id;
        }
        public void setNextLoserGame(TournamentRound nextRound, int nextLoseGameSlotIndex) {
            this.nextLoserRound = nextRound;
            this.nextLoseGameSlotIndex = nextLoseGameSlotIndex;
        }
        public void setNextGame(TournamentRound nextRound, int nextGameSlotIndex) {
            this.nextRound = nextRound;
            this.nextWinGameSlotIndex = nextGameSlotIndex;
        }
        public void setServerVariables(IPeer admin, RunningTournamentGame runningGame) {
            isServer = true;
            this.admin = admin;
            theTournament = runningGame;
        }
        public void setToTournamentDTO(TournamentRoundDTO dto) {
            players = dto.players.Select(p => new TournamentPlayer() { info = p.info, isReady = p.isReady, slotIndex = p.slotIndex}).ToList();
            if(dto.state == RoundState.Over) {
                try {
                    players.Find(p => p.info.username == dto.winner).isWinning = true;
                } catch {  }
            } 

            score = dto.score;
            state = dto.state;
            preGameRoomID = dto.preGameID;
        }

        public void addPlayer(TournamentPlayer player, int id = -1) {
            try {
                if (id == -1)
                    playerSlotIndex.Add(player.info.username, players.Count);
                else
                    playerSlotIndex.Add(player.info.username, id);
            } catch { }

            players.Add(player);
            if (player.getIsNPC())
                setHasPossibleBotGame();
            setState(RoundState.Idle);
        }
        #endregion

        #region Getters
        public bool hasAllPlayers() { return players.Count >= 2; }
        public bool canStartGame() { return hasAllPlayers() && getNonNPCPlayers().All(p => p.isReady); }
        private List<TournamentPlayer> getNPCPlayers() { return players.Where(p => p.getIsNPC()).ToList(); }
        private List<TournamentPlayer> getNonNPCPlayers() { return players.Where(p => p.getIsNPC() == false).ToList(); }
        public bool canStartRound() {return thePregame.canGameStart();}
        public RoundState getState() { return state; }
        public RoundID getGameID() { return id; }
        public List<TournamentPlayer> getPlayers() { return players; }
        public TournamentRoundDTO createDTO() {
            string roomID = state == RoundState.Lobby || state == RoundState.Playing ? thePregame.specs.roomID : "";
            TournamentPlayerDTO[] pDTO = players.Select(p => new TournamentPlayerDTO() {
                info = p.info,
                isReady = state == RoundState.Lobby ? getPlayerPreGameReady(p) : true,
                slotIndex = playerSlotIndex[p.info.username]
            }).ToArray();

            return new TournamentRoundDTO() {players = pDTO, state = this.state, ID = id, score = score,preGameID = roomID, winner = winnerUsername};
        }
        private bool getPlayerPreGameReady(TournamentPlayer p) {
            if (p.getIsNPC())
                return true;

            try { return thePregame.isPlayerReady(p.peer.Id); }
            catch {} return false;
        }
        #endregion

        #region PossibleBotGames
        public int calcRoundsUntilPossibleBotGame() {
            int steps = 0;
            TournamentRound round = this;

            while(round.hasPossibleBotGame == false && round.nextRound != null) {
                round = round.nextRound;
                steps++;
            }
            stashedRoundsUntilBotGame = steps;
            return steps;
        }

        private void setHasPossibleBotGame() {
            TournamentRound game = this;
            do{
                game.hasPossibleBotGame = true;
                game = game.nextRound;
            }while (game != null) ;
        }
        #endregion
    }





    public struct RoundID {public int col, row;}
    public struct GameScore {
        public int[] wins;
        public int draws, roundsCounter;
        public string winner;
    }
    public struct GameWinRules {
        public int winScore, maxRounds;
        public bool playUntilWinner;
    }
}