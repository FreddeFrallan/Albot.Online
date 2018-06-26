using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using Barebones.Networking;

namespace TCP_API.Snake {
    public class ProtocolGetPossibleMovesTest{


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
        public void getPossibleMovesTest(string playerDir, string enemyDir, string[] playerExpected, string[] enemyExpected, string rawBoard) {
            SnakeAPIRouter router = new SnakeAPIRouter();
            //Board startBoard = SnakeTestUtils.generateBoard(rawBoard, playerDir, enemyDir);
            JSONObject jMsg = SnakeProtocolEncoder.generateGetPossMovesJMsg(playerDir, enemyDir);

            APIMsgConclusion response = router.handleIncomingMsg(jMsg.Print());
            Assert.True(response.toServer == false);
            Assert.True(response.status == ResponseStatus.Success);

            Debug.Log(response.msg);

            PossibleMoves moves = extractPossibleMoves(new JSONObject(response.msg));
            Assert.True(SnakeTestUtils.comparePossibleMoves(playerExpected, moves.playerMoves));
            Assert.True(SnakeTestUtils.comparePossibleMoves(enemyExpected, moves.enemyMoves));
        }

        private PossibleMoves extractPossibleMoves(JSONObject jObj) {
            PossibleMoves temp = new PossibleMoves();
            temp.playerMoves = convertJObjToStringArray(jObj.GetField(Constants.JProtocol.playerMoves));
            temp.enemyMoves = convertJObjToStringArray(jObj.GetField(Constants.JProtocol.enemyMoves));
            return temp;
        }

        private string[] convertJObjToStringArray(JSONObject jObj) {
            List<string> temp = new List<string>();
            foreach (JSONObject j in jObj.list)
                temp.Add(j.str);
            return temp.ToArray();
        }
    }
}