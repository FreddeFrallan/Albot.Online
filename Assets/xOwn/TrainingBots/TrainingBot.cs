using System.Threading;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Game{

	public abstract class TrainingBot{
		public void initClientController(ClientController c) {theClientController = c;}
		protected ClientController theClientController;
		protected PlayerColor botColor;
		protected Thread moveThread;

		protected abstract void playMove (string input);
		public abstract void initBot (Dictionary<string, string> settings);

		public abstract List<Dictionary<string, string>> botSettings();
		public abstract int defaultSettings ();


		public virtual void onReceiveInput (string input){
			moveThread = new Thread(() => playMove(input));
			moveThread.Start ();
		}

		public void initColor(PlayerColor color){
			this.botColor = color;
		}

		public void killBot(){
			moveThread.Abort ();
		}
	}

}