using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace Bomberman{

	public abstract class BombermanVisualObj : MonoBehaviour {

		public PlayerColor color;
		public int ID;


		public void initVisualObj(PlayerColor color, int id){
			this.color = color;
			this.ID = id;
		}
	}

}