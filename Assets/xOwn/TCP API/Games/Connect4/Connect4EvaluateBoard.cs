using System.Collections;
using System.Collections.Generic;

namespace TCP_API.Connect4{

    public class BoardEvaluator{
    
        
        public static BoardState evaluateBoard(Board board) {
            foreach(string c in board.winChecks) {
                if (c.Contains("1111"))
                    return BoardState.playerWon;
                if (c.Contains("2222"))
                    return BoardState.enemyWon;
            }

            // Should make use of getPossibleMoves()?
            for (int i = 0; i < Consts.BOARD_WIDTH; i++)
                if (board.grid[i, 0] == "0") // There is a possible move, and noone has won => ongoing
                    return BoardState.ongoing;

            return BoardState.draw;
        }
        

    }
}