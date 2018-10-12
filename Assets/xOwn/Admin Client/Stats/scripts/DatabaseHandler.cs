using Barebones.MasterServer;
using Barebones.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UserData;

public class DatabaseHandler : MonoBehaviour {

    private List<UserSession> sessions = new List<UserSession>();
    private List<GamePlayed> gamesPlayed = new List<GamePlayed>();

    private bool loginsFetched = false, gamesFetched = false;

    void Start () {
        
    }


    // Queries the server for data
    public void fetchData(Action callback) {
        fetchGameData();
        fetchLoginData(callback);
    }

    public void fetchGameData() {
        TimeZone zone = TimeZone.CurrentTimeZone;
        Msf.Connection.SendMessage((short)CustomMasterServerMSG.requestGamesData, (status, response) => {
            if (status == ResponseStatus.Success)
                foreach (GameStartedStruct l in response.Deserialize<PlayedGamesDataODT>().entries) {
                    gamesPlayed.Add(new GamePlayed(l.players, "", l.gameType, zone.ToLocalTime(new DateTime(l.time)), l.gameID));
                    print("Players: " + l.players + "GameType: " + l.gameType);
                }

            gamesFetched = true;
        });
    }

    public void fetchLoginData(Action callback) {
        TimeZone zone = TimeZone.CurrentTimeZone;
        Msf.Connection.SendMessage((short)CustomMasterServerMSG.requestLoginData, (status, response) => {
            if (status == ResponseStatus.Success)
                foreach (UserLoginEntryStruct l in response.Deserialize<LoginDataODT>().entries)
                    sessions.Add(new UserSession(l.username, zone.ToLocalTime(new DateTime(l.time)), new DateTime(l.duration)));

            loginsFetched = true;
            callback();
        });
    }

    public List<UserSession> getSessionsByDate(DateTime date) {
        return sessions.Where(x => x.startTime.Date == date.Date).ToList();
    }

    public List<UserSession> getSessionsByMonth(int year, int month) {
        return sessions.Where(x => x.startTime.Month == month && x.startTime.Year == year).ToList();
    }

    public List<UserSession> getSessionsByYear(int year) {
        return sessions.Where(x => x.startTime.Year == year).ToList();
    }

    public List<GamePlayed> getGamesByDate(DateTime date) {
        return gamesPlayed.Where(x => x.startTime.Date == date.Date).ToList();
    }

    public List<GamePlayed> getGamesByMonth(int year, int month) {
        return gamesPlayed.Where(x => x.startTime.Month == month && x.startTime.Year == year).ToList();
    }

    public List<GamePlayed> getGamesByYear(int year) {
        return gamesPlayed.Where(x => x.startTime.Year == year).ToList();
    }

    public abstract class UserData {
        public DateTime startTime;
    }

    public class UserSession : UserData {
        public readonly string username;
        public readonly DateTime duration;

        public UserSession(string username, DateTime login, DateTime duration) {
            this.username = username;
            this.startTime = login;
            this.duration = duration;
        }

        public override string ToString() {
            string durationString = duration.ToString("HHmmss").Insert(6, "s").Insert(4, "m").Insert(2, "h");
            return username + ":    " + startTime.ToString("yyyy/MM/dd|HH:mm:ss") + ", " + durationString;
        }
    }

    public class GamePlayed : UserData {
        readonly string username1, username2, gameType;
        readonly long gameId;

        public GamePlayed(string username1, string username2, string gameType, DateTime startTime, long gameId) {
            this.username1 = username1;
            this.username2 = username2;
            this.gameType = gameType;
            this.startTime = startTime;
            this.gameId = gameId;
        }

        public override string ToString() {
            return gameType + ": " + username1 + " vs " + username2 + ", Id: " + gameId + ", " + startTime.ToString("yyyy/MM/dd|HH:mm:ss");
        }
    }
}
