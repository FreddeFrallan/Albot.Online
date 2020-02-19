using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBattleRenderer : MonoBehaviour {

	public List<GameObject> avatars;

	public void movePlayer(int[] pos1, int[] pos2){
		avatars [0].transform.position = new Vector3 (pos1[0], 0, pos1[1]);
		avatars [1].transform.position = new Vector3 (pos2[0], 0, pos2[1]);
	}

}
