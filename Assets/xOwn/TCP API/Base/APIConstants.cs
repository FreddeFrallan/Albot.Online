using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCP_API {
    public enum BoardState { PlayerWon, EnemyWon, Draw, Ongoing, }

    public class APIStandardConstants {

        public static class Fields {
            public const string board = "Board";
            public const string evaluate = "Evaluate";
            public const string possibleMoves = "PossMoves";
            public const string winner = "Winner";
            public const string move = "Move";
            public const string player = "Player";
            public const string enemy = "Enemy";
            public const string action = "Action";
            public const string boardState = "boardState";
        }

        public static class Actions {
            public const string makeMove = "MakeMove";
            public const string simMove = "SimulateMove";
            public const string evalBoard = "EvaluateBoard";
            public const string getPossMoves = "GetPossibleMoves";
        }
    }

}