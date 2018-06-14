using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

namespace TCP_API.Snake {

    public class EncodeBoardTest {

        [TestCase(
         "Right", "Left",
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "X X P 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 E X X " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 "
        )]
        [TestCase(
         "Right", "Left",
         "0 0 0 0 0 0 0 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "X X X X X X X 0 0 0 " +
         "0 0 0 0 0 0 X 0 0 0 " +
         "0 0 0 0 0 0 X 0 0 0 " +
         "0 0 0 0 0 0 P 0 0 0 " +
         "0 0 0 0 0 0 0 0 0 0 " +
         "X X X X X X X X X X " +
         "X 0 0 0 0 0 0 0 0 0 " +
         "X X E 0 0 0 0 0 0 0 "
        )]
        public void encodeDecodeBoard(string playerDir, string enemyDir, string rawBoard) {
            Board startBoard = SnakeTestUtils.generateBoard(rawBoard, playerDir, enemyDir);
            JSONObject jBoard = SnakeProtocolEncoder.compressBoard(startBoard);
            Board decodedBoard = new Board(jBoard);

            Assert.True(SnakeTestUtils.compareBoards(startBoard, decodedBoard));
        }

           // SimulatedMove m = new SimulatedMove() { board = board, playerMove = playerDir, enemyMove = enemyDir };

    }
}