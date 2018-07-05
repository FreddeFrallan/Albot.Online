using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TCP_API;
using TCP_API.Connect4;

public class Connect4EvaluateBoardTest{

    #region Diagonals
    [TestCase(BoardState.Ongoing,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(BoardState.PlayerWon,
        "0 0 0 0 0 0 0 " +
        "0 1 0 0 0 0 0 " +
        "0 0 1 0 0 0 0 " +
        "0 0 0 1 0 0 0 " +
        "0 0 0 0 1 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(BoardState.PlayerWon,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 1 0 " +
        "0 0 0 0 1 0 0 " +
        "0 0 0 1 0 0 0 " +
        "0 0 1 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(BoardState.PlayerWon,
        "0 0 0 1 0 0 0 " +
        "0 0 0 0 1 0 0 " +
        "0 0 0 0 0 1 0 " +
        "0 0 0 0 0 0 1 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(BoardState.PlayerWon,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "1 0 0 0 0 0 0 " +
        "0 1 0 0 0 0 0 " +
        "0 0 1 0 0 0 0 " +
        "0 0 0 1 0 0 0 "
        )]
    [TestCase(BoardState.PlayerWon,
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
    [TestCase(BoardState.Ongoing,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(BoardState.PlayerWon,
        "1 1 1 1 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(BoardState.PlayerWon,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 1 1 1 1 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(BoardState.PlayerWon,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 1 1 1 1 "
        )]
    [TestCase(BoardState.PlayerWon,
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
    [TestCase(BoardState.Ongoing,
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(BoardState.PlayerWon,
        "1 0 0 0 0 0 0 " +
        "1 0 0 0 0 0 0 " +
        "1 0 0 0 0 0 0 " +
        "1 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(BoardState.PlayerWon,
        "0 1 0 0 0 0 0 " +
        "0 1 0 0 0 0 0 " +
        "0 1 0 0 0 0 0 " +
        "0 1 0 0 0 0 0 " +
        "0 1 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(BoardState.PlayerWon,
        "0 0 0 0 0 0 0 " +
        "0 0 0 1 0 0 0 " +
        "0 0 0 1 0 0 0 " +
        "0 0 0 1 0 0 0 " +
        "0 0 0 1 0 0 0 " +
        "0 0 0 0 0 0 0 "
        )]
    [TestCase(BoardState.PlayerWon,
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
    [TestCase(BoardState.Ongoing,
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
    [TestCase(BoardState.Draw,
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
        if (expected == BoardState.PlayerWon)
            invertedExpected = BoardState.EnemyWon;
        else if (expected == BoardState.EnemyWon)
            invertedExpected = BoardState.PlayerWon;

        evaluateWin(expected, rawBoard);
        evaluateWin(invertedExpected, invertedBoard);
    }

    private void evaluateWin(BoardState expected, string board) {
        Board b = new Board(board, true);
        BoardState actual = BoardEvaluator.evaluateBoard(b);
        Assert.AreEqual(expected, actual, b.getWinChecks());
    }
}