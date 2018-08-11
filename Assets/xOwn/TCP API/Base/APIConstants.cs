using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCP_API {
    public enum BoardState { PlayerWon, EnemyWon, Draw, Ongoing, }

    public class APIStandardConstants {

        public static class Fields {
            public const string board = "board";
            public const string evaluate = "evaluate";
            public const string possibleMoves = "possMoves";
            public const string winner = "winner";
            public const string move = "move";
            public const string player = "player";
            public const string enemy = "enemy";
            public const string action = "action";
            public const string boardState = "boardState";
            public const string gameOver = "gameOver";
        }

        public static class Actions {
            public const string restartGame = "restartGame";
            public const string makeMove = "makeMove";
            public const string simMove = "simulateMove";
            public const string evalBoard = "evaluateBoard";
            public const string getPossMoves = "getPossibleMoves";
        }
    }

}