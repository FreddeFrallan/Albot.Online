using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace Snake{

	public class SnakeTCPFormater : MonoBehaviour {

		private static int mapSize = 20;
		private int team;
		private List<int> freshCoords = new List<int> (), freshCoords2 = new List<int>(), oldCoords = new List<int> ();

		public SnakeTCPFormater(int team){
			this.team = team;
		}


		public void addNewUpdate(List<int> coords, int dir, int enemyDir, int playerPos, int enemyPos){
			foreach (int c in coords) {
				if (freshCoords.Any ((x) => x == c) || oldCoords.Any ((x) => x == c))
					continue;
				freshCoords.Add (c);
			}
				
			Game.RealtimeTCPController.gotNewBoard (team, formatBoard(dir, enemyDir, playerPos, enemyPos));
		}


		private string formatBoard(int dir, int enemyDir, int playerPos, int enemyPos){
			JsonPlayer player = new JsonPlayer (playerPos, dir);
			JsonPlayer enemy = new JsonPlayer (enemyPos, enemyDir);


			List<int> blockedCoords = new List<int> ();
			blockedCoords.AddRange (freshCoords);
			blockedCoords.AddRange (freshCoords2);

			blockedCoords = removePlayerFromFresh (playerPos, enemyPos, blockedCoords);
			string blockedString = hardCodeBlockedList (blockedCoords);

			JSONObject jBoard = new JSONObject ();
			jBoard.AddField ("Player", new JSONObject(JsonUtility.ToJson (player)));
			jBoard.AddField ("Enemy", new JSONObject(JsonUtility.ToJson (enemy)));


			string playersString = jBoard.Print();
			playersString = playersString.Substring (0, playersString.Length - 1);
			playersString += ", \"Blocked\":" + blockedString + "}";

			return playersString;
		}


		private string hardCodeBlockedList(List<int> blockedCoords){
			string s = "[";
			for (int i = 0; i < blockedCoords.Count; i++) {
				s +=  JsonUtility.ToJson (new JsonBlockedPos (blockedCoords[i]));
				if (i + 1 < blockedCoords.Count)
					s += ", ";
			}
			s += "]";
			return s;
		}


		public void newBoardSent(){
			oldCoords.AddRange (freshCoords2);
			freshCoords2.Clear ();
			freshCoords2.AddRange (freshCoords);
			freshCoords.Clear ();
		}


		private List<int> removePlayerFromFresh(int pPos, int ePos, List<int> blockedCoords){
			blockedCoords = blockedCoords.Where ((pos) => pos != pPos).ToList();
			blockedCoords = blockedCoords.Where ((pos) => pos != ePos).ToList();
			return blockedCoords;
		}
			

		private class JsonPlayer{
			public int posX, posY;
			public string direction;
			public JsonPlayer(int pos, int dir){
				this.posX = pos % mapSize;
				this.posY = mapSize - (pos / mapSize) -1;
				direction = dirToString(dir);
			}
		}

		private class JsonBlockedPos{
			public int posX, posY;
			public JsonBlockedPos(int pos){
				this.posX = pos % mapSize;
				this.posY = mapSize - (pos / mapSize) - 1;
			}
		}


		private static string dirToString(int dir){
			if (dir == 0)return "Right";
			if (dir == 1)return "Up";
			if (dir == 2)return "Left";
			else return "Down";
		}
	}

}