using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Barebones.Networking;
using AlbotServer;

namespace Game{

	public abstract class ServerController : MonoBehaviour {

		public abstract GameMaster getController ();
		public abstract void initController (Action<string> printFunc, Action<object, int, short> sendMsgFunc, Action shutdownGameServer, GameWrapper wrapper, List<string> preGamePlayers);
	}

}