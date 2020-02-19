using System;
using LiteDB;
using AlbotDB;
using System.IO;
using System.Reflection;

namespace AlbotDBManager
{
	class MainClass
	{
		private static LiteDatabase theDB;
		private static LiteCollection<UserLoginInformation> userLoginColl;
		private static LiteCollection<UserInfo> userGameStatColl;
		private static LiteCollection<UserChatLog> userChatLogColl;
		private static LiteCollection<ActiveUser> activeUsersColl;
		private static Random random = new Random();
		private static string executePath = "";

		//Fix so that the DB does not have to be in the same folder
		private static bool initDB(){
			executePath = Directory.GetCurrentDirectory ();
			string filePath = executePath + "\\" + AlbotDBDatatypes.DBPATH;

			if (File.Exists (filePath) == false) {
				Console.WriteLine ("Could not find DB at: " + filePath);
				Console.ReadLine ();
				return false;
			}

			theDB = new LiteDatabase (filePath);

			userLoginColl = theDB.GetCollection<UserLoginInformation> (AlbotDBDatatypes.LOGINCOLECTION);
			userGameStatColl = theDB.GetCollection<UserInfo> (AlbotDBDatatypes.USERCOLECTION);
			userChatLogColl = theDB.GetCollection<UserChatLog> (AlbotDBDatatypes.CHATCOLECTION);
			activeUsersColl = theDB.GetCollection<ActiveUser> (AlbotDBDatatypes.ACTIVEUSERCOLECTION);
			return true;
		}

		public static void Main (string[] args){
			if (initDB () == false)
				return;
			
			int menuInput = takeMenuInput ();
			while (menuInput != 0) {
				Console.Clear ();
				switch (menuInput) {
				case 1:
					addPlayer ();
					break;
				case 2:
					removePlayer ();
					break;
				}
				menuInput = takeMenuInput ();
			}
		}

		private static int takeMenuInput(){
			Console.WriteLine ("0. Quit");
			Console.WriteLine ("1. add player");
			Console.WriteLine ("2. remove player");
			Console.Write ("What do you wish to do: ");

			int tempInput = 2;
			while (!int.TryParse (Console.ReadLine (), out tempInput) || tempInput < 0 || tempInput > 2)
				Console.Write ("\nInvalid command, try again: ");
			return tempInput;
		}

		private static void removePlayer(){
			string name = "";

			Console.Write ("\nName of player to remove: ");
			name = Console.ReadLine();
			if (userLoginColl.FindOne (x => x.username == name) == null) {
				Console.WriteLine ("Could not find any player named \"" + name);
				return;
			}

			Console.WriteLine ("Are you sure you wish to remove \"" + name + "\"? (y/n) ");
			if (Console.ReadLine ().ToLower () == "y") {
				userLoginColl.Delete (name);
				Console.WriteLine("Succesfully removed login credentials for: \"" + name + "\"");

				if (userGameStatColl.FindOne (x => x.username == name) != null) {
					userGameStatColl.Delete (name);
					Console.WriteLine("Succesfully removed game data for: \"" + name + "\"");
				}
				else
					Console.WriteLine("Could not find and remove game data for: \"" + name + "\"");
			}
		}

		private static void addPlayer(){
			bool validInput = false;
			string name = "";
			string pw = "";

			Console.Write ("\nName of new player: ");
			name = Console.ReadLine();
			if (userLoginColl.FindOne (x => x.username == name) != null) {
				Console.WriteLine ("A player named \"" + name + "\" already exists!");
				return;
			}
				
			while (validInput == false) {
				Console.Write ("\nNew password: ");
				pw = Console.ReadLine();

				if (pw.Length < 4 || pw.Length > 15) {
					Console.WriteLine ("Invalid password format!");
					continue;
				}
				validInput = true;
			}

			int profilePic = random.Next (1, 100);
			userLoginColl.Insert (new UserLoginInformation{ username = name, password = pw, isLoggedIn = false });
			userGameStatColl.Insert (new UserInfo{username = name, roomActions = "0", logins = "0", profilePic = profilePic.ToString()});

			Console.WriteLine ("\nSuccesfully added new player: \"" + name +"\" with password: \"" + pw +"\" and profile pic: " + profilePic);
		}
	}
}
