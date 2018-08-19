using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TCP_API;
using TCP_API.Connect4;

public class Connect4EvaluateBoardTest{

    #region Diagonals
    [TestCase(BoardState.ongoing,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(BoardState.playerWon,
        "0 0 0 0 0 0 0 " +
        "0 1 0 0 0 0 0 " +
        "0 0 1 0 0 0 0 " +
        "0 0 0 1 0 0 0 " +
        "0 0 0 0 1 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(BoardState.playerWon,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 1 0 " +
        "0 0 0 0 1 0 0 " +
        "0 0 0 1 0 0 0 " +
        "0 0 1 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(BoardState.playerWon,
        "0 0 0 1 0 0 0 " +
        "0 0 0 0 1 0 0 " +
        "0 0 0 0 0 1 0 " +
        "0 0 0 0 0 0 1 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(BoardState.playerWon,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "1 0 0 0 0 0 0 " +
        "0 1 0 0 0 0 0 " +
        "0 0 1 0 0 0 0 " +
        "0 0 0 1 0 0 0 "
        )]
    [TestCase(BoardState.playerWon,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 1 " +
        "0 0 0 0 0 1 0 " +
        "0 0 0 0 1 0 0 " +
        "0 0 0 1 0 0 0 "
        )]
    public void evaluateDiagonals(BoardState expected, string board) {
        evaluateAndInvert(expected, board);
    }

    #endregion
    #region Rows
    [TestCase(BoardState.ongoing,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(BoardState.playerWon,
        "1 1 1 1 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(BoardState.playerWon,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 1 1 1 1 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(BoardState.playerWon,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 1 1 1 1 "
        )]
    [TestCase(BoardState.playerWon,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "1 1 1 1 1 1 1 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    public void evaluateRows(BoardState expected, string board) {
        evaluateAndInvert(expected, board);
    }
    #endregion
    #region Colls
    [TestCase(BoardState.ongoing,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(BoardState.playerWon,
        "1 0 0 0 0 0 0 " +
        "1 0 0 0 0 0 0 " +
        "1 0 0 0 0 0 0 " +
        "1 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(BoardState.playerWon,
        "0 1 0 0 0 0 0 " +
        "0 1 0 0 0 0 0 " +
        "0 1 0 0 0 0 0 " +
        "0 1 0 0 0 0 0 " +
        "0 1 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(BoardState.playerWon,
        "0 0 0 0 0 0 0 " +
        "0 0 0 1 0 0 0 " +
        "0 0 0 1 0 0 0 " +
        "0 0 0 1 0 0 0 " +
        "0 0 0 1 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(BoardState.playerWon,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 1 " +
        "0 0 0 0 0 0 1 " +
        "0 0 0 0 0 0 1 " +
        "0 0 0 0 0 0 1 "
        )]
    public void evaluateColls(BoardState expected, string board) {
        evaluateAndInvert(expected, board);
    }
    #endregion
    #region Earlier Problems
    [TestCase(BoardState.ongoing,
    "0 0 0 0 0 0 0 " +
    "0 0 0 0 0 0 0 " +
    "-1 0 0 0 0 0 0 " +
    "1 0 0 0 0 0 0 " +
    "1 0 0 -1 0 0 0 " +
    "1 0 0 -1 0 0 0 "
    )]
    public void evaluateEarlierProblems(BoardState expected, string board) {
        evaluateAndInvert(expected, board);
    }
    #endregion
    #region Draw
    [TestCase(BoardState.draw,
        "-1 -1 1 -1 -1 1 1 " +
        "1 -1 -1 1 1 -1 -1 " +
        "-1 1 -1 -1 -1 1 1 " +
        "1 -1 1 1 1 -1 -1 " +
        "-1 1 1 -1 -1 1 1 " +
        "1 -1 1 1 -1 -1 1 "
    )]
    public void evaluateDraw(BoardState expected, string board) {
        evaluateAndInvert(expected, board);
    }
    #endregion

    private void evaluateAndInvert(BoardState expected, string rawBoard) {
        string invertedBoard = rawBoard.Replace("1", "-1");
        BoardState invertedExpected = expected;// = expected * -1;
        if (expected == BoardState.playerWon)
            invertedExpected = BoardState.enemyWon;
        else if (expected == BoardState.enemyWon)
            invertedExpected = BoardState.playerWon;

        evaluateWin(expected, rawBoard);
        evaluateWin(invertedExpected, invertedBoard);
    }

    private void evaluateWin(BoardState expected, string board) {
        Board b = new Board(board, true);
        BoardState actual = BoardEvaluator.evaluateBoard(b);
        Assert.AreEqual(expected, actual, b.getWinChecks());
    }
}