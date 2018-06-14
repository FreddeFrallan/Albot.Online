using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TCP_API.Connect4;

public class Connect4EvaluateBoardTest{

    #region Diagonals
    [TestCase(0,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(1,
        "0 0 0 0 0 0 0 " +
        "0 1 0 0 0 0 0 " +
        "0 0 1 0 0 0 0 " +
        "0 0 0 1 0 0 0 " +
        "0 0 0 0 1 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(1,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 1 0 " +
        "0 0 0 0 1 0 0 " +
        "0 0 0 1 0 0 0 " +
        "0 0 1 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(1,
        "0 0 0 1 0 0 0 " +
        "0 0 0 0 1 0 0 " +
        "0 0 0 0 0 1 0 " +
        "0 0 0 0 0 0 1 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(1,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "1 0 0 0 0 0 0 " +
        "0 1 0 0 0 0 0 " +
        "0 0 1 0 0 0 0 " +
        "0 0 0 1 0 0 0 "
        )]
    [TestCase(1,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 1 " +
        "0 0 0 0 0 1 0 " +
        "0 0 0 0 1 0 0 " +
        "0 0 0 1 0 0 0 "
        )]
    public void evaluateDiagonals(int expected, string board) {
        evaluateAndInvert(expected, board);
    }

    #endregion
    #region Rows
    [TestCase(0,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(1,
        "1 1 1 1 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(1,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 1 1 1 1 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(1,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 1 1 1 1 "
        )]
    [TestCase(1,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "1 1 1 1 1 1 1 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    public void evaluateRows(int expected, string board) {
        evaluateAndInvert(expected, board);
    }
    #endregion
    #region Colls
    [TestCase(0,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(1,
        "1 0 0 0 0 0 0 " +
        "1 0 0 0 0 0 0 " +
        "1 0 0 0 0 0 0 " +
        "1 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(1,
        "0 1 0 0 0 0 0 " +
        "0 1 0 0 0 0 0 " +
        "0 1 0 0 0 0 0 " +
        "0 1 0 0 0 0 0 " +
        "0 1 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(1,
        "0 0 0 0 0 0 0 " +
        "0 0 0 1 0 0 0 " +
        "0 0 0 1 0 0 0 " +
        "0 0 0 1 0 0 0 " +
        "0 0 0 1 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(1,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 1 " +
        "0 0 0 0 0 0 1 " +
        "0 0 0 0 0 0 1 " +
        "0 0 0 0 0 0 1 "
        )]
    public void evaluateColls(int expected, string board) {
        evaluateAndInvert(expected, board);
    }
    #endregion
    #region Earlier Problems
    [TestCase(0,
    "0 0 0 0 0 0 0 " +
    "0 0 0 0 0 0 0 " +
    "-1 0 0 0 0 0 0 " +
    "1 0 0 0 0 0 0 " +
    "1 0 0 -1 0 0 0 " +
    "1 0 0 -1 0 0 0 "
    )]
    public void evaluateEarlierProblems(int expected, string board) {
        evaluateAndInvert(expected, board);
    }
    #endregion

    private void evaluateAndInvert(int expected, string rawBoard) {
        string invertedBoard = rawBoard.Replace("1", "-1");
        int invertedExpected = expected * -1;

        evaluateWin(expected, rawBoard);
        evaluateWin(invertedExpected, invertedBoard);
    }

    private void evaluateWin(int expected, string board) {
        Board b = new Board(board, true);
        int actual = BoardEvaluator.evaluateBoard(b);
        Assert.AreEqual(expected, actual, b.getWinChecks());
    }
}