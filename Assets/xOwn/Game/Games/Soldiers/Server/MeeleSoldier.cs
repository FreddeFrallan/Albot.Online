using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soldiers{
	public class MeeleSoldier : Soldier {
		protected override void moving (){
			if (Vector3.Distance (transform.position, moveTarget) <= stopDist) {
				startAutoAttack ();
			}
		}

		protected override void attack (){
			if (Vector3.Distance (transform.position, attackTargetTransform.position) <= attackRange) {
				stop ();
				dmgTarget ();
			} else {
				agent.Resume ();
				agent.SetDestination (attackTargetTransform.position);
			}
		}
			
		private void dmgTarget(){
			if (canDmg == false)
				return;

			SoldiersDamageOverlord.addDmg(attackTarget, calcAttackDmg());
			canDmg = false;
			StartCoroutine (attackCooldown ());
		}


		private IEnumerator attackCooldown(){
			yield return new WaitForSeconds (attackSpeed);
			canDmg = true;
		}


		private int calcAttackDmg(){return damage;}
	}
}