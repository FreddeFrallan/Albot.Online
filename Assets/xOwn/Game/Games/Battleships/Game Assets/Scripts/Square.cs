using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battleship{
	
	public class Square : MonoBehaviour {
		public SquareStatus status = SquareStatus.empty;
		public Ship ship;
		private GameObject anim;
		private float rocketHeight = 7;
		public bool lastSunkSquare = false;

		private float animTime;
		private float animHitTime = 4, animMissTime = 1.5f;


		public void fireAtSquare(){
			if (ship != null)
				ship.takeFire (this);
			else
				status = SquareStatus.miss;
			decideAnim ();
			spawnRocket();
		}

		public void fireAtOponentSquare(char targetStatus, ShipType targetType, Vector3 spawnPos, bool horizontal){
			bool spawnSunkShip = false;
			if (targetStatus == GameOverlord.singleton.hitSign || targetStatus == GameOverlord.singleton.sunkSign) {
				anim = GameOverlord.singleton.hitAnim;
				animTime = animHitTime;
				status = SquareStatus.hit;

				spawnSunkShip = targetStatus == GameOverlord.singleton.sunkSign;
			} else {
				anim = GameOverlord.singleton.missAnim;
				animTime = animMissTime;
				status = SquareStatus.miss;
			}
				
			spawnRocket (targetType, spawnPos, horizontal, spawnSunkShip);
		}

		private void spawnRocket(ShipType targetType = ShipType.battle, Vector3 spawnPos = new Vector3(), bool horizontal = false, bool spawnSunkShip = false){
			Rocket tempR = Instantiate (GameOverlord.singleton.rocket, transform.position + new Vector3 (0, rocketHeight, 0), GameOverlord.singleton.rocket.transform.rotation).GetComponent<Rocket>();
			tempR.init (anim, this, animTime);

			if (spawnSunkShip) 
				tempR.setSpawnSunkShip (targetType, spawnPos, horizontal, ship);
		}


		private void decideAnim(){
			if (lastSunkSquare) {
				anim = GameOverlord.singleton.hitAnim;
				lastSunkSquare = false;
				animTime = animHitTime;
			} else if (status == SquareStatus.miss || status == SquareStatus.sunk) {
				anim = GameOverlord.singleton.missAnim;
				animTime = animMissTime;
			} else if (status == SquareStatus.hit) {
				anim = GameOverlord.singleton.hitAnim;
				animTime = animHitTime;
			}
		}

		public void updateSquareVisuals(){
			Material mat;
			if (status == SquareStatus.hit || status == SquareStatus.sunk)
				mat = GameOverlord.singleton.hitSquare;
			else
				mat = GameOverlord.singleton.missSquare;

			foreach(MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
				mesh.material = mat;

			if (ship != null)
				ship.squareVisualHit ();
		}
	}

	public enum SquareStatus{
		empty, miss, hit, sunk
	}

}