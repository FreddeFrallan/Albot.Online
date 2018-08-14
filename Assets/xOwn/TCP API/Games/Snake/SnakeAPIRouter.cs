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
            string encodedBoards;

            SimulatedMove temp;
            if (moves.hasPlayerMove == false) {
                //List<SimulatedMove[]> temp = SnakeAPILogic.simulateAllPossibleMoves(startBoard);
                temp = SnakeAPILogic.simulateSingleMove(startBoard, moves.enemyMove, false, false);
            }
            else if (moves.hasEnemyMove == false) {
                //SimulatedMove[] temp = SnakeAPILogic.simulateAllEnemyMoves(startBoard, startBoard.getPlayers(), moves.playerMove);
                temp = SnakeAPILogic.simulateSingleMove(startBoard, moves.playerMove, true, false);
            } 
            else {
                temp = SnakeAPILogic.simulateMove(startBoard, moves.playerMove, moves.enemyMove, false);
            }
            encodedBoards = SnakeProtocolEncoder.compressSimulatedMove(temp);

            return new APIMsgConclusion() {status = ResponseStatus.Success, msg = encodedBoards, target = MsgTarget.Player };
        }

        private APIMsgConclusion handleGetPossibleMoves(JSONObject jObj) {
            string[] directions = new string[2];
            directions[0] = jObj.GetField(Constants.JProtocol.player).str;
            directions[1] = jObj.GetField(Constants.JProtocol.enemy).str;
            Debug.Log("Player dir:" + directions[0]);
            Debug.Log("Enemy dir:" + directions[1]);
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