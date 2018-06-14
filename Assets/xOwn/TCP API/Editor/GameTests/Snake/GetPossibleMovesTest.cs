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
            "Right", "Left",
            new string[] {"Right", "Down", "Up"},
            new string[] { "Up", "Down", "Left" },
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
            "Down", "Down",
            new string[] { "Right", "Down", "Left" },
            new string[] { "Right", "Down", "Left" },
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
        public void getPossibleMoves(string playerDir, string enemyDir,string[] expectedPlayer, string[] expectedEnemy, string rawBoard) {
            Board b = SnakeTestUtils.generateBoard(rawBoard, playerDir, enemyDir);
            PossibleMoves m = SnakeAPILogic.getPossibleMoves(b);

            Assert.True(SnakeTestUtils.comparePossibleMoves(m.playerMoves, expectedPlayer));
            Assert.True(SnakeTestUtils.comparePossibleMoves(m.enemyMoves, expectedEnemy));
        }

    }
}