﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCP_API.Snake {

    public class SnakeAPILogic {


        public static List<SimulatedMove[]> simulateAllPossibleMoves(Board board) {
            List<SimulatedMove[]> possibleStates = new List<SimulatedMove[]>();
            SnakePlayer[] players = board.getPlayers();
            PossibleMoves moves = getPossibleMoves(board);

            foreach (string playerDir in moves.playerMoves) //Iterate over player dirs
                possibleStates.Add(simulateAllEnemyMoves(board, playerDir, moves.enemyMoves));

            return possibleStates;
        }

        /// <summary>
        /// Currently hardoceded to only work for 2 players, this allowes for some marginal optimization
        /// </summary>
        /// <param name="board"></param>
        /// <param name="dir"></param>
        /// <returns>Board[]</returns>
        public static SimulatedMove[] simulateAllEnemyMoves(Board board, SnakePlayer[] players, string dir) {
            return simulateAllEnemyMoves(board, dir, Constants.Movement.getPossibleMovesFromDir(players[1].dir));
        }
        public static SimulatedMove[] simulateAllEnemyMoves(Board board, string playerDir, List<string> enemyDirs) {
            SimulatedMove[] newBoards = new SimulatedMove[(Constants.AMOUNT_PLAYERS - 1) * Constants.POSSIBLE_MOVES];
            for (int i = 0; i < newBoards.Length; i++)
                newBoards[i] = simulateMove(board, new string[] { playerDir, enemyDirs[i] });

            return newBoards;
        }

        public static SimulatedMove simulateSingleMove(Board board, string dir, bool player, bool doDeepCopy = true) {
            Board newBoard = doDeepCopy ? board.deepCopy() : board;
            newBoard.playSingleMove(dir, player);
            return new SimulatedMove() { board = newBoard };
        }
        public static SimulatedMove simulateMove(Board board, string[] dirs, bool doDeepCopy = true) {
            Board newBoard = doDeepCopy ? board.deepCopy() : board;
            newBoard.playMove(dirs);
            return new SimulatedMove() {board = newBoard, playerMove = dirs[0], enemyMove = dirs[1]};
        }
        public static SimulatedMove simulateMove(Board board, string playerDir, string enemyDir, bool doDeepCopy = true) {
            return simulateMove(board, new string[] { playerDir, enemyDir }, doDeepCopy);
        }
        
        public static PossibleMoves getPossibleMoves(Board board) {
            SnakePlayer[] players = board.getPlayers();
            //string[][] moves = new string[players.Length][];
            List<string>[] moves = new List<string>[players.Length];
            for (int i = 0; i < players.Length; i++)
                moves[i] = Constants.Movement.getPossibleMovesFromDir(players[i].dir);

            return new PossibleMoves() { playerMoves = moves[0], enemyMoves = moves[1] };
        }
        
        public static PossibleMoves getPossibleMoves(string[] directions) {
            List<string>[] moves = new List<string>[directions.Length];
            for (int i = 0; i < directions.Length; i++)
                moves[i] = Constants.Movement.getPossibleMovesFromDir(directions[i]);

            return new PossibleMoves() { playerMoves = moves[0], enemyMoves = moves[1] };
        }

    }

    //public struct PossibleMoves {public string[] playerMoves, enemyMoves;}
    public struct PossibleMoves { public List<string> playerMoves, enemyMoves; }

    public struct SimulatedMove {
        public Board board;
        public string playerMove, enemyMove;
    }
}