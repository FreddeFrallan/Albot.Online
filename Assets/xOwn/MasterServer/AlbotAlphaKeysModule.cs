using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Barebones.Networking;
using Barebones.MasterServer;

public class AlbotAlphaKeysModule{
	private class AlphaTesterData{
		public int logins, actions;
		public AlphaTesterData(int logins, int actions){this.logins = logins; this.actions = actions;}
	}
	private static Dictionary<string, AlphaTesterData> storedUsers = new Dictionary<string, AlphaTesterData> ();
	private static List<string> keys;

	public static void readKeys(){
		keys = new List<string> (){ "Pingul", "Zully"};
		try{
			string filePath = Application.dataPath + "/Keys.txt";
			string[] lines = File.ReadAllLines (filePath);
			foreach (string l in lines)
				keys.Add (l.Trim ());
		}
		catch{}
	}


	public static bool requstLogin(string key){
		readKeys ();
		if (keys.Contains (key) == false)
			return false;

		if(storedUsers.ContainsKey(key))
			storedUsers[key].logins++;
		else
			storedUsers.Add(key,  new AlphaTesterData(1, 0));
		saveLoginData(key, storedUsers[key]);

		return true;
	}

	public static void handleRoomAccessRequst(IPeer peer){
		try{
			string key = AuthModule.singleton.getKeyFromUser (peer);
			if(storedUsers.ContainsKey(key))
				storedUsers[key].actions++;
			else
				storedUsers.Add(key,  new AlphaTesterData(1, 1));
			saveLoginData(key, storedUsers[key]);
		}catch{}
	}


	private static void saveLoginData(string key, AlphaTesterData user){
		string filePath = Application.dataPath + "/" + key + ".txt";
		string saveString = "Logins: " + user.logins + "  RoomActions: " + user.actions;
		File.WriteAllText (filePath, saveString);
	}
}
