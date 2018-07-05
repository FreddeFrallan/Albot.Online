using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

namespace TCP_API.Snake {

    public class SimulateMoveTest {

        #region simulateBothMoves
        [TestCase(
         "right", "left",
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 P 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 E 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 ",

         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 X P 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 E X 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 "
         )]
        [TestCase(
         "down", "right",
         "0 0 X X X X X X 0 0 " +
         "0 0 X 0 0 0 0 X 0 0 " +
         "0 0 X 0 0 0 X X 0 0 " +
         "0 0 0 0 0 0 x 0 0 0 " +
         "0 0 X E 0 0 X 0 0 0 " +
         "0 0 X 0 0 0 P 0 0 0 " +
         "0 0 X 0 0 0 0 0 0 0 " +
         "0 0 X X X X X X 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 ",

         "0 0 X X X X X X 0 0 " +
         "0 0 X 0 0 0 0 X 0 0 " +
         "0 0 X 0 0 0 X X 0 0 " +
         "0 0 0 0 0 0 x 0 0 0 " +
         "0 0 X X E 0 X 0 0 0 " +
         "0 0 X 0 0 0 X 0 0 0 " +
         "0 0 X 0 0 0 P 0 0 0 " +
         "0 0 X X X X X X 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 "
         )]
        [TestCase(
         "left", "up",
         "0 0 0 0 0 P 0 0 0 0 " +
         "X X X X X X 0 0 0 0 " +
         "X 0 0 0 0 0 0 0 0 0 " +
         "X 0 0 0 0 0 0 0 0 0 " +
         "X X X X 0 0 0 0 0 0 " +
         "0 0 0 X 0 0 0 E 0 0 " +
         "0 0 0 X 0 0 0 X 0 0 " +
         "0 0 0 X 0 0 0 X X 0 " +
         "0 X X X X X X X X 0 " +
         "0 X X X X X X X X 0 ",

         "0 0 0 0 P X 0 0 0 0 " +
         "X X X X X X 0 0 0 0 " +
         "X 0 0 0 0 0 0 0 0 0 " +
         "X 0 0 0 0 0 0 0 0 0 " +
         "X X X X 0 0 0 E 0 0 " +
         "0 0 0 X 0 0 0 X 0 0 " +
         "0 0 0 X 0 0 0 X 0 0 " +
         "0 0 0 X 0 0 0 X X 0 " +
         "0 X X X X X X X X 0 " +
         "0 X X X X X X X X 0 "
         )]
        public void simulateBothMoves(string playerDir, string enemyDir, string rawBoard, string rawExpected) {
            simulateMoveTest(playerDir, enemyDir, rawBoard, rawExpected);
        }
        #endregion

        #region simulatePlayerMove
        [TestCase(
         "right",
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 P 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 E 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 ",

         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 X P 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 E 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 "
         )]
        [TestCase(
         "down",
         "0 0 X X X X X X 0 0 " +
         "0 0 X 0 0 0 0 X 0 0 " +
         "0 0 X 0 0 0 X X 0 0 " +
         "0 0 0 0 0 0 x 0 0 0 " +
         "0 0 X E 0 0 X 0 0 0 " +
         "0 0 X 0 0 0 P 0 0 0 " +
         "0 0 X 0 0 0 0 0 0 0 " +
         "0 0 X X X X X X 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 ",

         "0 0 X X X X X X 0 0 " +
         "0 0 X 0 0 0 0 X 0 0 " +
         "0 0 X 0 0 0 X X 0 0 " +
         "0 0 0 0 0 0 x 0 0 0 " +
         "0 0 X E 0 0 X 0 0 0 " +
         "0 0 X 0 0 0 X 0 0 0 " +
         "0 0 X 0 0 0 P 0 0 0 " +
         "0 0 X X X X X X 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 "
         )]
        [TestCase(
         "left",
         "0 0 0 0 0 P 0 0 0 0 " +
         "X X X X X X 0 0 0 0 " +
         "X 0 0 0 0 0 0 0 0 0 " +
         "X 0 0 0 0 0 0 0 0 0 " +
         "X X X X 0 0 0 0 0 0 " +
         "0 0 0 X 0 0 0 E 0 0 " +
         "0 0 0 X 0 0 0 X 0 0 " +
         "0 0 0 X 0 0 0 X X 0 " +
         "0 X X X X X X X X 0 " +
         "0 X X X X X X X X 0 ",

         "0 0 0 0 P X 0 0 0 0 " +
         "X X X X X X 0 0 0 0 " +
         "X 0 0 0 0 0 0 0 0 0 " +
         "X 0 0 0 0 0 0 0 0 0 " +
         "X X X X 0 0 0 0 0 0 " +
         "0 0 0 X 0 0 0 E 0 0 " +
         "0 0 0 X 0 0 0 X 0 0 " +
         "0 0 0 X 0 0 0 X X 0 " +
         "0 X X X X X X X X 0 " +
         "0 X X X X X X X X 0 "
         )]
        public void simulatePlayerMove(string playerDir, string rawBoard, string rawExpected) {
            simulateMoveTest(playerDir, "", rawBoard, rawExpected); // Empty string cause not needed
        }
        #endregion

