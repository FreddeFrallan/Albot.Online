using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Soldiers{

	public class SoldiersTCPProtocol {

		private static Dictionary<string, TCPCommandPrefab> prefabCommands = new Dictionary<string, TCPCommandPrefab> () {
			{"attack", new TCPCommandPrefab(){amountOfFollowData = 1, type = TCPCommandType.attack}},
			{"autoattack", new TCPCommandPrefab(){amountOfFollowData = 0, type = TCPCommandType.autoAttack}},
			{"move", new TCPCommandPrefab(){amountOfFollowData = 2, type = TCPCommandType.move}}
		};


		public static List<TCPCommand> convertMsgToCommands(string msg){
			List<TCPCommand> extractedCommands = extractMsgCommands (msg);
			List<TCPCommand> validCommands = new List<TCPCommand> ();

			List<int> checkedIds = new List<int> ();
			for (int i = 0; i < extractedCommands.Count; i++) {
				if (checkedIds.Contains (extractedCommands [i].soldierId) == false)
					validCommands.Add (extractedCommands [i]);

				checkedIds.Add (validCommands [i].soldierId);
			}

			return validCommands;
		}



		private static List<TCPCommand> extractMsgCommands(string msg){
			List<TCPCommand> newCommands = new List<TCPCommand> ();

			msg = msg.ToLower ();
			string[] soldierCommands = msg.Split (':');
			for(int i = 1; i < soldierCommands.Length; i++){
				string[] words = soldierCommands[i].Split (new char[]{',', ' '});

				int id;
				if (int.TryParse (words [0], out id) == false)
					continue;
				

				int followDataCounter = 0;
				TCPCommand lastCommand = null;

				foreach (string w in words) {


					if (followDataCounter > 0) {
						followDataCounter--;;
						if (lastCommand.enterFollowData (w) == false) {
							newCommands.Remove (lastCommand);
							followDataCounter = 0;
						}
						continue;
					}
					if (prefabCommands.ContainsKey (w) == false) {
						followDataCounter = 0;
						continue;
					}
					TCPCommandPrefab prefab = prefabCommands [w];
					followDataCounter = prefab.amountOfFollowData;

					lastCommand = new TCPCommand (id, prefab.type, prefab.amountOfFollowData);
					newCommands.Add (lastCommand);
				}
			}

			return newCommands;
		}



		private struct TCPCommandPrefab{
			public int amountOfFollowData;
			public TCPCommandType type;
		}
	}

	[Serializable]
	public class TCPCommand{
		public TCPCommandType type;
		public float[] followData;
		public int soldierId;
		private int dataCounter = 0;
		public TCPCommand(int soldierId, TCPCommandType type, int amountOfData){
			this.soldierId = soldierId; this.type = type;
			this.followData = new float[amountOfData];
		
		}
		public bool enterFollowData(string data){
			return float.TryParse (data, out followData[dataCounter++]);
		}
		public override string ToString (){
			string dataString = "";
			for (int i = 0; i < followData.Length; i++)
				dataString += followData [i] + ", ";
			return soldierId.ToString () + "-" + type.ToString () + (dataCounter == 0 ? "" : dataString);
		}
	}

	[Serializable]
	public enum TCPCommandType{
		attack,
		move,
		autoAttack,
		idle
	}

}