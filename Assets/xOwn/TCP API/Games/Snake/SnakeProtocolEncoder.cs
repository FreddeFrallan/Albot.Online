using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;


namespace TCP_API.Snake {

    public class SnakeProtocolEncoder {


        #region SimulatedMove
        public static string compressSimulatedMove(List<SimulatedMove[]> moves) {
            List<JSONObject> jMoves = new List<JSONObject>();
            foreach (SimulatedMove[] pm in moves)
                foreach (SimulatedMove m in pm)
                    jMoves.Add(generateJMove(m));

            return new JSONObject(jMoves.ToArray()).Print();
        }
        public static string compressSimulatedMove(SimulatedMove[] moves) {
            List<JSONObject> jMoves = new List<JSONObject>();
            foreach (SimulatedMove m in moves)
                jMoves.Add(generateJMove(m));

            return new JSONObject(jMoves.ToArray()).Print();
        }
        public static string compressSimulatedMove(SimulatedMove move) { return generateJMove(move).Print(); }

        public static JSONObject generateJMove(SimulatedMove move) {
            JSONObject jObj = compressBoard(move.board);
            //Debug.Log("SimMove response obj: \n" + jObj.Print(true));
            //jObj.AddField(Constants.JProtocol.board, compressBoard(move.board));
            //jObj.AddField(Constants.JProtocol.playerMove, move.playerMove);
            //jObj.AddField(Constants.JProtocol.enemyMove, move.enemyMove);
            return jObj;
        }
        #endregion

        #region Board
        public static JSONObject compressBoard(Board b) {return compressBoard(b.getBlockedList(), b.getPlayers());}
        public static JSONObject compressBoard(List<Position2D> blockedList, SnakePlayer[] players) {
            JSONObject jBoard = new JSONObject();

            addPlayerField(ref jBoard, players[0], Constants.JProtocol.player);
            addPlayerField(ref jBoard, players[1], Constants.JProtocol.enemy);
            addBlockedField(ref jBoard, blockedList);

            return jBoard;
        }
        #endregion

        #region Util
        public static void addStateField(ref JSONObject jObj, BoardState state) {
            jObj.AddField(Constants.Fields.boardState, state.ToString());
        }

        public static void addPlayerField(ref JSONObject jObj, SnakePlayer p, string name) {
            JSONObject player = new JSONObject();
            player.AddField(Constants.JProtocol.dir, p.dir);
            player.AddField(Constants.JProtocol.posX, p.x);
            player.AddField(Constants.JProtocol.posY, p.y);
            jObj.AddField(name, player);
        }

        public static void addBlockedField(ref JSONObject jObj, List<Position2D> blockedList, string fieldName = Constants.JProtocol.blocked) {
            List<JSONObject> jBlocked = new List<JSONObject>();
            foreach (Position2D b in blockedList) {
                JSONObject temp = new JSONObject();
                temp.AddField(Constants.JProtocol.posX, b.x);
                temp.AddField(Constants.JProtocol.posY, b.y);
                jBlocked.Add(temp);
            }

            jObj.AddField(fieldName, new JSONObject(jBlocked.ToArray()));
        }

        public static JSONObject generateJBoard(Board b, string playerDir, string enemyDir) {
            JSONObject jObj = new JSONObject();
            addPlayerField(ref jObj, b.getPlayers()[0], Constants.JProtocol.player);
            addPlayerField(ref jObj, b.getPlayers()[1], Constants.JProtocol.enemy);
            addBlockedField(ref jObj, b.getBlockedList());
            return jObj;
        }
        #endregion

        #region EvaluateBoard
        public static string encodeBoardState(string boardState) {
            JSONObject jObj = new JSONObject();
            jObj.AddField(Constants.Fields.boardState, boardState);
            return jObj.Print();
        }
        #endregion

        #region SimulateMove
        public static JSONObject generateSimulateJMsg(Board b, string playerDir, string enemyDir) {
            JSONObject jObj = new JSONObject();
            jObj.AddField(Constants.JProtocol.board, generateJBoard(b, playerDir, enemyDir));
            jObj.AddField(Constants.Fields.action, Constants.Actions.simMove);
            jObj.AddField(Constants.JProtocol.playerMove, playerDir);
            jObj.AddField(Constants.JProtocol.enemyMove, enemyDir);

            return jObj;
        }
        #endregion


        #region PossibleMoves
        public static JSONObject encodePossibleMoves(PossibleMoves possMoves) {
            JSONObject jObj = new JSONObject();
            addPossibleMovesJObj(ref jObj, possMoves.playerMoves, Constants.JProtocol.playerMoves);
            addPossibleMovesJObj(ref jObj, possMoves.enemyMoves, Constants.JProtocol.enemyMoves);
            return jObj;
        }

        private static void addPossibleMovesJObj(ref JSONObject jObj, List<string> dirs, string fieldName) {
            JSONObject[] moves = new JSONObject[dirs.Count];
            for (int i = 0; i < moves.Length; i++) { 
                moves[i] = new JSONObject();
                moves[i].str = dirs[i];
                moves[i].type = JSONObject.Type.STRING;
            }
            jObj.AddField(fieldName, new JSONObject(moves));
        }
        public static JSONObject generateGetPossMovesJMsg(string playerDir, string enemyDir) {
            JSONObject jObj = new JSONObject();//generateJBoard(b, playerDir, enemyDir);
            jObj.AddField(Constants.Fields.action, Constants.Actions.getPossMoves);
            jObj.AddField(Constants.JProtocol.player, playerDir);
            jObj.AddField(Constants.JProtocol.enemy, enemyDir);
            return jObj;
        }
        #endregion
    }

}