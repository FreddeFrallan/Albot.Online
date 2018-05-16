using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Soldiers{

	public class SoldiersBoardCompresser{


		public static void compressBoardUpdate(BoardUpdate board){
			List<JsonSoldier> soldiers = new List<JsonSoldier> ();
			int ownTeam = board.color == Game.PlayerColor.Blue ? 1 : 2;

			for (int i = 0; i < board.ids.Length; i++) {
				soldiers.Add(new JsonSoldier ( 
					board.healths[i], board.units [i, 0], board.units [i, 1],
					board.canAttack[i], SoldierState.Unkown)
				);

				if (board.teams [i] && ownTeam == 1)
					soldiers [soldiers.Count - 1].state = board.states [i].ToString ();
				else if(!board.teams [i] && ownTeam == 2)
					soldiers [soldiers.Count - 1].state = board.states [i].ToString ();
			}
				
			Dictionary<string, JSONObject> team1 = new Dictionary<string, JSONObject> ();
			Dictionary<string, JSONObject> team2 = new Dictionary<string, JSONObject> ();


			for(int i = 0; i < soldiers.Count; i++){
				string JsonSoldier = JsonUtility.ToJson (soldiers[i]);
				JSONObject o = new JSONObject (JsonSoldier);

				if(board.teams[i])
					team1.Add (board.ids[i].ToString (), o);
				else
					team2.Add (board.ids[i].ToString (), o);
			}

			JSONObject jTeam1 = new JSONObject (team1);
			JSONObject jTeam2 = new JSONObject (team2);
			JSONObject jBoard = new JSONObject ();

			if (ownTeam == 1) {
				jBoard.AddField ("Units", jTeam1);
				jBoard.AddField ("Enemies", jTeam2);
			} else {
				jBoard.AddField ("Units", jTeam2);
				jBoard.AddField ("Enemies", jTeam1);
			}

			Game.RealtimeTCPController.gotNewBoard (ownTeam, jBoard.Print());
		}


		private class JsonSoldier{
			public float posX, posY;
			public int hp;
			public bool canAttack;
			public string state;
			public JsonSoldier(int health, float posX, float posY, bool canAttack, SoldierState state){
				this.hp = health; this.posX = (float)Math.Round(posX,2); this.posY =  (float)Math.Round(posY,2);
				this.canAttack = canAttack; this.state = state.ToString();
			}
		}
	}

}