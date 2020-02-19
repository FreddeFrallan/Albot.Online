using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FallingServerController : Game.ServerController {

	public Game.GameMaster daGame = new FallingDebris.FallingGameMaster();

	public override Game.GameMaster getController (){
		return daGame;
	}

	public override void initController (Action<string> printFunc, Action<object, int, short> sendMsgFunc, Action shutdownGameServer, GameWrapper wrapper, List<string> preGamePlayers){
		daGame.init (printFunc, sendMsgFunc, shutdownGameServer, wrapper, preGamePlayers);
	}
}
