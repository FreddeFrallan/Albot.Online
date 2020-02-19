using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BrakethroughBot;

namespace Breakthrough{

	public class BreakthroughTrainingBot : Game.TrainingBot{

		private int boardHeight = 8;

		public override List<Dictionary<string, string>> botSettings (){return new List<Dictionary<string,string>> () {
				new Dictionary<string, string> (){{"Name", "Level 1"}, { "Level", "1"} },
				new Dictionary<string, string> (){{"Name", "Level 2"}, { "Level", "2"} },
				new Dictionary<string, string> (){{"Name", "Level 3"}, { "Level", "3"} },
				new Dictionary<string, string> (){{"Name", "Level 4"}, { "Level", "4"} },
			};
		}
		public override int defaultSettings (){return 1;}


		protected override void playMove (string input){
			string move = MainClass.playBotMove (input);
			int[] start = new int[2], target = new int[2];
			if (ClientUtil.stringToCoord (move.Substring (0, 2), ref start) == false || ClientUtil.stringToCoord (move.Substring (2, 2), ref target) == false)
				return;
			
			if (botColor == Game.PlayerColor.Black)
				ClientUtil.rotateCoords (boardHeight, ref start, ref target);

			move = ClientUtil.moveToString (start, target);
			MainThread.fireEventAtMainThread(() => {theClientController.onOutgoingLocalMsg (move, botColor);});
		}

		public override void initBot (Dictionary<string, string> settings){
			int level = int.Parse (settings ["Level"]);
			MainClass.initBot (level);
		}
			
	}
}