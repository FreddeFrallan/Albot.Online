using System.Collections.Generic;
using ExternalAPI.Connect4;
using Xunit;
using System;

namespace Connect4_API.Tests {
    public class EvaluateBoardTest {

        [Theory]
        [InlineData(-1, 
            "0 0 0 0 0 0 0 " +
            "0 0 -1 0 0 0 0 " +
            "0 0 0 -1 0 0 0 " +
            "0 0 0 0 -1 0 0 " +
            "0 0 0 0 0 -1 0 " +
            "0 0 0 0 0 0 0 "
            )]
        [InlineData(1,
            "0 0 0 0 0 0 0 " +
            "0 0 1 0 0 0 0 " +
            "0 0 0 1 0 0 0 " +
            "0 0 0 0 1 0 0 " +
            "0 0 0 0 0 1 0 " +
            "0 0 0 0 0 0 0 "
            )]
        [InlineData(1,
            "0 0 0 0 0 0 0 " +
            "0 0 0 0 0 0 0 " +
            "0 0 0 0 0 0 1 " +
            "0 0 0 0 0 1 0 " +
            "0 0 0 0 1 0 0 " +
            "0 0 0 1 0 0 0 "
            )]
        [InlineData(0,
            "0 0 0 0 0 0 0 " +
            "0 0 0 0 0 0 0 " +
            "0 0 0 0 0 0 1 " +
            "0 0 0 0 0 -1 0 " +
            "0 0 0 0 1 0 0 " +
            "0 0 0 1 0 0 0 "
            )]
        public void evaluateBoardDiagonals(int expected, string board) {
            Board b = new Board(board, true);
            int actual = BoardEvaluator.evaluateBoard(b);
            Assert.Equal(expected, actual);
        }


        [Theory]
        [InlineData(1, 
            "0 0 0 0 0 0 0 " +
            "0 0 1 1 1 1 0 " +
            "0 0 0 0 0 0 0 " +
            "0 0 0 0 0 0 0 " +
            "0 0 0 0 0 0 0 " +
            "0 0 0 0 0 0 0 "
            )]
        [InlineData(-1,
            "0 0 0 0 0 0 0 " +
            "0 0 0 0 0 0 0 " +
            "0 0 0 0 0 0 0 " +
            "0 0 0 0 0 0 0 " +
            "0 0 -1 -1 -1 -1 0 " +
            "0 0 0 0 0 0 0 "
            )]
        [InlineData(1,
            "0 0 0 0 0 0 0 " +
            "0 0 0 0 0 0 0 " +
            "0 0 0 0 0 0 0 " +
            "0 0 0 0 0 0 0 " +
            "0 0 0 0 0 0 0 " +
            "1 1 1 1 0 0 0 "
            )]
        [InlineData(0,
            "0 0 0 0 0 0 0 " +
            "0 0 0 0 0 0 0 " +
            "0 0 0 0 0 0 0 " +
            "0 0 0 0 0 0 0 " +
            "0 0 0 0 0 0 0 " +
            "0 0 0 0 0 0 0 "
            )]
        public void evaluateBoarRows(int expected, string board) {
            Board b = new Board(board, true);
            int actual = BoardEvaluator.evaluateBoard(b);
            Assert.Equal(expected, actual);
        }
    }
}
