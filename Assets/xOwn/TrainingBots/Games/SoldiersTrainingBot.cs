using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

namespace Soldiers{

	public class SoldiersTrainingBot: Game.TrainingBot {

		private int updateCounter = 0;
		private bool hasNewInput = false;
		private string lastInput;
		private bool isPlaying = true;

		protected override void playMove (string input){
			while (isPlaying) {
				Thread.Sleep (500);
				if (hasNewInput == false)
					continue;

				hasNewInput = false;
				makeDecision ();
				Game.RealtimeTCPController.requestBoard (SoldiersGameController.convertColorToTeam (botColor));
			}
		}

		private void makeDecision(){
			if (updateCounter++ % 10 != 0)
				return;
			
			List<TCPCommand> playerCommands = SoldiersTCPProtocol.convertMsgToCommands (createAutoAttackMsg (lastInput));
			PlayerCommands outMsg = new PlayerCommands (botColor, playerCommands);
			theClientController.onOutgoingLocalMsgObj (outMsg, (short)SoldiersProtocol.MsgType.playerCommands);
		}

		public override void initBot (Dictionary<string, string> settings){

		}

		public override List<Dictionary<string, string>> botSettings (){
			return new List<Dictionary<string, string>> (){ new Dictionary<string, string>() {{"Name", "AutoAttack"}} };
		}

		public override int defaultSettings (){
			return 0;
		}

		public override void onReceiveInput (string input){
			if (input == "GameOver")
				isPlaying = false;

			hasNewInput = true;
			lastInput = input;
			if (updateCounter == 0) {
				moveThread = new Thread(() => playMove(input));
				moveThread.Start ();
			}
		}


		private string createAutoAttackMsg(string board){
			string autoAttackMsg = "";
			JSONObject jObj = new JSONObject (board);
			JSONObject units = jObj.GetField("Units");

			foreach (string key in units.keys)
				autoAttackMsg += ":" + key + ",autoattack";

			return autoAttackMsg;
		}

	}
}