using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeBot {
    public class Constants {

        #region Printing & Debuging
        public const char ENEMY_SIGN = 'E';
        public const char PLAYER_SIGN = 'P';
        public const char EMPTY_SIGN = '-';
        public const char BLOCKED_SIGN = 'X';
        #endregion

        public const int BOARD_SIZE = 10;
        public const int PLAYER_ID = 0;
        public const int ENEMY_ID = 1;
        public const int BLOCKED_BOARD_VALUE = -1;
        public const int PLAYER_BOARD_VALUE = 1;
        public const int ENEMY_BOARD_VALUE = 2;



        public class Fields {
            public const string board = "board";
            public const string evaluate = "Evaluate";
            public const string possibleMoves = "PossMoves";
            public const string winner = "Winner";
            public const string move = "Move";
            public const string player = "player";
            public const string enemy = "enemy";
            public const string blocked = "blocked";
            public const string action = "Action";

            public const string posX = "x";
            public const string posY = "y";
            public const string direction = "dir";
            public const string right = "right";
            public const string left = "left";
            public const string up = "up";
            public const string down = "down";
        }

        public static class Actions {
            public const string makeMove = "Move";
            public const string simMove = "SimulateMove";
            public const string evalBoard = "EvaluateBoard";
            public const string getPossMoves = "GetPossibleMoves";
        }
    }
}
