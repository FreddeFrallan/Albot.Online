using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeBot {
    class BoardParser {

        public static void parseBoard(string rawMsg, ref Board currentBoard){
            Console.WriteLine(rawMsg);
            JSONObject obj = new JSONObject(rawMsg);
            JSONObject player = obj.GetField(Constants.Fields.player);
            JSONObject enemy = obj.GetField(Constants.Fields.enemy);
            JSONObject blocked = obj.GetField(Constants.Fields.blocked);
            currentBoard.handleUpdate(blocked.list, player, enemy);
        }

    }
}
