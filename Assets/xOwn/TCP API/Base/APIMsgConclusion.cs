using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.Networking;


namespace TCP_API{

    /// <summary>
    /// Container of information regarding the parsed TCP msg and what to do with that information.
    /// </summary>
	public struct APIMsgConclusion{
		public string msg;
		public MsgTarget target;
		public ResponseStatus status;
	}

    public enum MsgTarget {
        Server, Player, None
    }
}