using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using Barebones.Networking;

namespace TCP_API.Snake {
    public class ProtocolGetPossibleMovesTest{


        [TestCase(
        "down", "down",
        new string[] { "right", "down", "left" },
        new string[] { "right", "down", "left" }
        )]
        public void getPossibleMovesTest(string playerDir, string enemyDir, string[] playerExpected, string[] enemyExpected) {
            List<string> playerExpectedList = playerExpected.ToList();
            List<string> enemyExpectedList = enemyExpected.ToList();
            SnakeAPIRouter router = new SnakeAPIRouter();
            //Board startBoard = SnakeTestUtils.generateBoard(rawBoard, playerDir, enemyDir);
            JSONObject jMsg = SnakeProtocolEncoder.generateGetPossMovesJMsg(playerDir, enemyDir);

            APIMsgConclusion response = router.handleIncomingMsg(jMsg.Print());
            Assert.True(response.target == MsgTarget.Player);
            Assert.True(response.status == ResponseStatus.Success);

            Debug.Log(response.msg);

            PossibleMoves moves = extractPossibleMoves(new JSONObject(response.msg));
            Assert.True(SnakeTestUtils.comparePossibleMoves(playerExpectedList, moves.playerMoves));
            Assert.True(SnakeTestUtils.comparePossibleMoves(enemyExpectedList, moves.enemyMoves));

            Debug.Log("True....");
        }

        private PossibleMoves extractPossibleMoves(JSONObject jObj) {
            PossibleMoves temp = new PossibleMoves();
            temp.playerMoves = convertJObjToStringArray(jObj.GetField(Constants.JProtocol.playerMoves));
            temp.enemyMoves = convertJObjToStringArray(jObj.GetField(Constants.JProtocol.enemyMoves));
            return temp;
        }

        private List<string> convertJObjToStringArray(JSONObject jObj) {
            List<string> temp = new List<string>();
            foreach (JSONObject j in jObj.list)
                temp.Add(j.str);
            return temp;//temp.ToArray();
        }
    }
}