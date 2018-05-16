using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soldiers{

	public class SoldiersDamageOverlord : MonoBehaviour {

		private static List<dmg> roundDmg = new List<dmg> ();


		void Update(){
			foreach (dmg d in roundDmg)
				d.target.takeDmg (d.amount);
			roundDmg.Clear ();
		}


		public static void addDmg(Soldier t, int a){
			roundDmg.Add(new dmg(){target = t, amount = a});
		}

		private struct dmg{
			public Soldier target;
			public int amount;
		}
	}

}