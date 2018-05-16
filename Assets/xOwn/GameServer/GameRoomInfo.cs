using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;



public class GameRoomInfo : NetworkBehaviour {
	[SyncVar]
	public Game.GameType currentGameType;
}
