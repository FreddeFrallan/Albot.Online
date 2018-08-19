using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Game {

    [Serializable]
    public struct Position2D { public int x, y; }
    [Serializable]
    public struct Position3D { public int x, y, z; }

    public class GameUtils {

        public static List<int[]> pos2DToIntList(List<Position2D> pList) {
            List<int[]> temp = new List<int[]>();
            pList.ForEach(p => temp.Add(pos2DToInt(p)));
            return temp;
        }
        public static bool compareVec3(Vector3 a, Vector3 b) { return a.x == b.x && a.y == b.y && a.z == b.z; }
        public static Vector2 pos2DToVec2(Position2D p) { return new Vector2(p.x, p.y); }
        public static int[] pos2DToInt(Position2D p) {return new int[] { p.x, p.y };}
        public static bool comparePos(Position2D a, Position2D b) { return a.x == b.x && a.y == b.y; }
        public static void printPos(Position2D p) { Debug.Log(p.x + "." + p.y); }

        public static PlayerColor stringToColor(string s) {
            switch (s) {
            case "Red": return PlayerColor.Red;
            case "Green": return PlayerColor.Green;
            case "Blue": return PlayerColor.Blue;
            case "Yellow": return PlayerColor.Yellow;
            case "Black": return PlayerColor.Black;
            case "White": return PlayerColor.White;
            default: return PlayerColor.None;
            }
        }


        public static GameType stringToGameType(string t) {
            switch (t.ToUpper()) {

            case "CONNECT4": return GameType.Connect4;
            case "OTHELLO": return GameType.Othello;
            case "CHESS": return GameType.Chess;
            case "FALLINGDEBRIS": return GameType.FallingDebris;
            case "BATTLESHIP": return GameType.Battleship;
            case "SOLDIERS": return GameType.Soldiers;
            case "BRICKPUZZLE": return GameType.BrickPuzzle;
            case "GAME2048": return GameType.Game2048;
            case "TOWEROFHANOI": return GameType.TowerOfHanoi;
            case "BOMBERMAN": return GameType.Bomberman;
            case "BREAKTHROUGH": return GameType.Breakthrough;
            case "SNAKE": return GameType.Snake;
            case "BLOCKBATTLE": return GameType.BlockBattle;
            case "SPEEDRUNNER": return GameType.SpeedRunner;

            default: return GameType.None;
            }
        }
    }

}