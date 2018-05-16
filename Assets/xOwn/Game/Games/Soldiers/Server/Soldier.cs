using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using System.Linq;

namespace Soldiers{
	
	public abstract class Soldier : MonoBehaviour {
		protected Vector3 moveTarget;
		protected NavMeshAgent agent;
		public int HP = 30;
		public int team = 1;
		public int id = 0;
		protected float stopDist = 0.8f;

		protected float attackSpeed = 1;
		protected int damage = 10;
		protected float attackRange = 1.3f;
		public bool canDmg = true;
		protected bool autoAttack = true;
		public Soldier attackTarget;
		protected Transform attackTargetTransform;
		public SoldierState intention = SoldierState.Idle;

		public event Action<Soldier> SoldierDeath;

		// Use this for initialization

		public void init (int id, int team) {
			this.id = id;
			this.team = team;
			agent = GetComponent<NavMeshAgent> ();
			GameOverlord.addSoldierToList (this, team);
		}

		// Update is called once per frame
		void Update () {
			if (intention == SoldierState.Attacking)
				attack ();
			if (intention == SoldierState.Moving)
				moving ();
		}


		protected void stop(){agent.Stop ();}
		protected abstract void attack ();
		protected abstract void moving ();


		private void death(){
			if (attackTarget != null)
				attackTarget.SoldierDeath -= targetDied;
			GameOverlord.removeSoldierFromList (this, team);
			if (SoldierDeath != null)
				SoldierDeath.Invoke (this);
			Destroy (gameObject);
		}

		public void takeDmg(int dmg){
			HP -= dmg;
			if (HP <= 0)
				death ();
		}

		public void targetDied(Soldier deadSoldier){
			deadSoldier.SoldierDeath -= targetDied;
			if (autoSelectTarget () == false)
				startIdle ();
		}


		protected void startIdle(){
			intention = SoldierState.Idle;
			stop ();
		}



		protected bool autoSelectTarget(){
			if (autoAttack == false)
				return false;
			
			agent.Resume ();
			List<Soldier> enemies = GameOverlord.getEnemyList (team);
			if (enemies.Count == 0)
				return false;
			enemies = enemies.OrderBy (x => Vector3.Distance (transform.position, x.transform.position)).ToList ();
			for (int i = 0; i < enemies.Count (); i++)
				if (enemies [i] != null) {
					setAttackTarget (enemies[i]);
					return true;
				}
			
			return false;
		}
			
		public void setAttackTarget(Soldier target){
			if (attackTarget != null)
				attackTarget.SoldierDeath -= targetDied;

			intention = SoldierState.Attacking;
			target.SoldierDeath += targetDied;
			attackTarget = target;
			attackTargetTransform = target.transform;
		}


		public void setMovingTarget(Vector2 target){
			intention = SoldierState.Moving;
			moveTarget = new Vector3 (target.x, 0, target.y);
			agent.SetDestination (moveTarget);
			agent.Resume ();
		}




		public void startAutoAttack(){
			autoAttack = true;
			if(autoSelectTarget ())
				intention = SoldierState.Attacking;
		}

		public void setNewState(SoldierState state){
			if (state == intention)
				return;
		
		}
	}


	public enum SoldierState{
		Moving,
		Attacking,
		Patrol,
		Idle,
		Unkown
	}
}