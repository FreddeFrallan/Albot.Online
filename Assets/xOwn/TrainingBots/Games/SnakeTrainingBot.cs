﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SnakeBot;

namespace Snake{

	public class SnakeTrainingBot : Game.TrainingBot {
		private SnakeBot.Board snakeBoard;

		public override int defaultSettings (){return 0;}
		public override List<Dictionary<string, string>> botSettings (){
			return new List<Dictionary<string,string>> () {
				new Dictionary<string, string> (){{"Name", "Stupid"}, { "Level", "1"} },
			};
		}


		public override void initBot (Dictionary<string, string> settings){
            SnakeBot.MainClass.setPrintFunction(Debug.Log);
            snakeBoard = SnakeBot.MainClass.init ();
		}


		protected override void playMove (string input){
			theClientController.onOutgoingLocalMsg (SnakeBot.MainClass.makeMove (input, snakeBoard), botColor);
		}
			

    }
}