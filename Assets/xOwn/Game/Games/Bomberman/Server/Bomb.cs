using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bomberman{

	public class Bomb : BombermanMapObject {

		private float explodeTime;
		private int explodeRange;
		private int[] pos;
		private MapSquare square;
		private PlayerController owner;

		public void init(float delay, int range, int[] pos, PlayerController owner){
			this.owner = owner;
			explodeRange = range;
			explodeTime = Time.time + delay;
			this.pos = (int[])pos.Clone();
			this.square = GameBoard.singleton.gridMap [pos [1]] [pos [0]];;
			square.setState (SquareState.Bomb);
			StartCoroutine (tickTock ());

			type = BombermanObjType.Bomb;
			ID = BombermanGameStateUpdater.generateNewID ();
			BombermanGameStateUpdater.addNewObj (this as BombermanMapObject);
		}
		 
		private IEnumerator tickTock(){
			while (Time.time < explodeTime)
				yield return new WaitForEndOfFrame ();
			explode ();
		}

		private void explode(){
			GameBoard.explodeBomb (pos);
			explodeInDir (1, 0);
			explodeInDir (0, 1);
			explodeInDir (-1, 0);
			explodeInDir (0, -1);
			square.setState (SquareState.Empty);
			BombermanGameStateUpdater.addNewAction (new BoardAction (ActionType.BombExplosion, ID, null));
			BombermanGameStateUpdater.removeObj (this as BombermanMapObject);

			if(owner != null)
				owner.placedBombExplode ();
			Destroy (gameObject);
		}

		private void explodeInDir(int x, int y){
			for (int i = 1; i <= explodeRange; i++) {
				int[] targetPos = new int[]{ pos [0] + x * i, pos [1] + y* i };
				if(coordOfMap(targetPos[0]) || coordOfMap(targetPos[1]) )
					continue;

				if (GameBoard.explodeBomb (targetPos) == false) //If we hit a obsticale we stop in that direction
					break;
			}
		}


		public override MapObj convertToMapObj(){
			MapObj temp = new MapObj (ID, type, BombermanOverlord.vecToFlArray (transform.localPosition));
			temp.explodeDuration =  explodeTime - Time.time;
			return temp;
		}

		private bool coordOfMap(int a){return a < 0 || a > GameBoard.mapMaxOffset;}
	}
}