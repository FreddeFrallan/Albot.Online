using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bomberman{

	public class BombermanBoardParser{

		// NOTE
		// Inheritence in the classes ParsedMapObj & ParsedPlayerObj seems to not be working correctly?!?
		// Look into this later


		public static string parseBoard(BoardUpdate b){
			List<ParseBombObj> parsedObjs = new List<ParseBombObj> ();
			List<ParsedPlayerObj> parsedPlayers = new List<ParsedPlayerObj> ();

			foreach (MapObj obj in b.currentMap) {
				if (obj.type == BombermanObjType.Player) {
					string type = obj.color == b.color ? "Player" : "Enemy";
					ParsedPlayerObj temp = new ParsedPlayerObj (type, obj.pos, obj.bombsLeft);
					parsedPlayers.Add(temp);

					if (type == "Player" && b.color == Game.PlayerColor.Blue) {
						//Debug.Log ("Blue Player: " + temp.Pos [0]);
					}
				}

				if (obj.type == BombermanObjType.Bomb) {
					string type = "Bomb";
					float dur = (float)decimal.Round ((decimal)obj.explodeDuration, 2);
					ParseBombObj temp = new ParseBombObj (type, obj.pos, dur);
					parsedObjs.Add(temp);
				}
			}

			List<JSONObject> mapObjs = new List<JSONObject> ();
			foreach (ParsedPlayerObj obj in parsedPlayers)
				mapObjs.Add (new JSONObject( JsonUtility.ToJson (obj)));
			foreach (ParseBombObj obj in parsedObjs)
				mapObjs.Add (new JSONObject( JsonUtility.ToJson (obj)));


			JSONObject board = new JSONObject ();
			foreach (JSONObject jObj in mapObjs)
				board.Add (jObj);

			return board.ToString ();
		}




	}

	public class ParseBombObj{
		public string Type;
		public float[] Pos;
		public float Duration;
		
		public ParseBombObj(string Type, float[] Pos, float dur){
			this.Type = Type;
			float xPos = (float)decimal.Round((decimal)Pos[0], 2) + 4;
			float yPos = (float)decimal.Round((decimal)Pos[2], 2) + 4;
			this.Pos = new float[]{xPos, yPos};
			this.Duration = dur;
		}
	}
	
	
	public class ParsedPlayerObj{
		public string Type;
		public float[] Pos;
		public int Bombs;
		
		public ParsedPlayerObj(string Type, float[] Pos, int bombs){
			this.Type = Type;
			this.Bombs = bombs;
			float xPos = (float)decimal.Round((decimal)Pos[0], 2) + 4;
			float yPos = (float)decimal.Round((decimal)Pos[2], 2) + 4;
			this.Pos = new float[]{xPos, yPos};
		}
		
	}

}