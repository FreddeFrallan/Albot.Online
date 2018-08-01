using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using System.Linq;

namespace Snake {

    public class SnakeGameLogProtocol : GameLogProtocol {

        #region Encoding
        public static string generateState(Position2D[] playerPos, int[] dir) {
            return string.Format("{0} {1} {2} {3} {4} {5}",
                (int)playerPos[0].x, (int)playerPos[0].y, dir[0],
                 (int)playerPos[1].x, (int)playerPos[1].y, dir[1]);
        }

        public static string generateCrash(bool p1, bool p2, Vector2[] crashPos) {
            string crashMsg = "Crash ";

            if (p1)
                crashMsg += "B " + getCrashPosInString(crashPos[0]) + " ";
            if(p2)
                crashMsg += "R " + getCrashPosInString(crashPos[1]) + " ";

            return crashMsg;
        }

        private static string getCrashPosInString(Vector2 crash) { return (int)crash.x + " " + (int)crash.y; }
        #endregion


        #region Decoding
        public static BoardUpdate parseGameState(string state) {
            if (state.Contains("Crash"))
                return parseCrashMsg(state.Split(' '));
            return parseGameUpdate(state.Split(' '));
        }

        private static BoardUpdate parseCrashMsg(string[] words) {
            bool[] playerCrashes = new bool[] { false, false };
            Position2D[] crashPos = new Position2D[] { new Position2D(), new Position2D() };

            for(int i = 1; i + 2 < words.Length; i+= 3) {
                int playerIndex = words[i] == "B" ? 0 : 1;
                playerCrashes[playerIndex] = true;
                crashPos[playerIndex] = parsePos(words[i + 1], words[i + 2]);
            }

            PlayerColor winColor;
            if (playerCrashes[0] && playerCrashes[1])
                winColor = PlayerColor.None;
            else if (playerCrashes[0])
                winColor = PlayerColor.Red;
            else
                winColor = PlayerColor.Blue;

            return new BoardUpdate() {
                gameOver = true,
                blueCoords = new Position2D[] { crashPos[0] },
                redCoords = new Position2D[] { crashPos[1] },
                winnerColor = winColor,
            };
        }


        private static BoardUpdate parseGameUpdate(string[] words) {
            return new BoardUpdate() {
                gameOver = false,
                blueCoords = new Position2D[] { parsePos(words[0], words[1]) },
                blueDir = int.Parse(words[2]),

                redCoords = new Position2D[] { parsePos(words[3], words[4]) },
                redDir = int.Parse(words[5]),
            };
        }

        private static Position2D parsePos(string s1, string s2) {
            return new Position2D() { x = int.Parse(s1), y = int.Parse(s2) };
        }
        #endregion
    }



}
