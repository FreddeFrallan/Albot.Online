using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TCP_API {

    public class TestUtils{

        public static void iterateBoard(int xSize, int ySize, Action<int, int> a) {
            for (int y = 0; y < ySize; y++)
                for (int x = 0; x < xSize; x++)
                    a(x, y);
        }

        public static void iterateBoard(int xSize, int ySize, Action<int, int> a, Action<int> b) {
            for (int y = 0; y < ySize; y++) {
                for (int x = 0; x < xSize; x++)
                    a(x, y);
                b(y);
            }
        }

    }
}