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
        public static int[] pos2DToInt(Position2D p) {return new int[] { p.x, p.y };}
        public static bool comparePos(Position2D a, Position2D b) { return a.x == b.x && a.y == b.y; }
        public static void printPos(Position2D p) { Debug.Log(p.x + "." + p.y); }
    }

}