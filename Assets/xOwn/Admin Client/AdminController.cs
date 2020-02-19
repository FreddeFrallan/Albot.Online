using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdminUI{

	public class AdminController : MonoBehaviour {

		public GameUI theGameUI;
		protected bool isInit = false;

		protected List<string> gameLog = new List<string>();


		public virtual void init(){
			isInit = true;
		}


		public virtual void initLog(string[] logHistory){
			//gameLog.Clear ();
			gameLog.AddRange (logHistory);
		}

		public virtual void addLogMove(string[] logMsg){
			if (isInit == false)
				init ();
			gameLog.AddRange (logMsg);
		}


		public virtual void handleGameInfoMsg(string msg){
			foreach (string player in msg.Split(':')) {
				if (player.Trim().Length == 0)
					continue;

				string[] words = player.Split (' ');
				int iconNumber = int.Parse (words [2]);
				theGameUI.initPlayerSlot (extractColor (words [0]), words [1], 20);
			}
		}


		private Game.PlayerColor extractColor(string msg){
			switch (msg.ToLower ()) {
				case "blue":return Game.PlayerColor.Blue;
				case "red":return Game.PlayerColor.Red;
				case "white":return Game.PlayerColor.White;
				case "black":return Game.PlayerColor.Black;
			}
			return Game.PlayerColor.None;
		}
	}
}