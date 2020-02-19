using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockBattle{

	public class BlockBattleUpdater : MonoBehaviour {

		private BlockBattleGameMaster gameMaster;
		private List<ConnectedPlayer> players;
		private BlockBattleProtocol protocol;
		private uint updateCounter = 1;

		public void init(BlockBattleProtocol protocol, List<ConnectedPlayer> players){
			this.protocol = protocol;
			this.players = players;
		}



		public void sendBoardUpdate(Vector3 p1, Vector3 p2){
			int[] pos1 = new int[]{ (int)p1.x, (int)p1.z };
			int[] pos2 = new int[]{ (int)p2.x, (int)p2.z };

			GameUpdate update = new GameUpdate(updateCounter++, pos1, pos2);
			broadcastUpdate (update);
		}


		private void broadcastUpdate(GameUpdate update){
			foreach (ConnectedPlayer p in players) {
				update.myColor = p.color;
				try{
					protocol.sendBoard (gameMaster.getMatchingPlayer(p.color).client.peerID, update);
				}catch{Debug.LogError ("Error Sending broadcast");}
			}
		}


	}

}