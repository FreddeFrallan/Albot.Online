using LiteDB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UserData {

    public class UserDataDB{

        private static LiteDatabase theDB;
        private static LiteCollection<UserLoginEntry> loginColl;
        private static LiteCollection<GameStartedEntry> startedGamesColl;
        private static long gameCounter = 0;


        public UserDataDB(string path) {
            theDB = new LiteDatabase(path);

            loginColl = theDB.GetCollection<UserLoginEntry>(UserDataDatatypes.LoginCollection);
            startedGamesColl = theDB.GetCollection<GameStartedEntry>(UserDataDatatypes.GameStartedCollection);
        }


        #region Logins
        public void insertNewLogin(long time, string username) {
            loginColl.Insert(newLoginEntry(time, username));
        }

        public void registerEndSession(long loginTime, string username, long logoutTime) {
            UserLoginEntry entry = loginColl.FindOne(l => l.username == username && l.time == loginTime);
            if (entry != null)
                loginColl.Update(newLoginEntry(loginTime, username, logoutTime));
        }

        private UserLoginEntry newLoginEntry(long loginTime, string username) {
            return new UserLoginEntry() { time = loginTime, username = username };
        }
        private UserLoginEntry newLoginEntry(long loginTime, string username, long logoutTime) {
            return new UserLoginEntry() { time = loginTime, username = username, duration = (logoutTime - loginTime) };
        }

        public UserLoginEntryStruct[] getAllLoginEntries() {
            List<UserLoginEntryStruct> temp = new List<UserLoginEntryStruct>();
            foreach(UserLoginEntry u in loginColl.FindAll())
                temp.Add(new UserLoginEntryStruct() { time = u.time, username = u.username, duration = u.duration });
            return temp.ToArray();
        }
        #endregion



        #region GameStarted
        public void insertNewGameStarted(long time, string gameType, string players) {
            startedGamesColl.Insert(
                new GameStartedEntry() {gameID = gameCounter++, time = time, gameType = gameType, players = players });
        }

        public GameStartedStruct[] getAllPlayedGames() {
            List<GameStartedStruct> temp = new List<GameStartedStruct>();
            foreach (GameStartedEntry u in startedGamesColl.FindAll())
                temp.Add(new GameStartedStruct() { time = u.time, gameID = u.gameID, gameType = u.gameType, players = u.players});
            return temp.ToArray();
        }
        #endregion
    }

}