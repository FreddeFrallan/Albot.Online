using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeBot {
    public class SnakeStructs {

        public struct MoveScore {
            public string dir;
            public int score;
        }
        public struct PossibleMoves {
            public List<string> player;
            public List<string> enemy;
        }
        public struct Position {
            public int x;
            public int y;
        }
        public struct Player {
            public int x, y;
            public string dir;
            public int id;
        }

    }
}
