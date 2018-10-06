using LiteDB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UserData {

    public class UserDataDatatypes : MonoBehaviour {
        public static readonly string DB_PATH = "UserData.db";
        public static readonly string LoginCollection = "LoginCollection";
        public static readonly string GameStartedCollection = "StartedGames";
    }

    public class UserLoginEntry {
        [BsonId]
        public long time { get; set; }
        public string username { get; set; }
        public long duration { get; set; }
    }

    public class GameStartedEntry {
        [BsonId]
        public long gameID { get; set; }
        public long time { get; set; }
        public string gameType { get; set; }
        public string players { get; set; }
    }


}