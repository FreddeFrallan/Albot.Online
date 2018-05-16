using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Game;
using System.Linq;

namespace Bomberman{

	public class BombermanGameStateUpdater : MonoBehaviour {

		public static BombermanGameStateUpdater singleton;
		private static int objIDCounter = 2;

		public List<BombermanMapObject> gameObjects = new List<BombermanMapObject>();
		public List<BoardAction> roundActions = new List<BoardAction>();
		public BombermanProtocol protocol;
		public BombermanGameMaster gameMaster;
		private List<ConnectedPlayer> players;
		private bool gamePlaying;
		private float refreshRate = 0.1f;
		private int updateID = 0;

		public void startGame(List<ConnectedPlayer> players){
			singleton = this;
			protocol = gameMaster.protocol;
			gamePlaying = true;
			this.players = players;

			StartCoroutine (updateGameStatus ());
			broadcastBoard(protocol.sendBoardInit);
		}

		private IEnumerator updateGameStatus(){
			while (gamePlaying) {
				broadcastBoard (protocol.sendBoardUpdate);
				updateID++;
				yield return new WaitForSeconds (refreshRate);
			}
		}

		public void sendFinalBoard(){	
			updateID++;
			broadcastBoard (protocol.sendBoardUpdate);
		}

		private void broadcastBoard(Action<int, BoardUpdate> sendFunc){
			List<MapObj> currentMap = formatMapObjects ();
			List<BoardAction> currentActions = flushBoardActions ();
			BoardUpdate b = new BoardUpdate(currentMap, currentActions, PlayerColor.None, updateID);

			foreach (ConnectedPlayer p in players) {
				try{
					b.color = p.color;
					sendFunc(gameMaster.getMatchingPlayer(p.color).client.peerID, b);
				}catch(Exception e){Debug.LogError ("Failed Send: " + e.Message);}
			}
		}
			

		public void setGameOver(){
			gamePlaying = false;
		}


		private List<MapObj> formatMapObjects(){
			List<MapObj> currentMap = new List<MapObj>();
			foreach (BombermanMapObject obj in gameObjects) 
				currentMap.Add (obj.convertToMapObj());
			
			return currentMap;
		}

		private List<BoardAction> flushBoardActions(){
			List<BoardAction> tempActions = roundActions.ToList ();
			roundActions.Clear ();
			return tempActions;
		}

		public static void removeObj(int id){
			BombermanMapObject targetObj = singleton.gameObjects.Find (x => x.ID == id);
			if (targetObj != null)
				singleton.gameObjects.Remove (targetObj);
		}
			

		public static PlayerController getMatchingPlayerObj(int id){
			return singleton.gameObjects.Find (x => x.ID == id) as PlayerController;
		}

		public static void removeObj(BombermanMapObject obj){singleton.gameObjects.Remove(obj);}
		public static void addNewObj(BombermanMapObject obj){singleton.gameObjects.Add (obj);}
		public static void addNewAction(BoardAction newAction){singleton.roundActions.Add (newAction);}
		public static int generateNewID(){return objIDCounter++;}
	}


}