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
    /*
    public void fetchData(Action callback) {
        fetchGamesData(callback);
        //fetchLoginData(callback);
    }
    */
    
    public void fetchLoginData(Action callback) {
        if (!loginsFetched) {
            TimeZone zone = TimeZone.CurrentTimeZone;
            Msf.Connection.SendMessage((short)CustomMasterServerMSG.requestLoginData, (status, response) => {
                if (status == ResponseStatus.Success)
                    foreach (UserLoginEntryStruct l in response.Deserialize<LoginDataODT>().entries)
                        sessions.Add(new UserSession(l.username, zone.ToLocalTime(new DateTime(l.time)), new DateTime(l.duration)));

                loginsFetched = true;
                callback();
            });
        } else
            callback();
    }

    public void fetchGamesData(Action callback) {
        if (!gamesFetched) {
            TimeZone zone = TimeZone.CurrentTimeZone;
            Msf.Connection.SendMessage((short)CustomMasterServerMSG.requestGamesData, (status, response) => {
                if (status == ResponseStatus.Success)
                    foreach (GameStartedStruct g in response.Deserialize<PlayedGamesDataODT>().entries) {
                        gamesPlayed.Add(new GamePlayed(g.players, g.gameType, zone.ToLocalTime(new DateTime(g.time)), g.gameID));
                    }

                gamesFetched = true;
                callback();
            });
        } else
            callback();
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
    public List<UserSession> getSessions() {
        return sessions;
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

    public List<GamePlayed> getGames() {
        return gamesPlayed;
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
        public readonly string players, gameType;
        public readonly long gameId;

        public GamePlayed(string players, string gameType, DateTime startTime, long gameId) {
            this.players = players;
            this.gameType = gameType;
            this.startTime = startTime;
            this.gameId = gameId;
        }

        public override string ToString() {
            return gameId + ", " + gameType + ": (" + players + ") " + startTime.ToString("yyyy/MM/dd|HH:mm:ss");
        }
    }
}
