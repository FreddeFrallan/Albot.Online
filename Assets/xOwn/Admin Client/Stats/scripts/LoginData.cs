using ChartAndGraph;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;
using UserData;
using Barebones.Networking;
using UnityEngine.UI;


public class LoginData : MonoBehaviour {

    public TitleHandler titleHandler;
    public GraphChart loginsByDate, loginsByMonth, loginsByYear;
    public Button buttonByDay, buttonByMonth, buttonByYear, buttonNext, buttonPrevious;

    private GraphChart currentChart;
    private string dataCategory = "Logins";
    private List<UserSession> allSessions = new List<UserSession>();
    private List<UserSession> currentSessions = new List<UserSession>(); // Sessions being shown atm
    //private List<DateTime> loginTimeStamps = new List<DateTime>();
    private int timeOffset = 0;
    

    void Start() {
        buttonByDay.onClick.AddListener(showByDate);
        buttonByMonth.onClick.AddListener(showByMonth);
        buttonByYear.onClick.AddListener(showByYear);
        buttonNext.onClick.AddListener(delegate { timeStep(true); });
        buttonPrevious.onClick.AddListener(delegate { timeStep(false); });

        fixTimeLabels();

        currentChart = loginsByDate;
        fetchLoginData();
    }

    public void showByDate() {
        switchChart(loginsByDate);
    }
    public void showByMonth() {
        switchChart(loginsByMonth);
    }
    public void showByYear() {
        switchChart(loginsByYear);
    }
    private void switchChart(GraphChart chart) {
        if (chart != currentChart) {
            timeOffset = 0;
            currentChart.gameObject.SetActive(false);
            chart.gameObject.SetActive(true);
            currentChart = chart;
            updateData();
        }
    }
    private void timeStep(bool forward) {
        timeOffset += forward ? 1 : -1;

        updateData();
    }

    public string getSessionsString(int x) {
        string res = "";
        if (currentChart == loginsByDate) {
            foreach (UserSession us in currentSessions.Where(s => s.login.Hour == x))
                res += us.ToString() + "\n";
        } else if(currentChart == loginsByMonth) {
            foreach (UserSession us in currentSessions.Where(s => s.login.Day == x))
                res += us.ToString() + "\n";
        } else
            foreach (UserSession us in currentSessions.Where(s => s.login.Month == x))
                res += us.ToString() + "\n";
        return res;
    }

    // For one day
    private List<DataPoint> calculateFreqByHour(List<UserSession> sessions) {
        List<DataPoint> points = new List<DataPoint>();
        for(int i = 0; i <= 23; i++) // Optimize with group by to achieve a speedup of up to 24
            points.Add(new DataPoint() { x = i, y = sessions.Where(s => s.login.Hour == i).Count() });
        return points;
    }

    // For one month
    private List<DataPoint> calculateFreqByDay(List<UserSession> dates) {
        List<DataPoint> points = new List<DataPoint>();
        for (int i = 1; i <= 31; i++)
            points.Add(new DataPoint() { x = i, y = dates.Where(d => d.login.Day == i).Count() });
        return points;
    }

    // For one year
    private List<DataPoint> calculateFreqByMonth(List<UserSession> dates) {
        List<DataPoint> points = new List<DataPoint>();
        for (int i = 1; i <= 12; i++)
            points.Add(new DataPoint() { x = i, y = dates.Where(d => d.login.Month == i).Count() });
        return points;
    }

    // Queries the server for data
    private void fetchLoginData() {
        TimeZone zone = TimeZone.CurrentTimeZone;
        Msf.Connection.SendMessage((short)CustomMasterServerMSG.requestLoginData, (status, response) => {
            if (status == ResponseStatus.Success) {
                foreach (UserLoginEntryStruct l in response.Deserialize<LoginDataODT>().entries) {
                    //loginTimeStamps.Add(zone.ToLocalTime(new DateTime(l.time)));
                    allSessions.Add(new UserSession() {
                        username = l.username,
                        login = zone.ToLocalTime(new DateTime(l.time)),
                        duration = new DateTime(l.duration)
                    });
                }
            }
            updateData();
        });
        
    }

    private void updateData() {
        currentChart.DataSource.StartBatch();
        currentChart.DataSource.ClearCategory(dataCategory);

        List<DataPoint> dataPoints;
        if (currentChart == loginsByDate) {
            DateTime date = DateTime.Now.AddDays(timeOffset).Date;
            dataPoints = calculateFreqByHour(getSessionsByDate(date));
            titleHandler.setDate(date.ToString("yyyy/MM/dd"));
        } else if (currentChart == loginsByMonth) {
            DateTime date = DateTime.Now.AddMonths(timeOffset);
            dataPoints = calculateFreqByDay(getSessionsByMonth(date.Year, date.Month));
            titleHandler.setDate(date.ToString("yyyy/MM"));
        } else {
            DateTime date = DateTime.Now.AddYears(timeOffset);
            dataPoints = calculateFreqByMonth(getSessionsByYear(date.Year + timeOffset));
            titleHandler.setDate(date.ToString("yyyy"));
        }

        foreach (DataPoint p in dataPoints) 
            currentChart.DataSource.AddPointToCategory(dataCategory, p.x, p.y);

        currentChart.DataSource.EndBatch();
    }

    private void fixTimeLabels() {
        for (int i = 0; i <= 9; i++) {
            loginsByDate.HorizontalValueToStringMap.Add(i, "0" + i.ToString());
            loginsByMonth.HorizontalValueToStringMap.Add(i, "0" + i.ToString());
            loginsByYear.HorizontalValueToStringMap.Add(i, "0" + i.ToString());
        }
    }

    private List<UserSession> getSessionsByDate(DateTime date) {
        currentSessions = allSessions.Where(x => x.login.Date == date.Date).ToList();
        return currentSessions;
        //return loginTimeStamps.Where(x => x.Date == date.Date).ToList();
    }

    private List<UserSession> getSessionsByMonth(int year, int month) {
        currentSessions = allSessions.Where(x => x.login.Month == month && x.login.Year == year).ToList();
        return currentSessions;
    }

    private List<UserSession> getSessionsByYear(int year) {
        currentSessions = allSessions.Where(x => x.login.Year == year).ToList();
        return currentSessions;
    }

    private struct DataPoint {
        public double x, y;
    }

    private struct UserSession {
        public DateTime login, duration;
        public string username;

        public override string ToString() {
            string durationString = duration.ToString("HHmmss").Insert(6, "s").Insert(4, "m").Insert(2, "h");
            return username + ":    " + login.ToString("yyyy/MM/dd|HH:mm:ss") + ", " + durationString;
        }
    }

}
