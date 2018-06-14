using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeBot {
    class BoardSimulator {

        public Board simulateMove(Board currentBoard, int move) {
            Board temp = currentBoard.deepCopy();
            return temp;
        }


    }
}
