using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TCP_API.Snake;
using System;

namespace TCP_API {

    public class SnakeGetPossibleMovesTest {

        [TestCase(
            "right", "left",
            new string[] {"right", "down", "up"},
            new string[] { "up", "down", "left" },
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
        [TestCase(
            "down", "down",
            new string[] { "right", "down", "left" },
            new string[] { "right", "down", "left" },
            "0 0 0 0 0 0 0 0 0 0 " +
            "X X X 0 0 0 0 0 0 0 " +
            "0 0 P 0 0 0 0 0 0 0 " +
            "0 0 0 0 0 0 0 0 0 0 " +
            "0 0 0 0 0 0 0 0 0 0 " +
            "0 0 0 0 0 0 0 0 0 0 " +
            "0 0 0 0 0 X X X X 0 " +
            "0 0 0 0 0 0 0 E X 0 " +
            "0 0 0 0 0 0 0 0 0 0 " +
            "0 0 0 0 0 0 0 0 0 0 "
            )]
        public void getPossibleMoves(string playerDir, string enemyDir, string[] playerExpected, string[] enemyExpected, string rawBoard) {
            List<string> playerExpectedList = playerExpected.ToList();
            List<string> enemyExpectedList = enemyExpected.ToList();
            Board b = SnakeTestUtils.generateBoard(rawBoard, playerDir, enemyDir);
            PossibleMoves m = SnakeAPILogic.getPossibleMoves(b);

            Assert.True(SnakeTestUtils.comparePossibleMoves(m.playerMoves, playerExpectedList));
            Assert.True(SnakeTestUtils.comparePossibleMoves(m.enemyMoves, enemyExpectedList));
        }

    }
}