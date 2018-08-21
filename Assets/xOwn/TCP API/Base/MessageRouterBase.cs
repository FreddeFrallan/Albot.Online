using Barebones.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCP_API{

    /// <summary>
    /// The base class for all games that implements a TCP API handler.
    /// 
    /// Contains a dictionary that maps parsed actions to handlers, so in order to use this all handlers for possible actions must be linked.
    /// If an action is found, but there is no specified handler it will return "errorParsingJson".
    /// 
    /// TODO: Implement a more detailed error system than only responding with "errorParsingJson"
    /// </summary>
	public abstract class APIMessageRouterBase{

        protected Dictionary<string, Func<JSONObject, APIMsgConclusion>> APIFuncs = new Dictionary<string, Func<JSONObject, APIMsgConclusion>>();

        /// <summary>
        /// Takes the raw text input sent from the client and parses it to Json.
        /// Thereafter it founds the specified handler for that action.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns>APIMsgConclusion</returns>
        public APIMsgConclusion handleIncomingMsg(string msg) {
            //msg = msg.Trim();
            if(msg == APIStandardConstants.Actions.restartGame) {
                MainThread.fireEventAtMainThread(() => ClientUI.CurrentGame.restartCurrentGame());
                return restartReturnMsg();
            }
            
            if (isJsonMsg(msg) == false)
                return new APIMsgConclusion() { msg = msg, target = MsgTarget.Server, status = ResponseStatus.Success };

            try{
                JSONObject jObj = new JSONObject(msg);
                Func<JSONObject, APIMsgConclusion> linkedFunc = APIFuncs[jObj.GetField(TCPFields.action).str];
                return linkedFunc(jObj);
            }
            catch(Exception e) {
                JSONObject jObj = new JSONObject(msg);
                Debug.LogError(jObj.GetField(TCPFields.action).str);
                Debug.LogError(e.Message);
                Debug.LogError(e.StackTrace);
                return errorParsingJson(e.Message);
            }
        }


        private APIMsgConclusion restartReturnMsg() {return new APIMsgConclusion() { status = ResponseStatus.Success, target = MsgTarget.None}; }
        private APIMsgConclusion errorParsingJson(string errorMsg) { return new APIMsgConclusion() { status = ResponseStatus.Error, msg = errorMsg, target = MsgTarget.Player}; }
        protected abstract bool isJsonMsg(string input);
    }



}