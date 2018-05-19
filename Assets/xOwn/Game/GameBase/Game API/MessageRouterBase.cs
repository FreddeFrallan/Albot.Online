using Barebones.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

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
            if (isJsonMsg(msg) == false)
                return new APIMsgConclusion() { msg = msg, toServer = true, status = ResponseStatus.Success };

            try{
                JSONObject jObj = new JSONObject(msg.Trim());
                Func<JSONObject, APIMsgConclusion> linkedFunc = APIFuncs[jObj.GetField(TCPFields.action).str];
                return linkedFunc(jObj);
            }
            catch {return errorParsingJson();}
        }


        private APIMsgConclusion errorParsingJson() { return new APIMsgConclusion() { status = ResponseStatus.Error, msg = "Error Parsing Msg", toServer = false }; }
        protected abstract bool isJsonMsg(string input);
    }



}