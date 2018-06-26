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


            CurrentGame.init(gameConnector);
            Msf.Server.SetHandler((short)ServerCommProtocl.GameRoomInvite, CurrentGame.handleStartGame);
			Game.ClientPlayersHandler.addUIStateListner ();
		}

	}

}