using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCP_API.Connect4 {

    public class Connect4APIRouter : APIMessageRouterBase {

        /// <summary>
        /// Initializes the action handlers
        /// </summary>
        public Connect4APIRouter() {
            APIFuncs.Add(Consts.Actions.simMove, handleSimulateMove);
            APIFuncs.Add(Consts.Actions.getPossMoves, handleGetPossibleMoves);
        }


        private Connect4Command parseAPICommand(JSONObject jObj, string action) {
            Connect4Command command = new Connect4Command() { action = action };
            command.board = command.board = Utils.parseBoard(jObj.GetField(Consts.Fields.board).str);
            command.evaluate = jObj.HasField(Consts.Fields.evaluate);

            return command;
        }

        private APIMsgConclusion handleSimulateMove(JSONObject jObj) {
            Connect4Command command = parseAPICommand(jObj, Consts.Actions.simMove);
            command.player = jObj.GetField(Consts.Fields.player).str;
            command.getPossibleMoves = jObj.HasField(Consts.Fields.possibleMoves);
            command.move = (int)jObj.GetField(Consts.Fields.move).i;

            return APIGameLogic.simulateMove(command);
        }

        private APIMsgConclusion handleGetPossibleMoves(JSONObject jObj) {
            Connect4Command command = parseAPICommand(jObj, Consts.Actions.simMove);
            
            return APIGameLogic.aquirePossibleMoves(command);
        }


        protected override bool isJsonMsg(string input) { return input.Length > 1; }
    }
}