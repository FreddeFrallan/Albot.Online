using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using System.Threading;
using System;
using BombermanBot;

namespace Bomberman{

	public class BombermanTrainingBot : TrainingBot {

		private int updateCounter = 0;
		private bool hasNewInput = false;
		private string lastInput;
		private bool isPlaying = true;
		private System.Random r = new System.Random();
		private Action botAction;

		public override void initBot (Dictionary<string, string> settings){
			if(settings["Name"] == "Clever")
				botAction = cleverAction;
			else
				botAction = stupidAction;
		}

		public override int defaultSettings (){return 1;}
		public override List<Dictionary<string, string>> botSettings (){
			return new List<Dictionary<string, string>> () { 
				new Dictionary<string, string> () { { "Name", "Random" } },
				new Dictionary<string, string> () { { "Name", "Clever" } }
			};
		}



		#region Clever bot
		private void cleverAction(){
			if (updateCounter++ == 0) {
				moveThread = new Thread(() => cleverPlayMove());
				moveThread.Start ();
			}
		}


		private void cleverPlayMove(){
			while (isPlaying) {
				Thread.Sleep (100);
				if (hasNewInput == false)
					continue;

				hasNewInput = false;

				bool dropBomb = false;
				int moveDir = -1;


				//Obvious quick fix, we should not have to try catch the DLL....
				try{
					moveDir = BombermanBot.MainClass.getCurrentAction (lastInput, ref dropBomb);
					PlayerCommand outMsg = new PlayerCommand (botColor, moveDir, dropBomb);
					theClientController.onOutgoingLocalMsgObj (outMsg, (short)BombermanProtocol.MsgType.playerCommands);
				
				}catch{}
				RealtimeTCPController.requestBoard (BombermanOverlord.convertColorToTeam (botColor));
			}
		}

		#endregion


		
		#region Random Bot
		private void stupidAction(){
			if (updateCounter++ == 0) {
				moveThread = new Thread(() => playMove(lastInput));
				moveThread.Start ();
			}
		}

		protected override void playMove (string input){
			while (isPlaying) {
				Thread.Sleep (200);
				if (hasNewInput == false)
					continue;
				
				hasNewInput = false;
				makeDecision ();
				RealtimeTCPController.requestBoard (BombermanOverlord.convertColorToTeam (botColor));
			}
		}
		
		private void makeDecision(){
			PlayerCommand outMsg = new PlayerCommand (botColor, r.Next(0, 4));
			theClientController.onOutgoingLocalMsgObj (outMsg, (short)BombermanProtocol.MsgType.playerCommands);
		}
		
		
		public override void onReceiveInput (string input){
			if (input == "GameOver") {
				isPlaying = false;
				return;
			}

			//Debug.Log ("Got input");
			hasNewInput = true;
			lastInput = input;
			botAction ();
		}
		#endregion
	}
}