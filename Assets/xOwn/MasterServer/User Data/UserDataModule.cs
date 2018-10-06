using Barebones.MasterServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UserData;
using System;
using System.Linq;
using Barebones.Networking;
using UnityEngine.Networking;

namespace AlbotServer {

    public class UserDataModule : ServerModuleBehaviour {

        private static UserDataDB theDB;
        private static Dictionary<string, long> latestLogins = new Dictionary<string, long>();



        public override void Initialize(IServer server) {
            theDB = new UserDataDB(@Application.dataPath + "\\" + UserDataDatatypes.DB_PATH);

            server.SetHandler((short)CustomMasterServerMSG.requestLoginData, handleRequestUserLoginData);
            server.SetHandler((short)CustomMasterServerMSG.requestGamesData, handleGamesPlayedData);

            Debug.LogError("UserData Inited");
        }


        #region GamesPlayed
        public static void onGameStarted(PreGame game) {
            theDB.insertNewGameStarted(DateTime.Now.Ticks, game.specs.type.ToString(), playersToString(game.getPlayerInfo()));
        }

        private static string playersToString(PlayerInfo[] players) {
            string temp = "";
            for (int i = 0; i < players.Length; i++) { 
                temp += players[i].username;

                if (i + 1 < players.Length)
                    temp += ", ";
            }

            return temp;
        }

        private static void handleGamesPlayedData(IIncommingMessage rawMsg) {
            rawMsg.Respond(new PlayedGamesDataODT() { entries = theDB.getAllPlayedGames() },
                ResponseStatus.Success);
        }
        #endregion

        #region User login
        public static void onUserLogedIn(string username) {
            long time = DateTime.Now.Ticks;
            latestLogins.Add(username, time);

            theDB.insertNewLogin(time, username);
        }


        public static void userLogedOut(string username) {
            if (latestLogins.ContainsKey(username) == false)
                return;

            long logoutDate = DateTime.Now.Ticks;
            long loginTime = latestLogins[username];
            latestLogins.Remove(username);

            theDB.registerEndSession(loginTime, username, logoutDate);
        }

        private static void handleRequestUserLoginData(IIncommingMessage rawMsg) {
            rawMsg.Respond(new LoginDataODT() { entries = theDB.getAllLoginEntries() },
                ResponseStatus.Success);
        }
        #endregion

    }


    public class LoginDataODT : MessageBase { public UserLoginEntry[] entries; }
    public class PlayedGamesDataODT : MessageBase {public GameStartedEntry[] entries;}

}