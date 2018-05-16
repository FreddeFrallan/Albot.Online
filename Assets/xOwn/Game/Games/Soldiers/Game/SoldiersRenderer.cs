using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClientUI;

namespace Soldiers{

	public class SoldiersRenderer : MonoBehaviour {

		public ClientUserPanelUI[] panels;
		public GameObject redPrefab, bluePrefab;
		private Dictionary<int, VisualSoldier> units = new Dictionary<int, VisualSoldier>();
		private int updateCounter = 0;

		public void createNewUnits(float[,] pos, bool[] team, int[] ids, float time){
			for (int i = 0; i < team.Length; i++) {
				GameObject tempPrefab = team [i] ? bluePrefab : redPrefab;
				Vector3 spawnPos = new Vector3 (pos [i, 0], 0, pos [i, 1]);
				VisualSoldier tempSpawn = Instantiate (tempPrefab, spawnPos, Quaternion.identity).GetComponent<VisualSoldier>();
				units.Add (ids[i], tempSpawn);
				tempSpawn.addNewPosition (time, spawnPos);
			}
		}
			


		public void addBoardUpdate(float[,] pos, bool[] team, int[] ids, float time, int[] deaths){
			//print ("Update: " + updateCounter);
			for (int i = 0; i < ids.Length; i++)
				units [ids[i]].addNewPosition (time, new Vector3 (pos [i, 0], 0, pos [i, 1]));


			if (++updateCounter == 3)
				foreach (int key in units.Keys)
					units [key].startMoving ();
			
			foreach (int i in deaths) {
				if (units.ContainsKey (i) == false)
					continue;
				
				units [i].death (time);
				units.Remove (i);
			}
			setCurrentScore (team);
		}


		private void setCurrentScore(bool[] teams){
			int blue = 0, red = 0;
			foreach (bool t in teams)
				if (t)
					blue++;
				else
					red++;

			panels [0].setScore (blue);
			panels [1].setScore (red);
		}
	}
}