using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battleship{

	public class Rocket : MonoBehaviour {

		private GameObject anim;
		private float animTime;
		private Square theSquare;
		private Vector3 startVel = new Vector3 (0, -2, 0);
		public GameObject smokeParticles, model;

		private Vector3 sunkShipSpawnPos;
		private ShipType sunkShipSpawnType;
		private bool sunkShipHorizontal, spawnSunkShip = false;
		private Ship alreadyExistingShip;

		public void init(GameObject anim, Square theSquare, float animTime){
			this.anim = anim;
			this.theSquare = theSquare;
			GetComponent<Rigidbody> ().velocity = startVel;
			this.animTime = animTime;
		}

		public void setSpawnSunkShip(ShipType type, Vector3 spawnPos, bool horizontal, Ship targetShip){
			this.sunkShipSpawnType = type;
			this.sunkShipSpawnPos = spawnPos;
			this.sunkShipHorizontal = horizontal;
			this.alreadyExistingShip = targetShip;
			spawnSunkShip = true;
		}


		private void spawnSinkingShip(){
			if (alreadyExistingShip != null) {
				alreadyExistingShip.kill ();
				return;
			}

	
			Ship sinkingShip = Instantiate (GameOverlord.singleton.ships [sunkShipSpawnType], sunkShipSpawnPos, Quaternion.identity).GetComponent<Ship>();
			int rot = sunkShipHorizontal ? 3 : 0;
			sinkingShip.transform.localEulerAngles = new Vector3 (0, 360 - rot * 90, 0);
			sinkingShip.kill ();
		}


		void OnTriggerEnter(Collider other){
			if (other.tag != "Water")
				return;

			theSquare.updateSquareVisuals ();

			if (anim != null) {
				Vector3 spawnPos = transform.position;
				spawnPos.y = other.transform.position.y;
				Destroy (Instantiate (anim, spawnPos, Quaternion.identity), animTime);
			}
			if (spawnSunkShip)
				spawnSinkingShip ();


			smokeParticles.GetComponent<ParticleSystem> ().Stop ();
			model.SetActive (false);
			Destroy (gameObject, 4);
		}
			
	}
}