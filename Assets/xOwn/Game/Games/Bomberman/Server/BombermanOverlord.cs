using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace Bomberman{

	public class BombermanOverlord : MonoBehaviour {

		public static BombermanOverlord singleton;
		public BombermanGameMaster gameMaster;
		public BombermanGameStateUpdater updater;
		public List<PlayerController> players = new List<PlayerController>();
		public GameBoard currentMap;
		public bool gameOver = false;


		void Start(){
			singleton = this;
			currentMap.Init ();

			foreach (PlayerController p in players)
				updater.gameObjects.Add (p as BombermanMapObject);
		}
			
		void Update(){
			if (gameOver)
				return;
			setCurrentGrid ();
			victoryCheck ();
		}

		private void setCurrentGrid(){
			foreach (PlayerController p in players) {
				int[] coord = GameBoard.getCurrentGridPos (p.transform.localPosition);
				p.setCurrentCoord (coord);
				currentMap.updatePlayerPos (coord, p.lastCoord, p);
			}
		}


		public static void playerDied(PlayerController p){
			singleton.players.Remove (p);
			BombermanGameStateUpdater.addNewAction (new BoardAction (ActionType.PlayerDeath, p.ID, p.getFloatPos()));
			BombermanGameStateUpdater.removeObj (p as BombermanMapObject);
		}

		private void victoryCheck(){
			if (singleton.players.Count <= 1){
				singleton.updater.setGameOver ();
				singleton.gameOver = true;

				PlayerColor winColor = singleton.players.Count > 0 ? singleton.players [0].color : PlayerColor.None;
				gameMaster.onGameOver (winColor);
			}
		}


		public static void moveHandler(object msg, ConnectedClient c){
			PlayerCommand cMsg;

			try{cMsg = (PlayerCommand)msg;
			}catch{return;}

			int playerIndex = convertColorToTeam (cMsg.color);
			if(cMsg.moveDir >= 0)
				singleton.players [playerIndex].initMovePlayer (intToMoveDir(cMsg.moveDir));
			if (cMsg.dropBomb)
				singleton.players [playerIndex].placeBomb ();
		}



		public static MoveDirections intToMoveDir(int dir){
			if (dir == 0)return MoveDirections.Right;
			if (dir == 1)return MoveDirections.Up;
			if (dir == 2)return MoveDirections.Left;
			return MoveDirections.Down;
		}
		public static float[] vecToFlArray(Vector3 vec){return new float[]{ vec.x, vec.y, vec.z };}
		public static Vector3 FArrayToVec(float[] pos){return new Vector3(pos[0], pos[1], pos[2]);}
		public static int convertColorToTeam(PlayerColor color){	return color == PlayerColor.Blue ? 0 : 1;}
	}
}