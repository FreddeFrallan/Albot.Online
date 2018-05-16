using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Bomberman{

	public class VisualBomb : BombermanVisualObj {

		private float explodeDuration;
		private float explodeRange = 3;
		private GameObject explosionPrefab;

		public void initBomb(int id, int range, float explodeDuration, GameObject explosionPrefab){
			initVisualObj (Game.PlayerColor.None, id);
			this.explodeDuration = explodeDuration;
			this.explosionPrefab = explosionPrefab;
		}

		public void explodeBomb(){
		//	spawnExplosion (0, 0);
			explodeInDir (1, 0);
			explodeInDir (-1, 0);
			explodeInDir (0, 1);
			explodeInDir (0, -1);
			Destroy (gameObject);
		}


		private void explodeInDir(int x, int y){
			for (int i = 1; i <= explodeRange; i++) {
				int[] pos = new int[]{(int)transform.localPosition.x + x * i, (int)transform.localPosition.z +  y* i};

				//Check for collision
				int xEven = pos [0] % 2;
				int yEven = pos [1] % 2;
				if (xEven != 0 && yEven != 0)
					break;

				//Check out of bounds
				if (pos [0] > 4 || pos [0] < -4 || pos [1] > 4 || pos [1] < -4)
					break;

				spawnExplosion (new Vector3(pos[0], 0, pos[1]));
			}
		}
		private void spawnExplosion(Vector3 localOffset){
			GameObject tempBomb = Instantiate (explosionPrefab, transform.position, Quaternion.identity, transform.parent);
			tempBomb.transform.localPosition = localOffset;
			Destroy (tempBomb, 2);
		}

		private void spawnExplosion(int x, int y){spawnExplosion(transform.localPosition + new Vector3(x, 0, y));}
	}

}