using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using Barebones.Networking;

namespace TCP_API.Snake {

    public class ProtocolSimulateBothPlayerMoveTest {
        [TestCase(
             "Left", "Up",
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
        [TestCase(
             "Down", "Right",
             "0 0 0 0 0 0 0 0 0 0 " +
             "X X X X X X 0 0 0 0 " +
             "X 0 0 0 0 X 0 E 0 0 " +
             "X 0 0 0 0 X 0 X 0 0 " +
             "X X X X 0 P 0 X 0 0 " +
             "0 0 0 X 0 0 0 X 0 0 " +
             "0 0 0 X 0 0 0 X 0 0 " +
             "0 0 0 X 0 0 0 X X 0 " +
             "0 X X X X X X X X 0 " +
             "0 X X X X X X X X 0 ",

             "0 0 0 0 0 0 0 0 0 0 " +
             "X X X X X X 0 0 0 0 " +
             "X 0 0 0 0 X 0 X E 0 " +
             "X 0 0 0 0 X 0 X 0 0 " +
             "X X X X 0 X 0 X 0 0 " +
             "0 0 0 X 0 P 0 X 0 0 " +
             "0 0 0 X 0 0 0 X 0 0 " +
             "0 0 0 X 0 0 0 X X 0 " +
             "0 X X X X X X X X 0 " +
             "0 X X X X X X X X 0 "
             )]
        public void protocolSimulateBothPlayerMove(string playerDir, string enemyDir, string rawBoard, string rawExpected) {
            SnakeAPIRouter router = new SnakeAPIRouter();
            Board startBoard = SnakeTestUtils.generateBoard(rawBoard, playerDir, enemyDir);
            Board expected = SnakeTestUtils.generateBoard(rawExpected, playerDir, enemyDir);
            JSONObject jObj = SnakeProtocolEncoder.generateSimulateJMsg(startBoard, playerDir, enemyDir);
            Debug.Log("Generated simulate move request: \n" + jObj.Print(true) + "\n");
            APIMsgConclusion response = router.handleIncomingMsg(jObj.Print());
            Assert.True(response.toServer == false);
            Assert.True(response.status == ResponseStatus.Success);
            JSONObject newJBoard = new JSONObject(response.msg);
            Debug.Log("Generated simulate move response: \n" + newJBoard.Print(true) + "\n");
            //JSONObject newJBoard = jResponse.GetField(Constants.JProtocol.board);
            Board newBoard = new Board(newJBoard);

            Assert.True(SnakeTestUtils.compareBoards(expected, newBoard));
        }


    }
}