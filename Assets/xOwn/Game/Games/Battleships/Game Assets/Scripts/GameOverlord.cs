using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battleship{

	public class GameOverlord : MonoBehaviour {

		public static GameOverlord singleton;

		public Dictionary<ShipType, GameObject> ships = new Dictionary<ShipType, GameObject> ();
		public GameObject carrierShip, battleShip, scoutShip, submarineShip, transportShip;
		public GameObject hitAnim, missAnim;
		public Material enemyShader, playerShader, missSquare, hitSquare, notCheckedSquare;
		public GameObject rocket;
		public char missSign, hitSign, undiscoveredSign, sunkSign;



		void Awake(){
			GameOverlord.singleton = this;
			ships.Add (ShipType.battle, battleShip);
			ships.Add (ShipType.carrier, carrierShip);
			ships.Add (ShipType.scout, scoutShip);
			ships.Add (ShipType.submarine, submarineShip);
			ships.Add (ShipType.transport, transportShip);
		}
	}
}