using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TCP_API;
using TCP_API.Connect4;
using Connect4;
using UnityEngine;

namespace TCP_API.Connect4 {

    public class Connect4FormatBoard {

        [TestCase(
             new int[7] { 0, 0, 0, 0, 0, 0, 0 },
             new int[7] { 0, 0, 0, 0, 0, 0, 0 },
             new int[7] { 0, 0, 0, 0, 0, 0, 1 },
             new int[7] { 0, 0, 0, 0, 0, 1, 0 },
             new int[7] { 0, 0, 0, 0, 1, 0, 0 },
             new int[7] { 0, 0, 0, 1, 0, 0, 0 },
        "[[0,0,0,0,0,0,0]," +
        "[0,0,0,0,0,0,0]," +
        "[0,0,0,0,0,0,1]," +
        "[0,0,0,0,0,1,0]," +
        "[0,0,0,0,1,0,0]," +
        "[0,0,0,1,0,0,0]]"
        )]
        [TestCase(
             new int[7] { 0, 0, 0, 0, 0, 0, 0 },
             new int[7] { 0, 0, 0, 0, 0, 0, 0 },
             new int[7] { 0, 0, -1, 0, 0, 0, 1 },
             new int[7] { 0, 0, -1, 0, 0, 1, 0 },
             new int[7] { 0, 0, -1, 0, 1, 0, 0 },
             new int[7] { 0, 0, -1, 1, 0, 0, 0 },
        "[[0,0,0,0,0,0,0]," +
        "[0,0,0,0,0,0,0]," +
        "[0,0,-1,0,0,0,1]," +
        "[0,0,-1,0,0,1,0]," +
        "[0,0,-1,0,1,0,0]," +
        "[0,0,-1,1,0,0,0]]"
        )]
        public void evaluateParsing(int[] r1, int[] r2, int[] r3, int[] r4, int[] r5, int[] r6, string rawBoard) {
            //string test = Connect4JsonParser.formatBoardMsgFromServer(rawBoard, Game.PlayerColor.Red);

            JSONObject jBoard = new JSONObject(rawBoard);
            List<int[]> grid = generateGrid(r1, r2, r3, r4, r5, r6);
            Debug.Log("Json grid: " + jBoard.ToString(true));

            for (int y = 0; y < Consts.BOARD_HEIGHT; y++) {
                //JSONObject row = jBoard.GetField(Consts.Fields.board).list[y];
                JSONObject row = jBoard.list[y];
                for (int x = 0; x < Consts.BOARD_WIDTH; x++) {
                    Assert.AreEqual(row.list[x].i, grid[y][x]);
                }
            }
        }

        private List<int[]> generateGrid(int[] r1, int[] r2, int[] r3, int[] r4, int[] r5, int[] r6) {
            List<int[]> temp = new List<int[]>();
            temp.Add(r1);
            temp.Add(r2);
            temp.Add(r3);
            temp.Add(r4);
            temp.Add(r5);
            temp.Add(r6);
            return temp;
        }
    }

}