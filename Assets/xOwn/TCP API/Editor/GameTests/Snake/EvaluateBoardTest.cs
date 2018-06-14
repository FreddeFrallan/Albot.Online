using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

namespace TCP_API.Snake {

    public class EvaluateBoardTest {

        [TestCase(
        BoardState.Ongoing, "Right", "Left",
        "0 0 0 0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 0 0 0 " +
        "0 0 P 0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 E 0 0 " +
        "0 0 0 0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 0 0 0 "
        )]
        public void evaluateBoard(BoardState expected, string playerDir, string enemyDir, string rawBoard) {
            Board b = SnakeTestUtils.generateBoard(rawBoard, playerDir, enemyDir);
            Assert.AreEqual(expected, b.evaluateBoard());
        }

        #region EnemyWon
        [TestCase(BoardState.EnemyWon, "Right", "Left",
        "0 0 0 0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 0 0 0 " +
        "0 0 X X X X X X X P " +
        "0 0 0 0 0 0 0 0 0 0 " +
        "0 0 E X X X X X 0 0 " +
        "0 0 0 0 0 0 0 X 0 0 " +
        "0 0 0 0 0 0 0 X 0 0 " +
        "0 0 0 0 0 0 0 X 0 0 " +
        "0 0 0 0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 0 0 0 "
        )]
        [TestCase(BoardState.EnemyWon, "Left", "Left",
        "0 0 0 0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 0 0 0 " +
        "0 0 X X X X X X X 0 " +
        "0 0 0 0 0 0 0 0 X 0 " +
        "0 0 E X X X X X P 0 " +
        "0 0 0 0 0 0 0 X 0 0 " +
        "0 0 0 0 0 0 0 X 0 0 " +
        "0 0 0 0 0 0 0 X 0 0 " +
        "0 0 0 0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 0 0 0 "
        )]
        #endregion
        #region Draw
        [TestCase(BoardState.Draw, "Right", "Left",
        "0 0 0 0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 0 0 0 " +
        "0 0 X 0 0 0 0 0 0 0 " +
        "0 0 X 0 0 0 0 0 0 0 " +
        "0 0 X 0 0 0 0 0 0 0 " +
        "0 0 X X X P 0 E X X " +
        "0 0 0 0 0 0 0 0 0 X " +
        "0 0 0 0 0 0 0 X X X " +
        "0 0 0 0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 0 0 0 "
        )]
        [TestCase(BoardState.Draw, "Down", "Right",
        "0 0 0 0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 0 0 0 " +
        "0 0 X 0 0 0 0 0 0 0 " +
        "0 0 X 0 0 0 0 0 0 0 " +
        "0 0 X 0 0 0 0 0 0 0 " +
        "0 0 X 0 0 0 0 0 0 0 " +
        "0 0 X 0 0 0 0 0 0 0 " +
        "0 0 X 0 0 0 0 X 0 0 " +
        "0 0 X 0 0 0 0 X X E " +
        "0 0 P 0 0 0 0 0 0 0 "
        )]
        [TestCase(BoardState.Draw, "Down", "Right",
        "0 0 0 0 0 0 0 0 0 0 " +
        "0 0 0 0 0 0 0 0 0 0 " +
        "0 0 X 0 0 0 0 0 0 0 " +
        "0 0 X 0 0 0 0 0 0 0 " +
        "0 0 X 0 0 0 0 0 0 0 " +
        "0 0 X 0 0 0 0 0 0 0 " +
        "0 0 X 0 0 0 0 0 0 0 " +
        "0 0 X 0 0 0 0 X 0 0 " +
        "0 0 X 0 0 0 0 X X E " +
        "0 0 P 0 0 0 0 0 0 0 "
        )]
        [TestCase(BoardState.Draw, "Down", "Right",
        "0 0 0 0 0 0 0 0 0 E " +
        "0 0 0 0 0 0 0 0 0 X " +
        "0 0 X 0 0 0 0 0 0 X " +
        "0 0 X 0 0 0 0 0 0 X " +
        "0 0 X 0 0 0 0 0 0 X " +
        "0 0 X 0 0 0 0 0 0 X " +
        "0 0 X 0 0 0 0 0 0 X " +
        "0 0 X 0 0 0 0 X 0 X " +
        "0 0 X 0 0 0 0 X X X " +
        "P X X 0 0 0 0 0 0 0 "
        )]
        #endregion
        #region PlayerWon
        [TestCase(BoardState.PlayerWon, "Up", "Up",
        "0 0 0 0 0 0 0 0 0 E " +
        "0 0 0 0 0 0 0 0 0 X " +
        "0 0 X 0 0 0 0 0 0 X " +
        "0 0 X 0 0 0 0 0 0 X " +
        "0 0 X 0 0 0 0 0 0 X " +
        "0 0 X 0 0 0 0 0 0 X " +
        "0 0 X 0 0 0 0 0 0 X " +
        "0 0 X 0 0 0 0 X 0 X " +
        "0 0 X 0 0 0 0 X X X " +
        "P X X 0 0 0 0 0 0 0 "
        )]
        [TestCase(BoardState.PlayerWon, "Up", "Left",
        "0 0 0 0 0 0 0 0 0 X " +
        "0 0 0 0 0 0 0 0 0 X " +
        "0 0 X 0 0 0 0 0 0 X " +
        "0 0 X 0 0 0 0 0 0 X " +
        "0 0 X 0 0 0 0 0 0 X " +
        "0 0 X 0 0 0 0 0 0 X " +
        "0 0 X 0 0 0 0 0 0 X " +
        "0 0 X E X X X X 0 X " +
        "0 0 X 0 0 0 0 X X X " +
        "P X X 0 0 0 0 0 0 0 "
        )]
        #endregion
        public void simulateAndEvaluate(BoardState expected, string playerDir, string enemyDir, string rawBoard) {
            Board b = SnakeTestUtils.generateBoard(rawBoard, playerDir, enemyDir);
            b.playMove(new string[] {playerDir, enemyDir });
            SnakeTestUtils.printBoard(b);

            Assert.AreEqual(expected, b.evaluateBoard());
        }
    }

}