using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleshipsBot : Game.TrainingBot {

	public override List<Dictionary<string, string>> botSettings (){return new List<Dictionary<string,string>> () {
			new Dictionary<string, string> (){{"Name", "Normal"}, { "Normal", ""} }

		};
	}

	public override int defaultSettings (){return 0;}


	protected override void playMove (string input){
		string respons =  BattleShipsBot.MainClass.playMove (input);
		theClientController.onOutgoingLocalMsg(respons, botColor);
	}

	public override void initBot (Dictionary<string, string> settings){
		
	}
		
}
