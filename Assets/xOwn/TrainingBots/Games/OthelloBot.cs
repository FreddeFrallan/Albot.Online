using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OthelloBot : Game.TrainingBot{
	public override List<Dictionary<string, string>> botSettings (){return new List<Dictionary<string,string>> () {
			new Dictionary<string, string> (){{"Name", "Level 1"}, { "Level", "1"} },
			new Dictionary<string, string> (){{"Name", "Level 2"}, { "Level", "2"} },
			new Dictionary<string, string> (){{"Name", "Level 3"}, { "Level", "3"} },
			new Dictionary<string, string> (){{"Name", "Level 4"}, { "Level", "4"} },
			new Dictionary<string, string> (){{"Name", "Level 5"}, { "Level", "5"} },
			new Dictionary<string, string> (){{"Name", "Level 6"}, { "Level", "6"} },
			new Dictionary<string, string> (){{"Name", "Level 7"}, { "Level", "7"} },
		};
	}
		
	public override int defaultSettings (){return 3;}
	protected override void playMove (string input){
		theClientController.onOutgoingLocalMsg (OthelloTrainingBot.MainClass.playMove (input), botColor);
	}

	public override void initBot (Dictionary<string, string> settings){
		int level = int.Parse (settings ["Level"]);
		OthelloTrainingBot.MainClass.initSettings (level);
	}

}
