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
            public static readonly float SPACE = 0.1f;
            public static readonly float START_X_POS = 0.125f;
            public static readonly float START_Y_POS = 0;
        }
    }
}