using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Battleship{
	public class GameBoardCreator : MonoBehaviour {
		
		private List<ShipFormat> shipFormats = new List<ShipFormat> ();
		void Start(){
			shipFormats.Add (new ShipFormat (ShipType.carrier, "55555", GameOverlord.singleton.carrierShip));
			shipFormats.Add (new ShipFormat(ShipType.battle, "4444", GameOverlord.singleton.battleShip));
			shipFormats.Add (new ShipFormat(ShipType.transport, "333", GameOverlord.singleton.transportShip));
			shipFormats.Add (new ShipFormat(ShipType.submarine, "222", GameOverlord.singleton.submarineShip));
			shipFormats.Add (new ShipFormat(ShipType.scout, "11", GameOverlord.singleton.scoutShip));
		}
			
	
		public void initPlayerBoard(char[,] inputBoard, PlayerBoard currentPlayer){
			List<Ship> ships = new List<Ship> ();
			List<string> lines = new List<string> ();

			lines.AddRange(generateRows (inputBoard) );
			lines.AddRange(generateCols (inputBoard) );

			for (int i = 0; i < lines.Count; i++)
				foreach (ShipFormat SF in shipFormats)
					if (lines [i].Contains (SF.stringFormat)) 
						ships.Add(createShip (SF, i, lines[i].IndexOf(SF.stringFormat)) );

			currentPlayer.placeShips (ships);
		}

		private Ship createShip(ShipFormat SF, int line, int startIndex){
			Ship tempShip = Instantiate (SF.prefabObj).GetComponent<Ship>();
			int rot = line >= 10 ? 3 : 0; // If in cols rot down, rows we rot right
			int[] startPos = new int[2];

			if (line >= 10) {
				startPos [0] = line - 10;
				startPos[1] = startIndex;
			}
			else{
				startPos [0] = startIndex;
				startPos[1] = line;
			}
				
			tempShip.setCoords (startPos, rot);
			return tempShip;
		}


		#region compress to strings
		private List<string> generateRows(char[,] charBoard){
			List<string> rows = new List<string> ();
			for (int y = 0; y < 10; y++) {
				string temp = "";
				for (int x = 0; x < 10; x++)
					temp += charBoard [x, y].ToString ();
				rows.Add (temp);
			}

			return rows;
		}
		private List<string> generateCols(char[,] charBoard){
			List<string> cols = new List<string> ();
			for (int x = 0; x < 10; x++) {
				string temp = "";
				for (int y = 0; y < 10; y++)
					temp += charBoard [x, y].ToString ();
				cols.Add (temp);
			}

			return cols;
		}
		#endregion
	}


	public enum ShipType{
		carrier,
		battle,
		transport,
		submarine,
		scout
	}

	public class ShipFormat{
		public ShipType type;
		public string stringFormat;
		public GameObject prefabObj;
		public ShipFormat(ShipType type, string stringFormat, GameObject prefabObj){
			this.type = type;
			this.stringFormat = stringFormat;
			this.prefabObj = prefabObj;
		}
	}
}