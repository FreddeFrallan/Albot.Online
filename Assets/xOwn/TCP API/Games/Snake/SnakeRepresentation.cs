using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Game;

namespace TCP_API.Snake {

    public class Constants : APIStandardConstants {
        public const int BOARD_HEIGHT = 10;
        public const int BOARD_WIDTH = 10;
        public const int AMOUNT_PLAYERS = 2;
        public const int POSSIBLE_MOVES = 3;

        public class JProtocol {
            public const string board = "board";
            public const string posX = "x";
            public const string posY = "y";
            public const string dir = "dir";
            public const string player = "player";
            public const string enemy = "enemy";
            public const string blocked = "blocked";
            public const string playerMove = "playerMove";
            public const string enemyMove = "enemyMove";
            public const string playerMoves = "playerMoves";
            public const string enemyMoves = "enemyMoves";
        }

        public class Movement {
            public const string right = "right";
            public const string up = "up";
            public const string left = "left";
            public const string down = "down";

            public static int[] dirToCoords(string dir) {
                switch (dir) {
                    case right: return new int[] { 1, 0 };break;
                    case up: return new int[] { 0, -1 }; break; 
                    case left: return new int[] { -1, 0 }; break; 
                    default: return new int[] { 0, 1}; break;
                }
            }

            /*
            public static string[] getPossibleMovesFromDir(string dir) {
                switch (dir) {
                    case right: return new string[] {up, right, down}; break;
                    case up: return new string[] { left, up, right}; break;
                    case left: return new string[] { down, left, up}; break;
                    default: return new string[] { left, down, right}; break;
                }
            }
            */
            public static List<string> getPossibleMovesFromDir(string dir) {
                switch (dir) {
                    case right: return new List<string> { up, right, down }; break;
                    case up: return new List<string> { left, up, right }; break;
                    case left: return new List<string> { down, left, up }; break;
                    default: return new List<string> { left, down, right }; break;
                }
            }
        }
    }

 
    public class Board {

        //Currently players[0] == Player && players[1] == Enemy
        private SnakePlayer[] players = new SnakePlayer[] {new SnakePlayer(), new SnakePlayer()};
        private List<Position2D> blockedList = new List<Position2D>();
        private bool[,] blockedGrid = new bool[Constants.BOARD_WIDTH, Constants.BOARD_HEIGHT];


        public Board(bool[,] oldBlockedGrid, SnakePlayer[] oldPlayers, List<Position2D> oldBlockedList, bool deepCopy) {
            if (deepCopy) {
                this.blockedGrid = (bool[,])oldBlockedGrid.Clone();
                this.players = (SnakePlayer[])oldPlayers.Clone();
                this.blockedList = oldBlockedList.ToList();
            } else {
                this.blockedGrid = (bool[,])oldBlockedGrid;
                this.players = (SnakePlayer[])oldPlayers;
                this.blockedList = oldBlockedList;
            }
        }
        public Board(JSONObject jObj) {
            parsePlayer(jObj.GetField(Constants.JProtocol.player), ref players[0]);
            parsePlayer(jObj.GetField(Constants.JProtocol.enemy), ref players[1]);
            //if(jObj.HasField(Constants.JProtocol.blocked))
            parseBlocked((jObj.GetField(Constants.JProtocol.blocked).list));
        }

        #region Parsing
        private void parseBlocked(List<JSONObject> blocked) {
            foreach (JSONObject b in blocked) {
                Position2D pos = parsePos(b);
                blockedList.Add(pos);
                blockedGrid[pos.x, pos.y] = true;
            }
        }

        private void parsePlayer(JSONObject jPlayer, ref SnakePlayer p) {
            p.x = (int)jPlayer.GetField(Constants.JProtocol.posX).n;
            p.y = (int)jPlayer.GetField(Constants.JProtocol.posY).n;
            p.dir = jPlayer.GetField(Constants.JProtocol.dir).str;
        }

        private Position2D parsePos(JSONObject jObj) {
            return new Position2D() { x = (int)jObj.GetField(Constants.JProtocol.posX).n, y = (int)jObj.GetField(Constants.JProtocol.posY).n };
        }
        #endregion

        public void playSingleMove(string dir, bool player) {
            int id = player ? 0 : 1;
            applyPlayMove(ref players[id], dir);
        }
        public void playMove(string[] dirs) {
            for (int i = 0; i < dirs.Length; i++)
                applyPlayMove(ref players[i], dirs[i]);
        }
        private void applyPlayMove(ref SnakePlayer p, string dir) {
            int[] coordChange = Constants.Movement.dirToCoords(dir);
            
            blockedGrid[p.x, p.y] = true;
            blockedList.Add(new Position2D() { x = p.x, y = p.y });

            p.x += coordChange[0];
            p.y += coordChange[1];

            p.dir = dir;
        }

        #region Evaluation
        public BoardState evaluateBoard() {
            SnakePlayer p = players[0], e = players[1];
            Debug.Log("Player: " + p.x + "," + p.y + "\nEnemy: " + e.x + "," + e.y);
            if (p.x == e.x && p.y == e.y) // Players Crash into eachother
                return BoardState.Draw;

            bool playerCrash = coordCrash(p.x, p.y);
            bool enemyCrash = coordCrash(e.x, e.y);

            Debug.Log("Playercrash: " + playerCrash.ToString() + ", Enemycrash: " + enemyCrash.ToString());

            if (playerCrash && enemyCrash)
                return BoardState.Draw;
            if (playerCrash && enemyCrash == false)
                return BoardState.EnemyWon;
            if (playerCrash == false && enemyCrash)
                return BoardState.PlayerWon;

            return BoardState.Ongoing;
        }

        private bool coordCrash(int x, int y) {
            if (x < 0 || x >= Constants.BOARD_WIDTH || y < 0 || y >= Constants.BOARD_HEIGHT)
                return true;
            return blockedGrid[x, y];
        }
        #endregion

        public SnakePlayer[] getPlayers() { return players; }
        public Board deepCopy() {return new Board(blockedGrid, players, blockedList, true);}
        public bool[,] getBlockedGrid() { return blockedGrid; }
        public List<Position2D> getBlockedList() { return blockedList; }
    }

    public struct SnakePlayer {
        public int x, y;
        public string dir;
    }
    
}