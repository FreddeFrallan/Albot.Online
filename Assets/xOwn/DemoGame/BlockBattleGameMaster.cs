using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace BlockBattle{


	public class BlockBattleGameMaster : GameMaster {


		private BlockBattleProtocol protocol = new BlockBattleProtocol();

		public override GameType getGameType (){return GameType.BlockBattle;}
		public override bool isRealtime (){return true;}
		public override int maxNbrPlayers (){return 2;}
		public override CommProtocol getProtocol (){return protocol;}
		protected override void initProtocol (System.Action<object, int, short> sendMsgFunc){
			protocol.init (sendMsgFunc);
			colorOrder = new List<PlayerColor> (){PlayerColor.Green, PlayerColor.Red};
		}



		public BlockBattleUpdater updater;
		public BlockBattleLogic gameLogic;

		public override void startGame (){
			updater.init (protocol, players);
			gameLogic.startGame ();
		}

		public override void onPlayerLeft (ConnectedPlayer newPlayer){

		}





	}
}