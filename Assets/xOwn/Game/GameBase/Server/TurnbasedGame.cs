using UnityEngine;
using AlbotServer;
using System;
using System.Collections.Generic;

namespace Game {

	public abstract class TurnbasedGame : GameMaster {
		protected int mPlayerIt;

		public TurnbasedGame() { mPlayerIt = 0; }
		public ConnectedPlayer currentPlayer() {
			return players.Find(x => x.color == colorOrder[mPlayerIt]);
		}
		public void nextPlayer() { 
			mPlayerIt = (mPlayerIt + 1) % nbrPlayers();
			moveTimer = 0;
		}

		#region Timers
		private bool checkTime = false;
		public abstract float maxTimePerMove();
		public abstract void onTimesOut ();
		protected float moveTimer = 0, lastTime = 0;
		public void startTimer(){lastTime = Time.time; checkTime = true;}
		public void incrementTimer (){
			if (checkTime == false)
				return;
			
			float tempTime = Time.time;
			float dt = tempTime - lastTime;
			lastTime = tempTime;
			moveTimer += dt;

			if (moveTimer > maxTimePerMove ()) {
				checkTime = false;
				onTimesOut ();
			}
		}

		//Currently re sending alot of data. Should be opted lated
		public void initPlayerTimer(float startTime, Game.PlayerColor initColor, TurnBasedCommProtocol protocol){
			foreach (ConnectedClient c in clients)
				foreach (ConnectedPlayer p in players) {
					PlayerTimerMsg  msg2 = new PlayerTimerMsg (){ maxTime = startTime, color = p.color, startTimer = false };
					protocol.sendPlayerTimerInit(c.peerID, msg2);
				}
		}
		public void broadcastStartTimer(Game.PlayerColor playerColor, float startTime, TurnBasedCommProtocol protocol){
			PlayerTimerMsg  msg = new PlayerTimerMsg (){ maxTime = startTime, color = playerColor, startTimer = true };

			foreach (ConnectedClient c in clients)
				protocol.sendPlayerTimerCommand(c.peerID, msg);
		}
		#endregion
	}

} // namespace Game
