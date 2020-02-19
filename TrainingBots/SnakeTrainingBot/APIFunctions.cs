using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static SnakeBot.Constants.Fields;
using static SnakeBot.SnakeStructs;

namespace SnakeBot {
    class APIFunctions {

        public static List<string> getPossibleMoves(string direction) {
            return TCP_API.Snake.Constants.Movement.getPossibleMovesFromDir(direction);
        }
        
    }
}
