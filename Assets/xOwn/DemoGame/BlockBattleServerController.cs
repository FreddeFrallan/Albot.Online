using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockBattle{

	public class BlockBattleServerController : Game.ServerController {

		public BlockBattleUpdater updater;
		public BlockBattleLogic gameLogic;
		private BlockBattleGameMaster controller = new BlockBattleGameMaster();

		public override Game.GameMaster getController (){return controller;}
		public override void initController (System.Action<string> printFunc, System.Action<object, int, short> sendMsgFunc, 
			System.Action shutdownGameServer, GameWrapper wrapper, List<string> preGamePlayers){

			controller.updater = updater;
			controller.gameLogic = gameLogic;
			controller.init (printFunc, sendMsgFunc, shutdownGameServer, wrapper, preGamePlayers);
		}

	}
}