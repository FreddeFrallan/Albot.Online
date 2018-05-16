using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;
using Barebones.Networking;
using System;
using System.Linq;

public class AlbotActiveUsers{

	private static Dictionary<UserAcount, ActiveUser> users = new Dictionary<UserAcount, ActiveUser> (); 

	public static bool checkIfUserIsActive(UserAcount user){
		return users.ContainsKey (user);
	}

	public static bool checkIPeerIsActive(IPeer peer){
		return users.Where (x => x.Value.peer == peer).FirstOrDefault ().Value != null;
	}

	public static void registerLogedInUser(UserAcount user, IPeer peer){
		if (users.ContainsKey (user))
			throw new Exception ("Trying to register already registered user!");

		users.Add (user, new ActiveUser (user, peer));
	}

	public static void registerLogedOutUser(IPeer peer){
		try{
			ActiveUser user = users.Where (x => x.Value.peer == peer).FirstOrDefault ().Value;
			users.Remove(user.userAcount);
		}
		catch{
			Debug.LogError ("Tried to remove user that was not registred as active!");
		}
	}

	private class ActiveUser{
		public UserAcount userAcount;
		public IPeer peer;

		public ActiveUser(UserAcount userAcount, IPeer peer){
			this.userAcount = userAcount;
			this.peer = peer;
		}
	}
}
