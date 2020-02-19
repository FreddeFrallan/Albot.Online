using ChartAndGraph;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LoginDataController : DataController {
    

    private List<DatabaseHandler.UserSession> currentSessions = new List<DatabaseHandler.UserSession>(); // Sessions being shown atm

    protected override void Start() {
        base.Start();
        //dbHandler.fetchData(updateData);
    }

    protected override void retrieveData() {
        dbHandler.fetchLoginData(updateData);
    }

    public override string getInfoString(int x) {
        string res = "";
        if (currentGraph == graphByDate) {
            foreach (DatabaseHandler.UserSession ud in currentSessions.Where(s => s.startTime.Hour == x))
                res += ud.ToString() + "\n";
        } else if (currentGraph == graphByMonth) {
            foreach (DatabaseHandler.UserSession ud in currentSessions.Where(s => s.startTime.Day == x))
                res += ud.ToString() + "\n";
        } else
            foreach (DatabaseHandler.UserSession ud in currentSessions.Where(s => s.startTime.Month == x))
                res += ud.ToString() + "\n";
        return res;
    }

    protected override List<DatabaseHandler.UserData> getDataByDate(DateTime date) {
        currentSessions = dbHandler.getSessionsByDate(date);
        return currentSessions.Cast<DatabaseHandler.UserData>().ToList();
    }

    protected override List<DatabaseHandler.UserData> getDataByMonth(DateTime date) {
        currentSessions = dbHandler.getSessionsByMonth(date.Year, date.Month);
        return currentSessions.Cast<DatabaseHandler.UserData>().ToList();
    }

    protected override List<DatabaseHandler.UserData> getDataByYear(DateTime date) {
        currentSessions = dbHandler.getSessionsByYear(date.Year + timeOffset);
        return currentSessions.Cast<DatabaseHandler.UserData>().ToList();
    }

    protected override string getTitle(string date) {
        return "Logins " + date;
    }
}
