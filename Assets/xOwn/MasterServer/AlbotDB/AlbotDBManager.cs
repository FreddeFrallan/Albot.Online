using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteDB;
using System.Linq;

namespace AlbotDB{
	
	public class AlbotDBManager{

		private static LiteDatabase theDB;
		private static LiteCollection<UserLoginInformation> userLoginColl;
		private static LiteCollection<UserInfo> userGameStatColl;
		private static LiteCollection<UserChatLog> userChatLogColl;

        /*
         * For some reason we can't store active users with ID 0 ????
         * Hence the quick fix was to make the key a string instead 
         */
        private static LiteCollection<ActiveUser> activeUsersColl;

		public static void initDB(){
			theDB = new LiteDatabase (@Application.dataPath + "\\" + AlbotDBDatatypes.DBPATH);

			userLoginColl = theDB.GetCollection<UserLoginInformation> (AlbotDBDatatypes.LOGINCOLECTION);
			userGameStatColl = theDB.GetCollection<UserInfo> (AlbotDBDatatypes.USERCOLECTION);
			userChatLogColl = theDB.GetCollection<UserChatLog> (AlbotDBDatatypes.CHATCOLECTION);
			activeUsersColl = theDB.GetCollection<ActiveUser> (AlbotDBDatatypes.ACTIVEUSERCOLECTION);
		}


		//Should later be rewritten so we separate a update and a new entry. For opting the performance
		//Also use Generics/Polymorphism
		#region Setters
		public static void updateLoginInfo(UserLoginInformation update){
			UserLoginInformation loginAttempt = userLoginColl.FindOne (x => x.username == update.username);
			if (loginAttempt == null)
				userLoginColl.Insert (update);
			else
				userLoginColl.Update (update);
		}
		public static void updateGameStat(UserInfo update){
			UserInfo loginAttempt = userGameStatColl.FindOne (x => x.username == update.username);
			if (loginAttempt == null)
				userGameStatColl.Insert (update);
			else
				userGameStatColl.Update (update);
		}
		public static void updateChatLog(UserChatLog update){
			UserChatLog loginAttempt = userChatLogColl.FindOne (x => x.username == update.username);
			if (loginAttempt == null)
				userChatLogColl.Insert (update);
			else
				userChatLogColl.Update (update);
		}
		#endregion



		#region Getters
		public static bool getLoginInfo(string username, out UserLoginInformation user){
			UserLoginInformation loginAttempt = (UserLoginInformation)userLoginColl.FindOne (x => x.username == username);
			user = loginAttempt;
			if (loginAttempt == null)
				return false;
			return true;
		}
		public static bool getUserData(string username, out UserInfo user){
			UserInfo loginAttempt =  (UserInfo)userGameStatColl.FindOne (x => x.username == username);
			user = loginAttempt;
			if (loginAttempt == null)
				return false;
			return true;
		}
		public static bool getChatLog(string username, out UserChatLog user){
			UserChatLog loginAttempt =  (UserChatLog)userChatLogColl.FindOne (x => x.username == username);
			user = loginAttempt;
			if (loginAttempt == null)
				return false;
			return true;
		}
		#endregion


		#region Active users
		public static void onUserLogedIn(UserLoginInformation user, int peerID){
			user.isLoggedIn = true;
			updateLoginInfo (user);
			addActiveUser (user, peerID);
		}

		private static void incrementLoginCounter(UserLoginInformation user){
			UserInfo info;
			if (getUserData (user.username, out info) == false)
				return;

			int currentLogins;
			if (int.TryParse (info.logins, out currentLogins) == false)
				return;
			info.logins = (currentLogins + 1).ToString();
			updateGameStat (info);
		}

		//Right now the peerID and username search makes no sense... Fix later
		//To error check we could later store the peerID -> Username somewhere
		private static void addActiveUser(UserLoginInformation user, int newPeerID){
			ActiveUser newUser = activeUsersColl.FindOne (x => x.username == user.username);
			if (newUser == null) {
				activeUsersColl.Insert (new ActiveUser{ username = user.username, peerId = newPeerID.ToString() });
				Debug.LogError ("Adding user with ID: " + newPeerID);
			}
			else { //This should never happen
				newUser.peerId = newPeerID.ToString();
				activeUsersColl.Update (newUser);
			}
		}

		public static void onPlayerDissconnected(int peerID){
            string keyID = peerID.ToString();
			ActiveUser oldUser = activeUsersColl.FindOne (x => x.peerId == keyID);
			if (oldUser != null) {
				Debug.LogError ("Removing for id: " + peerID);
				activeUsersColl.Delete (oldUser.peerId);
				UserLoginInformation savedUser = userLoginColl.FindOne (x => x.username == oldUser.username);
				if (savedUser != null) {
					savedUser.isLoggedIn = false;
					updateLoginInfo (savedUser);
				}
			}
			else
				Debug.LogError("Player dissconnected but was not loged in");
		}


		public static bool getActiveUsersInfo(int peerID, ref UserInfo info){
            string keyID = peerID.ToString();
			ActiveUser activeUser = activeUsersColl.FindOne (x => x.peerId == keyID);
			if (activeUser != null) 
				return getUserData (activeUser.username, out info);

			Debug.LogError("Could not find a active user with matching Peer ID: " + peerID);
            return false;
		}

		//Use this with caution!
		//Kind of only suitable on reboot of the server
		public static void clearActiveUsers(){
			if(theDB.CollectionExists(AlbotDBDatatypes.ACTIVEUSERCOLECTION))
				theDB.DropCollection(AlbotDBDatatypes.ACTIVEUSERCOLECTION);
		}

		public static void resetLoggedInUsers(){
			if (theDB.CollectionExists (AlbotDBDatatypes.LOGINCOLECTION))
				foreach (UserLoginInformation u in userLoginColl.FindAll()) {
					u.isLoggedIn = false;
					updateLoginInfo (u);
				}
		}
		#endregion


		public static void shutdownGameServer(){AlbotGameTerminator.onGameFinished ();}
	}

}