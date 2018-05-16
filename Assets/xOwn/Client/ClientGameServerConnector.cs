using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using Game;
using AlbotServer;

public class ClientGameServerConnector : NetworkBehaviour {

	//Is a override of a NetworkBehaviour, will run on the client when connection is settled.
	public override void OnStartAuthority(){
		connectionToServer.RegisterHandler ((short)ServerCommProtocl.GameRoomInitMsgChannel, onGotCurrentGameType);
		connectionToServer.Send ((short)ServerCommProtocl.GameRoomInitMsgChannel, new GameRoomInitMsg ());
	}

	private void onGotCurrentGameType(NetworkMessage msg){
		GameRoomInitMsg response = (GameRoomInitMsg)msg.ReadMessage<GameRoomInitMsg> ();
		Debug.LogError ("Starting Client, Gametype: " + response.type);

		foreach (ClientController c in GetComponents<ClientController>())
			c.initController (response.type);
	}
}
