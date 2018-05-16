using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPole : MonoBehaviour {

	public int amountOfDiscs = 0;

	public void removeDisc(){amountOfDiscs--;}

	public Vector3 addDisc(){
		amountOfDiscs++;
		return transform.position + new Vector3 (0, -1.79f + 0.5f*(amountOfDiscs-1), 0) ;
	}

}
