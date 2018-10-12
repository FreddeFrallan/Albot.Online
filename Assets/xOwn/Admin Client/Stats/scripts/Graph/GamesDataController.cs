using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GamesDataController : DataController {

    private List<DatabaseHandler.GamePlayed> currentGames = new List<DatabaseHandler.GamePlayed>(); // Sessions being shown atm

    protected override void retrieveData() {
        dbHandler.fetchGamesData(updateData);
    }

    public override string getInfoString(int x) {
        string res = "";
        if (currentGraph == graphByDate) {
            foreach (DatabaseHandler.GamePlayed ud in currentGames.Where(s => s.startTime.Hour == x))
                res += ud.ToString() + "\n";
        } else if (currentGraph == graphByMonth) {
            foreach (DatabaseHandler.GamePlayed ud in currentGames.Where(s => s.startTime.Day == x))
                res += ud.ToString() + "\n";
        } else
            foreach (DatabaseHandler.GamePlayed ud in currentGames.Where(s => s.startTime.Month == x))
                res += ud.ToString() + "\n";
        return res;
    }

    protected override List<DatabaseHandler.UserData> getDataByDate(DateTime date) {
        currentGames = dbHandler.getGamesByDate(date);
        return currentGames.Cast<DatabaseHandler.UserData>().ToList();
    }

    protected override List<DatabaseHandler.UserData> getDataByMonth(DateTime date) {
        currentGames = dbHandler.getGamesByMonth(date.Year, date.Month);
        return currentGames.Cast<DatabaseHandler.UserData>().ToList();
    }

    protected override List<DatabaseHandler.UserData> getDataByYear(DateTime date) {
        currentGames = dbHandler.getGamesByYear(date.Year + timeOffset);
        return currentGames.Cast<DatabaseHandler.UserData>().ToList();
    }

    protected override string getTitle(string date) {
        return "Games Played " + date;
    }
}
