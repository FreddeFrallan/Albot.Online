using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tournament.Server {

    public class TournamentRound {

        public bool hasPossibleBotGame = false;
        public int stashedRoundsUntilBotGame; //Simple optimization
        public TournamentRound nextRound;

        private List<TournamentPlayer> players = new List<TournamentPlayer>();
        private GameScore score = new GameScore() {p1Wins = 0, p2Wins = 0, draws = 0, roundsCounter = 0 };
        private GameWinRules rules = new GameWinRules() { winScore = 1, playUntilWinner = true };
        private bool playingUntilWinner = true;

        private GameState state = GameState.Empty;
        private GameID id;


        public TournamentRound(int col, int row) {
            id = new GameID() { col = col, row = row };
        }


        #region Gameplay
        public void startGame() {
            score.roundsCounter++;
            setState(GameState.Playing);

            //Debug
            reportResult(getRandomRoundResult());
        }

        public void reportResult(GameResult result) {
            if (result == GameResult.Player1) score.p1Wins++;
            else if (result == GameResult.Player2) score.p2Wins++;
            else score.draws++;

            checkForGameOver();
            if (playingUntilWinner && state != GameState.Over)
                startGame();
        }

        private void checkForGameOver() {
            if (score.p1Wins >= rules.winScore)
                setGameOver(players[0]);
            else if (score.p2Wins >= rules.winScore)
                setGameOver(players[1]);
        }

        public void setGameOver(TournamentPlayer winner) {
            setState(GameState.Over);
            score.winner = winner;
            if (nextRound != null)
                nextRound.addPlayer(winner);
        }


        private GameResult getRandomRoundResult() {
            int value = Random.Range(0, 100);
            if (value <= 33) return GameResult.Draw;
            else if (value <= 66) return GameResult.Player1;
            return GameResult.Player2;
        }
        #endregion

        #region Setters
        private void setState(GameState newState) { state = newState; }
        public void setNextGame(TournamentRound nextGame) { this.nextRound = nextRound; }

        public void addPlayer(TournamentPlayer player) {
            setState(GameState.Lobby);
            players.Add(player);
            if (player.getIsNPC())
                setHasPossibleBotGame();
        }
        #endregion

        #region Getters
        public GameState getState() { return state; }
        public GameID getGameID() { return id; }
        public List<TournamentPlayer> getPlayers() { return players; }
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
            }while (game.nextRound != null) ;
        }
        #endregion
    }


    public struct GameID {public int col, row;}
    public struct GameScore {
        public int p1Wins, p2Wins, draws, roundsCounter;
        public TournamentPlayer winner;
    }
    public struct GameWinRules {
        public int winScore, maxRounds;
        public bool playUntilWinner;
    }
    public enum GameResult {Player1, Player2, Draw,}
}