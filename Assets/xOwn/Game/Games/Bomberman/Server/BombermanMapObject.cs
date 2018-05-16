using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bomberman{

	public abstract class BombermanMapObject : MonoBehaviour {
		public int ID;
		public BombermanObjType type;

		public abstract MapObj convertToMapObj ();
	}

}