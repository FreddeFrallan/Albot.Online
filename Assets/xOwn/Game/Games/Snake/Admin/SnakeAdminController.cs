using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snake;
using System.Linq;


namespace AdminUI{

	public class SnakeAdminController : AdminController {

		public SnakeRenderer boardRenderer;
		private Player[] playerHistory = new Player[2];
		private bool isInit = false;

		public override void init (){
			base.init ();
			playerHistory [0] = new Player ();
			playerHistory [1] = new Player ();
		}

		public override void initLog (string[] logHistory){
			if (logHistory.Length == 0 || logHistory [0].Length == 0 || logHistory [0] [0] != ':') {
				addLogMove (logHistory);
				return;
			}

			handleGameInfoMsg (logHistory [0]);

			List<string> extracted = logHistory.ToList ();
			extracted.RemoveAt (0);
			addLogMove ( extracted.ToArray ());
		}


		#region Parsing incoming msg
		public override void addLogMove (string[] logMsg){
			base.addLogMove (logMsg);

			foreach (string s in logMsg)
				if (s.Contains ("Crash"))
					parseCrashMsg (s);
				else
					parseNormalMove (s);
			displayLastUpdate ();
		}


		private void parseCrashMsg(string msg){
			foreach (string s2 in msg.Split('#')) {
				if (s2.Trim ().Length == 0)
					continue;
				string[] words = s2.Split (' ');

				boardRenderer.displayCrash (new Vector2 (int.Parse (words [2]), int.Parse (words [3])));
			}
		}

		private void parseNormalMove(string msg){
			foreach (string s2 in msg.Split('#')) {
				string[] words = s2.Split (' ');


				playerHistory [extractPlayerIndex (words [0])].addPosition (int.Parse (words [1]), int.Parse (words [2]), int.Parse (words [3]));
			}
		}
		#endregion




		private void displayLastUpdate(){
			/*
			for (int i = 0; i < playerHistory.Length; i++) {
				foreach (Vector2 p in  playerHistory [i].getBody ())
					boardRenderer.visualizeBody (i, p);
				if(playerHistory [i].hasHead())
					boardRenderer.visualizeHead (i, playerHistory [i].getHead ());
			}
			*/
		}

	
		private int extractPlayerIndex(string colorSign){return colorSign == "B" ? 1 : 0;}



		private class Player{
			private List<Vector2> body = new List<Vector2> ();
			private List<int> dir = new List<int>();

			public void addPosition(int x, int y, int dir){
				if (body.Any ((p) => p.x == x && p.y == y))
					return;

				body.Add (new Vector2 (x, y));
				this.dir.Add (dir);
			}


			public List<Vector2> getBody(){return body;}
			public Vector2 getHead(){return body [body.Count - 1];}
			public int getCurrentDir(){return dir [dir.Count - 1];}
			public bool hasHead(){return body.Count > 0;}
		}
	}


}