using Barebones.MasterServer;
using UnityEngine;
using System.IO;
using System.Reflection;
using System;
using System.Collections.Generic;
using Game;

namespace AlbotServer{
	public class DllManager{
		
		/* Loads the approriate gamelogic. The gameMaster file should be located in the "GameLibrariesFolder/ *GameName* /main.dll"
		* Important that the namespace is "Main" & The class name "GameMaster"
		*/
		public static void loadServerGameLogic(ref GameWrapper wrapper, ref GameMaster gameController, ref GameType gameType, List<string> preGamePlayers){
			if (!Msf.Args.IsProvided(Msf.Args.LoadScene)){
				Debug.LogError("A scene to load was not provided");
				return;
			}

			//Go to the correct folder and get dll! Should be located in "GameLibraries/*GameName*/main.dll"
			string dllFolder = Application.dataPath.Substring (0, Application.dataPath.Length - 15);
			dllFolder += "GameLibraries/" + Msf.Args.LoadScene + "/";
			string filePath = dllFolder +"Main.dll";


			FileInfo dllFile = new FileInfo (filePath);
			if (dllFile.Exists == false) {
				Debug.LogError("Could not find approprate dll file!\n " + filePath);
				return;
			}

			//Create an instance. Must be named "GameMaster" and in "Main" namespace
			Assembly loadedFile = Assembly.LoadFile (dllFile.FullName);
			Type testType = loadedFile.GetType ("Main.GameMaster");
			object testInstance = Activator.CreateInstance (testType, null);

			//This is where the magic happens! We call getInstance wich returns a GameMaster object.
			MethodInfo useFunc = testType.GetMethod ("getInstance");
			gameController = (Game.GameMaster)useFunc.Invoke (testInstance, null);
			wrapper.init (gameController.getProtocol (), gameController);
			gameType = gameController.getGameType ();
			//Debug.LogError ("CurrentGame: " + gameType);

			//We give the Dll some functions so that it can interact with server
			Action<string> printFunc = printMsg;
			Action<object, int, short> networkMsg = wrapper.sendMsg;
			gameController.init (printFunc, networkMsg, AlbotNetworkManager.shutdownGameServer, wrapper, preGamePlayers);

			AlbotNetworkManager.onClientLeft += wrapper.clientLeft;
		}

		//Function for letting the GameController print stuff
		public static void printMsg(string gameControlleMsg){
            //Debug.LogError ("Dll msg:: " + gameControlleMsg);
        }
	}

}
