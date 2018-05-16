using System;
using System.Collections.Generic;
using Battleship;

namespace BattleshipGame
{
	public class Player
	{
		public ConnectedPlayer playerInfo;

		private Board ownBoard = new Board();
		private Player oponent;
	 

		private int hitsLeft = 17;
		private bool boardInit = false;
		public bool isReady(){	return boardInit;}


		public Player(ConnectedPlayer p){
			playerInfo = p;
		}
			
		public FireResult fireAtOponent(int target){
			FireResult result = oponent.fireAtPlayer (target);
			return result;
		}


		public FireResult fireAtPlayer(int target){
			ShipPlacement potentialShip = new ShipPlacement(ShipType.battle, 0, new List<int[]>(), false); //Just random Values assign
			char postTargetSign = ownBoard.fireAtBoard (target, ref potentialShip);
			FireType result = FireType.miss;
			ShipType shipHit = ShipType.battle;

			if (postTargetSign == GameConstants.HITCHAR || postTargetSign == GameConstants.SUNKCHAR) {// We got a hit
				hitsLeft--;
				result = postTargetSign == GameConstants.HITCHAR ? FireType.hit : FireType.sunk;
			}

		//	Main.GameMaster.singleton.debugPrint (playerInfo.username + " lives: " + hitsLeft);
			if (hitsLeft == 0)
				death ();
			
			return new FireResult(result, ownBoard.generateEnigmaBoard (), potentialShip, target);
		}
			
		private void death(){Main.GameMaster.singleton.setGameOver (oponent.playerInfo);}



		public string getFormatedBoard(){return ownBoard.formatBoard ();}
		public char[,] getUnformatedBoard(){return ownBoard.charBoard;}
		public char[,] getUnformatedTargetBoard(){return oponent.ownBoard.generateEnigmaBoard();}
			
		public void initBoard(char[,] startBoard){
			ownBoard.charBoard = startBoard;
			boardInit = true;
		}
		public void initOponent(Player op){oponent = op;}
		public void initShips(List<ShipPlacement> ships){	ownBoard.ships = ships;}

		public static char fireTypeToChar(FireType type){
			if (type == FireType.miss)return GameConstants.MISSCHAR;
			else if (type == FireType.hit)return GameConstants.HITCHAR;
			else return GameConstants.SUNKCHAR;
		}
	}

	public class FireResult{
		public FireType statusType;
		public ShipPlacement ship;
		public char[,] postBoard;
		public int target;
		public FireResult(FireType statusType, char[,] ShipPlacement, ShipPlacement ship, int target){
			this.statusType = statusType; this.ship = ship; this.postBoard = postBoard; this.target = target;
		}
	}
	public enum FireType{
		miss,
		sunk,
		hit,
	}
}

