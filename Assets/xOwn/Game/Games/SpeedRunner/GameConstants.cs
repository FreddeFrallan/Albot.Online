using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpeedRunner {
    public class GameConstants {

        public class PlayerStartValues {
            public static readonly float startRunSpeed = 10;
            public static readonly float startJumpSpeed = 20;
            public static readonly float startJustCooldown = 0.7f;
            public static readonly int startAmountJumps = 1;
        }


        public class MapGenerator {
            public static readonly float SPAWN_DISTANCE = 40;
            public static readonly int POOL_SIZE = 5;
        }

        public class PlayerSight {
            public static readonly int ROWS = 16;
            public static readonly int COLS = 24;
            public static readonly float SIZE = 1f;
            public static readonly Vector3 START_POS = new Vector3(0.2f, 0.2f, 0);

            public static char EMPTY_SIGN = '_';
            public static char GROUND_SIGN = 'X';
        }

        public class MapProtocol {
            public static readonly int X_PRECISION = 2;
            public static readonly int Y_PRECISION = 2;
        }
    }
}