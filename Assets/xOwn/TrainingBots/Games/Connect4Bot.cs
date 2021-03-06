﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using TCP_API.Connect4;

namespace Connect4Bot{

	public class Connect4Bot : Game.TrainingBot{
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


		private int depthLevel;

		public override void initBot (Dictionary<string, string> settings){
			int level = int.Parse (settings ["Level"]);
			depthLevel = level;
		}

		protected override void playMove (string input){
			theClientController.onOutgoingLocalMsg (requestMove (input).ToString (), botColor);
		}

			
		public int requestMove(string boardUpdate){
            JSONObject jObj = new JSONObject(boardUpdate);
			int[,] board = parseBoard (jObj.GetField(Consts.Fields.board));
			return Connect4BotMinMax.MinMaxSearch.findBestMove (board, depthLevel);
		}
			
		private static int[,] parseBoard(JSONObject grid){
			int tempRoundCounter = 0;
			int[,] tempBoard = new int[6, 7];

            Debug.Log(grid);
			for (int y = 0; y < 6; y++)
				for (int x = 0; x < 7; x++) {
					tempBoard [y, x] = (int)grid.list[y].list[x].i;
					if (tempBoard [y, x] != 0)
						tempRoundCounter++;
				}
			return tempBoard;
		}
	}

}