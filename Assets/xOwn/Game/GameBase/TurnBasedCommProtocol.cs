using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using AlbotServer;

namespace Game{

	public abstract class TurnBasedCommProtocol : CommProtocol{

		public void initTurnBasedCommProtocol(){
			currentProtocol.Add (new AlbotMessage (typeof(PlayerTimerMsg), (short)ServerCommProtocl.PlayerTimerInit));
			currentProtocol.Add (new AlbotMessage (typeof(PlayerTimerMsg), (short)ServerCommProtocl.PlayerTimerCommand));
		}

		public void sendPlayerTimerInit(int targetID, PlayerTimerMsg ptm) {sendMsg(ptm, targetID, (short)ServerCommProtocl.PlayerTimerInit);}
		public void sendPlayerTimerCommand(int targetID, PlayerTimerMsg ptm) {sendMsg(ptm, targetID, (short)ServerCommProtocl.PlayerTimerCommand);}
	}
	[Serializable]
	public class PlayerTimerMsg{
		public Game.PlayerColor color;
		public float maxTime;
		public bool startTimer;
	}	
}