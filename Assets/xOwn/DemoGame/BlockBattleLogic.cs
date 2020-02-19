using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockBattle{

	public class BlockBattleLogic : MonoBehaviour {

		public BlockBattleUpdater updater;
		public List<GameObject> avatars;

		private float updateCooldown = 0.1f;
		private bool gameRunning;
		

		public void movePlayer(int playerIndex, Vector2 pos){
			float xPos = Mathf.Clamp (pos.x, -4, 4);
			float zPos = Mathf.Clamp (pos.y, -4, 4);

			avatars [playerIndex].transform.position = new Vector3 (xPos, 0, zPos);
		}


		public void startGame(){
			gameRunning = true;
			StartCoroutine (sendUpdate ());
		}

		private IEnumerator sendUpdate(){
			while (gameRunning) {
				updater.sendBoardUpdate (avatars [0].transform.position, avatars [1].transform.position);
				yield return new WaitForSeconds (updateCooldown);
			}
		}

	}

}