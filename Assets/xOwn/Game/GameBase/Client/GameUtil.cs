using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game {
    public class GameUtil {
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