using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace Bomberman{

	public class BombermanRenderer : MonoBehaviour {

		public Transform mapObjectsParent;
		public GameObject playerPrefab, bombPrefab, explosionPrefab;
		public Dictionary<PlayerColor, VisualPlayer> players = new Dictionary<Game.PlayerColor, VisualPlayer>();
		public Dictionary<PlayerColor, int> colorOrder = new Dictionary<Game.PlayerColor, int>(){{PlayerColor.Blue, 0}, {PlayerColor.Red, 1}};
		public List<Material> playerColors = new List<Material>();

		public List<BombermanVisualObj> currentGameObjects = new List<BombermanVisualObj>();

		public void initBoard(BoardUpdate board){
			foreach (MapObj obj in board.currentMap)
				if (obj.type == BombermanObjType.Player) {
					GameObject tempObj = Instantiate (playerPrefab, Vector3.zero, Quaternion.identity, mapObjectsParent);
					tempObj.transform.localPosition = BombermanOverlord.FArrayToVec (obj.pos);

					VisualPlayer tempPlayer = tempObj.GetComponent<VisualPlayer> ();
					tempPlayer.init (obj.color, playerColors [colorOrder[obj.color]], floatArrayToVec(obj.pos), obj.gameID);
					players.Add (obj.color, tempPlayer);
					currentGameObjects.Add (tempPlayer as BombermanVisualObj);
				}
		}
			

		public void renderNewBoard(BoardUpdate board){
			foreach (MapObj obj in board.currentMap)
				if (obj.type == BombermanObjType.Player)
					players [obj.color].moveToPos (floatArrayToVec(obj.pos), floatArrayToVec(obj.targetPos));

			foreach (BoardAction a in board.currentActions) {
				if (a.type == ActionType.BombSpawn)
					spawnBomb (a);
				if (a.type == ActionType.BombExplosion)
					explodeBomb (a);
				if (a.type == ActionType.PlayerDeath)
					playerDeath (a);
			}
		}



		private void spawnBomb(BoardAction a){
			GameObject temp = Instantiate (bombPrefab, Vector3.zero, Quaternion.identity, mapObjectsParent);
			temp.transform.localPosition = BombermanOverlord.FArrayToVec (a.pos);
			VisualBomb bomb = temp.GetComponent<VisualBomb> ();
			bomb.initBomb (a.targetID, 0, 0, explosionPrefab);
			currentGameObjects.Add (bomb as BombermanVisualObj);
		}


		private void explodeBomb(BoardAction a){
			BombermanVisualObj bomb = currentGameObjects.Find (x => x.ID == a.targetID);
			(bomb as VisualBomb).explodeBomb ();
			currentGameObjects.Remove (bomb);
		}

		private void playerDeath(BoardAction a){
			BombermanVisualObj player = currentGameObjects.Find (x => x.ID == a.targetID);
			(player as VisualPlayer).death ();
			currentGameObjects.Remove (player);
		}


		public static Vector3 floatArrayToVec(float[] array){return new Vector3 (array [0], array [1], array [2]);}
	}
}