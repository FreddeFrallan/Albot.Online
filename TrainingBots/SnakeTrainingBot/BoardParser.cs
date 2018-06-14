using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeBot {
    class BoardParser {

        public static void parseBoard(string rawMsg, ref Board currentBoard){
            Console.WriteLine(rawMsg);
            JSONObject obj = new JSONObject(rawMsg);
            JSONObject player = obj.GetField("Player");
            JSONObject enemy = obj.GetField("Enemy");
            JSONObject blocked = obj.GetField("Blocked");
            currentBoard.handleUpdate(blocked.list, player, enemy);
        }

    }
}
