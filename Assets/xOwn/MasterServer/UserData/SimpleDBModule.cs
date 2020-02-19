using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;
using Barebones.Networking;
using LiteDB;


public class SimpleDBModule : ServerModuleBehaviour {

	/*
	public override void Initialize (IServer server)
	{
		base.Initialize (server);
		Debug.LogError ("Init costum DB Module, WOW");


		LiteDatabase db = new LiteDatabase (@Application.dataPath + "\\" + LOGININFORMATIONDBOATH);
		Debug.LogError ("Saving DB at: " + (Application.dataPath + "\\" + LOGININFORMATIONDBOATH));

		LiteCollection<UserLoginInformation> users;

		if (db.CollectionExists (COLLECTIONNAME)) {
			Debug.LogError ("Found old loginInformation DB");
			users = db.GetCollection<UserLoginInformation> (COLLECTIONNAME);
			Debug.LogError(users.FindById ("TestUser").password);
			db.DropCollection (COLLECTIONNAME);
			print (users.Count());
		} else {
			Debug.LogError ("It doea not exist");
		}


		UserLoginInformation newUser = new UserLoginInformation {
			username = "TestUser",
			password = "1234",
		
		};
		users = db.GetCollection<UserLoginInformation> (COLLECTIONNAME);
		users.Insert (newUser);
		//numbers.Insert (42);

		print (users.Count());
	}
*/

}
