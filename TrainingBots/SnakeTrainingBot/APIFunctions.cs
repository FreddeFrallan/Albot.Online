using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static SnakeBot.Constants.Fields;
using static SnakeBot.SnakeStructs;

namespace SnakeBot {
    class APIFunctions {

        public static List<string> getPossibleMoves(string direction) {
            switch (direction) {
                case right: return new List<string> { up, right, down }; break;
                case up: return new List<string> { left, up, right }; break;
                case left: return new List<string> { down, left, up }; break;
                default: return new List<string> { left, down, right }; break;
            }
        }
    }
}
