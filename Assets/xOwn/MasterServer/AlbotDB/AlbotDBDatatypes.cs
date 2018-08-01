using System.Collections;
using System.Collections.Generic;
using LiteDB;

namespace AlbotDB{

	public class AlbotDBDatatypes{
		public static readonly string DBPATH = "AlbotStoredData.db";
		public static readonly string LOGINCOLECTION = "LoginCollection";
		public static readonly string USERCOLECTION = "UserCollection";
		public static readonly string CHATCOLECTION = "ChatCollection";
		public static readonly string ACTIVEUSERCOLECTION = "ActiveUserCollection";
	}

	public class UserLoginInformation{
		[BsonId] // the unique identifier
		public string username { get; set; }
		public string password { get; set; }
		public bool isLoggedIn { get; set; }
	}
	public class UserInfo{
		[BsonId] // the unique identifier
		public string username { get; set; }
		public string roomActions { get; set; }
		public string logins { get; set; }
		public string profilePic {get; set;}
	}
	public class UserChatLog{
		[BsonId] // the unique identifier
		public string username { get; set; }
		public List<string> messages { get ; set; }
	}
	public class ActiveUser{
		[BsonId] // the unique identifier
		public string peerId { get; set; }
		[BsonId] // the unique identifier
		public string username { get; set; }
	}
}