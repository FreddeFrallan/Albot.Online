using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soldiers{

	public class GameOverlord : MonoBehaviour {

		public static readonly Vector2 minPos = new Vector2(0, 0), maxPos = new Vector2(100, 100);

		public GameObject redSoldier, blueSoldier;
		private int soldiersPerUnit = 20, soldiersPerRow = 10;
		private float lineSpace = 2f, rowSpace = 3;



		public List<Soldier> team1 = new List<Soldier>(), team2 = new List<Soldier>();
		private static GameOverlord singleton;
		private int idCounter = 0;


		void Awake(){
			singleton = this;
			spawnTeams (12, 77, 1, blueSoldier);
			spawnTeams (42, 77, 1, blueSoldier);
			spawnTeams (72, 77, 1, blueSoldier);

			spawnTeams (12, 20, 2, redSoldier);
			spawnTeams (42, 20, 2, redSoldier);
			spawnTeams (72, 20, 2, redSoldier);
		}


		private void spawnTeams(int xPos, int yPos, int team, GameObject s){
			for (int i = 0; i < soldiersPerUnit; i++) {
			//	Vector3 spawnPos = new Vector3 (Random.Range (minPos[0], maxPos[0]), 0, Random.Range (minPos[1], maxPos[1]));
				float ySpawnPos = yPos - (i / soldiersPerRow)*rowSpace;
				float xSpawmPos = (i % soldiersPerRow) * lineSpace + xPos;
				Vector3 spawnPos = new Vector3(xSpawmPos, 0, ySpawnPos);
				GameObject temp = Instantiate (s, spawnPos, Quaternion.identity);


				temp.GetComponent<Soldier> ().init (idCounter++, team);
				temp.transform.LookAt (new Vector3 (50, 1, 50));
			}
		}


		public static void addSoldierToList(Soldier s, int team){
			if (team == 1)
				singleton.team1.Add (s);
			else
				singleton.team2.Add (s);
		}

		public static void removeSoldierFromList(Soldier s, int team){
			if (team == 1)
				singleton.team1.Remove (s);
			else
				singleton.team2.Remove (s);

			Soldiers.SoldiersGameStateUpdater.deaths.Add (s);
		}

		public static List<Soldier> getEnemyList(int ownTeam){
			if (ownTeam == 1)
				return singleton.team2;
			else
				return singleton.team1;
		}



	}
}