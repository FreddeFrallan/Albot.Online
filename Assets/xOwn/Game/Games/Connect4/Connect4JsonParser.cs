using System.Collections;
using System.Collections.Generic;
using TCP_API.Connect4;
using UnityEngine;

namespace Connect4 {

    public class Connect4JsonParser : MonoBehaviour {

        
        public static string formatBoardMsgFromServer(string rawBoard, Game.PlayerColor color) {
            rawBoard = replaceServerFormat(rawBoard, color);
            JSONObject jMsg = new JSONObject();
            jMsg.AddField(Consts.Fields.board, createJGrid(rawBoard));

            return jMsg.Print();
        }
        

        public static JSONObject createJGrid(string rawBoard) {
            string[] words = rawBoard.Split(' ');
            JSONObject jGrid = new JSONObject();
            for (int y = 0; y < Consts.BOARD_HEIGHT; y++) {
                JSONObject row = new JSONObject();
                for(int x = 0; x < Consts.BOARD_WIDTH; x++)
                    row.Add(int.Parse(words[y * Consts.BOARD_WIDTH + x]));

                jGrid.Add(row);
            }
            return jGrid;
        }

        // In my old variation the server only sent the board as 0 = empty, 1 = yellow, 2 = red
        // So here i do a conversion to the local player to get  0 = empty, 1 = you, -1 = enemy
        private static string replaceServerFormat(string rawBoard, Game.PlayerColor color) {
            string tempBoard = "";

            if (color == Game.PlayerColor.Red)
                tempBoard = rawBoard.Replace("2", "-1");
            else {
                tempBoard = rawBoard.Replace("1", "-1");
                tempBoard = tempBoard.Replace("2", "1");
            }
            return tempBoard;
        }


    }
}