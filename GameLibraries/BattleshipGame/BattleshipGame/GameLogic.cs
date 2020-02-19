using System;
using System.Collections.Generic;
using Main;

namespace BattleshipGame
{
	public class GameLogic
	{
		private GameMaster master;
		private List<Player> battleShipPlayers = new List<Player>();

		public GameLogic(GameMaster master){this.master = master;}


		public bool playMove(int target, ConnectedPlayer p){
			if (target < 0 || target >= 100)
				return false;

			Player currentPlayer = getMatchingBattleshipPlayer (p.color);
			FireResult result = currentPlayer.fireAtOponent (target);
			master.broadcastRPCMove (result, p);

			return true;
		}
			

		#region init
		public void handleInitMsg(string msg, ConnectedPlayer p){
			//First time player sent init msg, so we create new player
			Player currentPlayer = getMatchingBattleshipPlayer (p.color);
			if (currentPlayer == null) {
				battleShipPlayers.Add (new Player (p));
				currentPlayer = battleShipPlayers [battleShipPlayers.Count - 1];
			}
			else if (currentPlayer.isReady ())
				return;



			char[,] charBoard;
			List<ShipPlacement> ships = new List<ShipPlacement>();
			if (isValidStartBoard (msg, out charBoard, ref ships) == false) {
				master.sendRequstInitBoard (p);
				return;
			}

			currentPlayer.initBoard (charBoard);
			currentPlayer.initShips (ships);
			if (checkIfInitIsFinished () == false)
				master.sendPlayerTwoInitMsg ();
		}


		private bool isValidStartBoard(string startBoard, out char[,] charBoard, ref List<ShipPlacement> ships){
			if (BoardUtils.charBoardFromString (startBoard, out charBoard) == false) {
				return false;
			}
			if (BoardUtils.containsRightAmountOfShips (charBoard, ref ships) == false) {
				return false;
			}
			charBoard = BoardUtils.charBoardFromShipList (ships);
			return true;
		}

		private bool checkIfInitIsFinished(){
			if (battleShipPlayers.Count == master.maxNbrPlayers ()) {
				foreach (Player p in battleShipPlayers)
					if (p.isReady () == false)
						return false;

				//Init everones oponents
				foreach (Player p in battleShipPlayers)
					foreach (Player pl in battleShipPlayers)
						if (p != pl)
							p.initOponent (pl);

				master.startToRunGame ();
				return true;
			}
			return false;
		}
		#endregion



		#region Util functions

		public Player getMatchingBattleshipPlayer(ConnectedPlayer p){
			return battleShipPlayers.Find (x => x.playerInfo.client.peerID == p.client.peerID) ;
		}


		public Player getMatchingBattleshipPlayer(Game.PlayerColor color){
			return battleShipPlayers.Find (x => x.playerInfo.color == color) ;
		}
		#endregion
	}
}

