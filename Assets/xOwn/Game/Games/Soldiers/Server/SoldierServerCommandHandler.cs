using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Soldiers{

	public class SoldierServerCommandHandler{

		public static void moveHandler(object msg, ConnectedClient c){
			PlayerCommands cMsg;
			try{cMsg = (PlayerCommands)msg;
			}catch{return;}


			int team = cMsg.color == Game.PlayerColor.Blue ? 1 : 2;
			foreach(TCPCommand command in cMsg.commands)
				activateCommand (command, team);
		}


		private static void activateCommand(TCPCommand command, int team){
			List<Soldier> aliveSoldiers = GameOverlord.getEnemyList (team == 1 ? 2 : 1);

			//Could be optimized using dicts
			Soldier s = aliveSoldiers.Find (x => x.id == command.soldierId);
			if (s == null)
				return;


			switch (command.type) {
			case TCPCommandType.attack:activateAttack (s, command.followData[0], team);break;
			case TCPCommandType.autoAttack:s.startAutoAttack();break;
			case TCPCommandType.move:activateMove(s, command.followData[0], command.followData[1]);break;
			}
		}



		private static void activateAttack(Soldier s, float targetId, int team){
			int target = (int)targetId;
			Soldier targetSoldier = GameOverlord.getEnemyList(team).Find (x => x.id == target);
			if (targetSoldier != null) 
				s.setAttackTarget (targetSoldier);
		}


		private static void activateMove(Soldier s, float moveTargetX, float moveTargetY){
			moveTargetX = Mathf.Clamp (moveTargetX, GameOverlord.minPos[0],  GameOverlord.maxPos[1]);
			moveTargetY = Mathf.Clamp (moveTargetY, GameOverlord.minPos[0],  GameOverlord.maxPos[1]);
			s.setMovingTarget (new Vector2 (moveTargetX, moveTargetY));
		}



	}

}