using System.Collections.Generic;
using System.Linq;
using Barebones.MasterServer;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using System.Reflection;
using System.IO;
using System.Collections;

namespace AlbotServer{

	public class AlbotNetworkManager : NetworkManager{

		public static event Action<ConnectedClient> onClientLeft;
		private static Game.GameMaster gameController;
		public static Game.GameType currentGameType;
		private GameWrapper wrapper = new GameWrapper ();
		private ConnectionManager connManager;

		private List<string> preGamePlayers = new List<string>();

		//Small workaround that sometimes the player properties are not loaded before the first player connects.
		//Therefore we force all inits to wait for  AlbotGameRoom.SpawnTaskController != null
		//Both waitForPropertisToLoad && waitForPlayersInit is waiting for this.
		private struct newConnection{public NetworkConnection conn;public short playerControllerId;}
		private List<newConnection> newConnQ = new List<newConnection>();
		private bool playersInited = false;

		void Awake(){
			Msf.Connection.SetHandler((short)ServerCommProtocl.CheckCurrentVersion, ((Barebones.Networking.IIncommingMessage message) => {
				Debug.LogError("Got version msg");
			}));
		}

		public override void OnStartServer(){
			Debug.LogError ("Server alive and well!");
			StartCoroutine (waitForPropertisToLoad ());
			StartCoroutine(waitForPlayersInit());
			GameRoomClients.init ();
			Instantiate(Resources.Load<GameObject>("Prefabs/ServerEntity"));
			Msf.Connection.AddConnectionListener (initServerGameLogic, true); //We tell server to run "loadServerGameLogic" when we establish the connection or if we are connected now
		}



		#region get playerInfo
		private IEnumerator waitForPropertisToLoad(){
			while (AlbotGameRoom.SpawnTaskController == null)
				yield return new WaitForEndOfFrame ();
			extractPreGamePlayers ();
			playersInited = true;
		}
		private void extractPreGamePlayers(){
			var prop = AlbotGameRoom.SpawnTaskController.Properties;
			for (int i = 0; i < 2; i++)//Extract player data
				if (prop.ContainsKey ("p" + i))
					preGamePlayers.Add (prop ["p" + i]);
				else
					break;
		}
		#endregion



		private void initServerGameLogic(){
			if (bool.Parse (Msf.Args.RealTime)) 
				initUnityBasedGame ();
			else {
				initDLLBasedGame ();
				initManagers ();
				StartCoroutine (turnsTimer ());
			}
		}
		private void initManagers(){
			connManager = GetComponent<ConnectionManager> ();
			connManager.init (gameController, currentGameType);
		}

			
		private void initDLLBasedGame(){
            Debug.LogError("Initing DLL based game");
			DllManager.loadServerGameLogic(ref wrapper, ref gameController, ref currentGameType, preGamePlayers);
		}
		private IEnumerator turnsTimer(){
			Game.TurnbasedGame turnBasedController = (Game.TurnbasedGame)gameController;
			while (true) {
				yield return new WaitForSeconds (0.1f);
				turnBasedController.incrementTimer ();
			}
		}




		#region RealTimeGames
		private void initUnityBasedGame(){
			DontDestroyOnLoad (gameObject);
			Debug.LogError ("Starting load wait");
			SceneManager.sceneLoaded += onSceneLoaded;
			SceneManager.LoadScene (Msf.Args.LoadScene + "Server", LoadSceneMode.Additive);
		}

		private void onSceneLoaded(Scene newScene, LoadSceneMode mode){
			Debug.LogError ("Load finished");

			gameController = GameObject.FindGameObjectWithTag ("ServerController").GetComponent<Game.ServerController>().getController();
			wrapper.init (gameController.getProtocol (), gameController);
			currentGameType = gameController.getGameType ();

			Action<string> printFunc = DllManager.printMsg;
			Action<object, int, short> networkMsg = wrapper.sendMsg;
			gameController.init (printFunc, networkMsg, shutdownGameServer, wrapper, preGamePlayers);
			onClientLeft += wrapper.clientLeft;
			initManagers ();
		}
		#endregion
			


		public override void OnServerDisconnect(NetworkConnection conn){
			ConnectedClient c = GameRoomClients.getMatchingClient (conn);
			Debug.LogError ("Some shit dissconnected yo");

			if(onClientLeft != null)
				onClientLeft.Invoke (c);
			GameRoomClients.clientDisconnected (c);
		}

		//AlbotGameRoom "HandleReceivedAccess" will also run, where we add the new Conn to GameRoomClients.
		public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
			if (playersInited == false) {
				newConnQ.Add(new newConnection{conn = conn, playerControllerId = playerControllerId});
				return;
			}

			GameObject playerObj = (GameObject)Instantiate(Resources.Load<GameObject>("Prefabs/PlayerEntity"));
			NetworkServer.AddPlayerForConnection(conn, playerObj.gameObject, playerControllerId);
			connManager.OnServerAddClient (conn);
		}
		private IEnumerator waitForPlayersInit(){
			while (playersInited == false)
				yield return new WaitForEndOfFrame ();
			newConnQ.ForEach(x => OnServerAddPlayer(x.conn, x.playerControllerId));
		}

		public static void shutdownGameServer(){AlbotGameTerminator.onGameFinished ();}
	}
}