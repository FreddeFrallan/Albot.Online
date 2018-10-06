using LiteDB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

namespace UserData {

    public class UserDataDatatypes : MonoBehaviour {
        public static readonly string DB_PATH = "UserData.db";
        public static readonly string LoginCollection = "LoginCollection";
        public static readonly string GameStartedCollection = "StartedGames";
    }

    #region UserLogin
    public class UserLoginEntry {
        [BsonId]
        public long time { get; set; }
        public string username { get; set; }
        public long duration { get; set; }
    }

    public struct UserLoginEntryStruct {
        public long time;
        public string username;
        public long duration;
    }
    #endregion


    #region GameStarted
    public class GameStartedEntry {
        [BsonId]
        public long gameID { get; set; }
        public long time { get; set; }
        public string gameType { get; set; }
        public string players { get; set; }
    }

    public struct GameStartedStruct {
        public long gameID { get; set; }
        public long time { get; set; }
        public string gameType { get; set; }
        public string players { get; set; }
    }
    #endregion
    public class LoginDataODT : MessageBase { public UserLoginEntryStruct[] entries; }
    public class PlayedGamesDataODT : MessageBase { public GameStartedStruct[] entries; }

}