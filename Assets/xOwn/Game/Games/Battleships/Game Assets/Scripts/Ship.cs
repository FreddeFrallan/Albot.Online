using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battleship{

	public class Ship : MonoBehaviour {

		public Animator theAnim;
		public char sign;
		public int rot = 0;
		public int size = 4;
		public List<int[]> coords = new List<int[]>();
		public List<Square> squares = new List<Square> ();
		private bool isDead = false;

		#region init
		//Rot: 0 - Right     1 - Up      2 - Left     3 -Down
		public void setCoords(int[] pos, int rot){
			this.rot = rot;
			int xDir = (rot % 2 == 0) ? 1 - rot : 0;
			int yDir = (rot % 2 != 0) ? 2 - rot : 0;
			List<int[]> tempCoords = new List<int[]> ();

			int[] tempPos = (int[])pos.Clone ();
			for (int i = 0; i < size; i++) {
				tempCoords.Add (new int[]{ tempPos [1], tempPos [0] }); // Inverting because in Game the X is up and down the board

				tempPos [0] += xDir;
				tempPos [1] -= yDir;
			}

			coords.AddRange(tempCoords);
		}

		public void assignSquare(Square s){
			squares.Add (s);
			s.ship = this;
		}
		#endregion


		public void takeFire(Square target){
			squares.Find (x => x == target).status = SquareStatus.hit;

			bool isSunk = true;
			foreach (Square s in squares)
				if (s.status != SquareStatus.hit)
					isSunk = false;

			if (isSunk) {
				for(int i = 0; i < squares.Count; i++)
					squares[i].status = SquareStatus.sunk;

				isDead = true;
				target.lastSunkSquare = true;
			}
		}

		public void squareVisualHit(){
			if (isDead)
				theAnim.SetBool("isDead", true);
		}

		public void kill(){
			isDead = true;
			squareVisualHit ();
		}


		#region Teams
		public void setTeam(int team){
			if (team == 0)
				setMaterial(GameOverlord.singleton.playerShader);
			else
				setMaterial(GameOverlord.singleton.enemyShader);
		}

		private void setMaterial(Material mat){
			foreach(MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
				mesh.material = mat;
		}
		#endregion
	}

}