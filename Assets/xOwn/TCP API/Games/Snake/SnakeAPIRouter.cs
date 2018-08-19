using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.Networking;

namespace TCP_API.Snake {

    public class SnakeAPIRouter : APIMessageRouterBase {


        public SnakeAPIRouter() {
            APIFuncs.Add(Constants.Actions.getPossMoves, handleGetPossibleMoves);
            APIFuncs.Add(Constants.Actions.simMove, handleSimulateMove);
            APIFuncs.Add(Constants.Actions.evalBoard, handleEvaluateBoard);
            APIFuncs.Add(Constants.JProtocol.simMoveDelta, handleSimulateMoveDelta);
        }

        private APIMsgConclusion handleEvaluateBoard(JSONObject jObj) {
            JSONObject jBoard = jObj.GetField(Constants.JProtocol.board);
            Board board = new Board(jBoard);
            string encodedBoardState = SnakeProtocolEncoder.encodeBoardState(board.evaluateBoard().ToString());
            return new APIMsgConclusion() {
                status = ResponseStatus.Success, msg = encodedBoardState,
                target = MsgTarget.Player
            };
        }

        private APIMsgConclusion handleSimulateMove(JSONObject jObj) {
            JSONObject jBoard = jObj.GetField(Constants.JProtocol.board);
            Board startBoard = new Board(jBoard);
            BoardMoves moves = parseBoardMoves(jObj);
            
            SimulatedMove temp = SnakeAPILogic.simulateMove(startBoard, moves.playerMove, moves.enemyMove, false);
            string encodedBoards = SnakeProtocolEncoder.compressSimulatedMove(temp);

            return new APIMsgConclusion() {status = ResponseStatus.Success, msg = encodedBoards, target = MsgTarget.Player };
        }

        private APIMsgConclusion handleSimulateMoveDelta(JSONObject jObj) {
            JSONObject jPlayer = jObj.GetField(Constants.JProtocol.player);
            JSONObject jEnemy = jObj.GetField(Constants.JProtocol.enemy);
            BoardMoves moves = parseBoardMoves(jObj);

            SnakePlayer p = new SnakePlayer() {
                x = (int)jPlayer.GetField(Constants.JProtocol.posX).i,
                y = (int)jPlayer.GetField(Constants.JProtocol.posY).i,
                dir = moves.playerMove
            };
            SnakePlayer e = new SnakePlayer() {
                x = (int)jEnemy.GetField(Constants.JProtocol.posX).i,
                y = (int)jEnemy.GetField(Constants.JProtocol.posY).i,
                dir = moves.enemyMove
            };

            Board b = new Board(new SnakePlayer[] { p, e }, false);
            b.playMove(new string[] { moves.playerMove, moves.enemyMove});
            SimulatedMove temp = new SimulatedMove() {enemyMove = moves.enemyMove, playerMove = moves.playerMove,board = b};

            string encodedBoards = SnakeProtocolEncoder.compressSimulatedMove(temp);
            return new APIMsgConclusion() { status = ResponseStatus.Success, msg = encodedBoards, target = MsgTarget.Player };
        }

        private APIMsgConclusion handleGetPossibleMoves(JSONObject jObj) {
            string[] directions = new string[2];
            directions[0] = jObj.GetField(Constants.JProtocol.player).str;
            directions[1] = jObj.GetField(Constants.JProtocol.enemy).str;
            //PossibleMoves possMoves = SnakeAPILogic.getPossibleMoves(new Board(jObj));
            PossibleMoves possMoves = SnakeAPILogic.getPossibleMoves(directions);

            string responseMsg = SnakeProtocolEncoder.encodePossibleMoves(possMoves).Print();
            return new APIMsgConclusion() { status = ResponseStatus.Success, msg = responseMsg, target = MsgTarget.Player};
        }
        
        private BoardMoves parseBoardMoves(JSONObject jObj) {
            BoardMoves m = new BoardMoves() {
                hasPlayerMove = jObj.HasField(Constants.JProtocol.playerMove),
                hasEnemyMove = jObj.HasField(Constants.JProtocol.enemyMove),
            };

            if (m.hasPlayerMove)
                m.playerMove = jObj.GetField(Constants.JProtocol.playerMove).str;
            
            if (m.hasEnemyMove)
                m.enemyMove = jObj.GetField(Constants.JProtocol.enemyMove).str;
            
            return m;
        }

        protected override bool isJsonMsg(string input) {
            if (input.Length <= 1)
                return false;
            return input.Trim()[0] == '{';
        }


        private struct BoardMoves {
            public bool hasPlayerMove, hasEnemyMove;
            public string playerMove, enemyMove;
        }
    }

}