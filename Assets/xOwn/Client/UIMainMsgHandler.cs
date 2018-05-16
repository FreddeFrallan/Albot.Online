using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;
using Barebones.Networking;
using AlbotServer;

namespace ClientUI{

	public class UIMainMsgHandler : MonoBehaviour {

		public NewGameCreator gameCreator;
		public GameConnectorUI gameConnector;
		// Use this for initialization
		void Awake () {
			if (ClientUIOverlord.hasLoaded)
				return;

			Msf.Connection.SetHandler((short)ServerCommProtocl.RequestJoinPreGame, gameCreator.handleJoinPreGameMsg);
			Msf.Connection.SetHandler((short)ServerCommProtocl.StartPreGame, gameConnector.onJoinStartedGame);
			Game.ClientPlayersHandler.addUIStateListner ();
		}

	}

}