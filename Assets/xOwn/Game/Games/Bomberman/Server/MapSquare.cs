using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bomberman{
	public class MapSquare : MonoBehaviour {
		public SquareState currentState(){return state;}
		public bool plannedToBeMovedTo = false;
		public int[] ID;

		private SquareState state;
		private PlayerController currentPlayer;
		private bool isWalkable = false;

		public void setState(SquareState newState){
			state = newState;
			isWalkable = state == SquareState.Empty || state == SquareState.Player;
		}

		public void removePlayer(){
			if (state == SquareState.Player)
				state = SquareState.Empty;
			currentPlayer = null;
			plannedToBeMovedTo = false;
		}

		public void planToMoveToSquare(){
			plannedToBeMovedTo = true;
		}


		public bool explode(){
			if (currentPlayer != null)
				currentPlayer.takeDamage ();
			return state != SquareState.Obsticale;
		}

		public void setPlayer(PlayerController player){currentPlayer = player;}
		public bool getIsWalkable(){return isWalkable;}
		public Vector3 getPos(){return transform.position;}
	}

	public enum SquareState{
		Empty,
		Player,
		Bomb,
		Obsticale
	}
}