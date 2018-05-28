using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCP_API.Connect4 {

    public class Connect4APIEvaluateBoard{
    
        
        public static int evaluateBoard(Board board) {
            foreach(string c in board.winChecks) {
                if (c.Contains("1111"))
                    return 1;
                if (c.Contains("-1-1-1-1"))
                    return -1;
            }

            return 0;
        }
        

    }
}