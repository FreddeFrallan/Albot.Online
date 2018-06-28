using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Game;
using System.Linq;

namespace TCP_API.Snake {

    public enum TestEnums { }

    public class SnakeTestUtils{

        #region Generating
        public static Board generateBoard(string rawBoard, string playerDir, string enemyDir) {
            string[] cells = rawBoard.Split(' ');
            int size = (int)Math.Round(Math.Sqrt(cells.Length));
            List<Position2D> blocked = new List<Position2D>();
            SnakePlayer[] players = new SnakePlayer[2];

            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    switch (cells[size * y + x]) {
                        case "X": blocked.Add(new Position2D() {x =  x, y = y }); break;
                        case "P": players[0] = new SnakePlayer() { x = x, y = y, dir = playerDir }; break;
                        case "E": players[1] = new SnakePlayer() { x = x, y = y, dir = enemyDir }; break;
                    }

            bool[,] grid = gengerateBlockedGrid(blocked, size);
            return new Board(grid, players, blocked, false);
        }

        public static bool[,] gengerateBlockedGrid(List<Position2D> blocked, int size) {
            bool[,] grid = new bool[size, size];
            foreach (Position2D p in blocked)
                grid[p.x, p.y] = true;

            return grid;
        }
        #endregion


        #region Comparing
        public static bool compareBoards(Board a, Board b) {
            bool[,] gridA = a.getBlockedGrid(), gridB = b.getBlockedGrid();
            if (gridA.GetLength(0) != gridB.GetLength(0) || gridA.GetLength(1) != gridB.GetLength(1))
                return false;

            for (int y = 0; y < gridA.GetLength(1); y++)
                for (int x = 0; x < gridA.GetLength(0); x++)
                    if (gridA[x, y] != gridB[x, y])
                        return false;

            if (a.evaluateBoard() != b.evaluateBoard())
                return false;

            return true;
        }

        public static bool comparePossibleMoves(List<string> a, List<string> b) {
            if (a.Count != b.Count)
                return false;
            foreach (string s in a)
                if (b.Any(s2 => s2 == s) == false)
                    return false;
            return true;
        }
        #endregion

        #region Debugging & Printing
        public static void printGrid(bool[,] grid) {
            Debug.Log("**************");
            string s = "";
            TestUtils.iterateBoard(grid.GetLength(0), grid.GetLength(1),
                (x, y) => { s += (grid[x, y] ? "X" : "0") + "  "; },
                (y) => { s += "\n"; });
            Debug.Log(s);
        }

        public static void printBoard(Board b) {
            printPlayers(b.getPlayers());
            printGrid(b.getBlockedGrid());
        }
        public static void printPlayers(SnakePlayer[] players) {
            printPlayer(players[0], "Player");
            printPlayer(players[1], "Enemy");
        }
        public static void printPlayer(SnakePlayer p, string name) {
            Debug.Log(name + ": " + p.x + "." + p.y + "  " + p.dir);
        }
        #endregion
    }
}