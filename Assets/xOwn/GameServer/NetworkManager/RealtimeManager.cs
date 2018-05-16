using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace AlbotServer{
	public class RealtimeManager : MonoBehaviour {
	
		private bool gameRunning = false;
		private RealtimeGame gameController;


		public void init(bool isRealtime, GameMaster gameController){
			if (isRealtime == false) {
				Destroy (this);
				return;
			}
			this.gameController = (gameController as RealtimeGame);
			startGame ();
		}


		public void startGame(){
			gameRunning = true;
			StartCoroutine (requestGameState ());
		}
		public void stopGame(){gameRunning = false;}

		// Update is called once per frame
		private IEnumerator requestGameState(){
			while (gameRunning) {
				gameController.Update ();
				//yield return new WaitForSeconds (0.5f);
				yield return new WaitForEndOfFrame();
			}
		}

	}

}