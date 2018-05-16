using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FallingServerController : Game.ServerController {

	public Game.GameMaster daGame = new FallingDebris.FallingGameMaster();

	public override Game.GameMaster getController (){
		return daGame;
	}

	public override void initController (System.Action<string> printFunc, System.Action<object, int, short> sendMsgFunc, System.Action shutdownGameServer, GameWrapper wrapper, List<string> preGamePlayers){
		daGame.init (printFunc, sendMsgFunc, shutdownGameServer, wrapper, preGamePlayers);
	}
}