        #region simulateEnemyMove
        [TestCase(
         "left",
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 P 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 E 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 ",

         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 P 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 E X 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 "
         )]
        [TestCase(
         "right",
         "0 0 X X X X X X 0 0 " +
         "0 0 X 0 0 0 0 X 0 0 " +
         "0 0 X 0 0 0 X X 0 0 " +
         "0 0 0 0 0 0 x 0 0 0 " +
         "0 0 X E 0 0 X 0 0 0 " +
         "0 0 X 0 0 0 P 0 0 0 " +
         "0 0 X 0 0 0 0 0 0 0 " +
         "0 0 X X X X X X 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 ",

         "0 0 X X X X X X 0 0 " +
         "0 0 X 0 0 0 0 X 0 0 " +
         "0 0 X 0 0 0 X X 0 0 " +
         "0 0 0 0 0 0 x 0 0 0 " +
         "0 0 X X E 0 X 0 0 0 " +
         "0 0 X 0 0 0 P 0 0 0 " +
         "0 0 X 0 0 0 0 0 0 0 " +
         "0 0 X X X X X X 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 "
         )]
        [TestCase(
         "up",
         "0 0 0 0 0 P 0 0 0 0 " +
         "X X X X X X 0 0 0 0 " +
         "X 0 0 0 0 0 0 0 0 0 " +
         "X 0 0 0 0 0 0 0 0 0 " +
         "X X X X 0 0 0 0 0 0 " +
         "0 0 0 X 0 0 0 E 0 0 " +
         "0 0 0 X 0 0 0 X 0 0 " +
         "0 0 0 X 0 0 0 X X 0 " +
         "0 X X X X X X X X 0 " +
         "0 X X X X X X X X 0 ",

         "0 0 0 0 0 P 0 0 0 0 " +
         "X X X X X X 0 0 0 0 " +
         "X 0 0 0 0 0 0 0 0 0 " +
         "X 0 0 0 0 0 0 0 0 0 " +
         "X X X X 0 0 0 E 0 0 " +
         "0 0 0 X 0 0 0 X 0 0 " +
         "0 0 0 X 0 0 0 X 0 0 " +
         "0 0 0 X 0 0 0 X X 0 " +
         "0 X X X X X X X X 0 " +
         "0 X X X X X X X X 0 "
         )]
        public void simulateEnemyMove(string enemyDir, string rawBoard, string rawExpected) {
            simulateMoveTest("", enemyDir, rawBoard, rawExpected);
        }
        #endregion


        private static void simulateMoveTest(string playerDir, string enemyDir, string rawBoard, string rawExpected) {
            Board expected = SnakeTestUtils.generateBoard(rawExpected, playerDir, enemyDir); 
            Board newBoard = SnakeTestUtils.generateBoard(rawBoard, playerDir, enemyDir);

            if (playerDir.Equals(""))
                newBoard.playSingleMove(enemyDir, false);
            else if (enemyDir.Equals(""))
                newBoard.playSingleMove(playerDir, true);
            else
                newBoard.playMove(new string[] { playerDir, enemyDir});
            
            SnakeTestUtils.printGrid(expected.getBlockedGrid());
            SnakeTestUtils.printGrid(newBoard.getBlockedGrid());

            Assert.True(SnakeTestUtils.compareBoards(newBoard, expected));
        }

    }

}