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
    private GraphChart currentChart;

    private string dataCategory = "Logins";
    private List<DateTime> loginTimeStamps = new List<DateTime>();

    public Button buttonByDay, buttonByMonth, buttonByYear;

    void Start() {
        Button btnDay = buttonByDay.GetComponent<Button>();
        Button btnMonth = buttonByMonth.GetComponent<Button>();
        Button btnYear = buttonByYear.GetComponent<Button>();
        btnDay.onClick.AddListener(showByDate);
        buttonByMonth.onClick.AddListener(showByMonth);
        buttonByYear.onClick.AddListener(showByYear);

        //Make more generic
        fixTimeLabels();

        currentChart = loginsByDate;
        fetchLoginData();
    }

    public void showByDate() {
        titleHandler.setDate(DateTime.Now.Date.ToString("yyyy/MM/dd"));
        switchToChart(loginsByDate);
    }
    public void showByMonth() {
        titleHandler.setDate(DateTime.Now.Date.ToString("yyyy/MM"));
        switchToChart(loginsByMonth);
    }
    public void showByYear() {
        titleHandler.setDate(DateTime.Now.Year.ToString());
        switchToChart(loginsByYear);
    }
    private void switchToChart(GraphChart chart) {
        if (chart != currentChart) {
            currentChart.gameObject.SetActive(false);
            chart.gameObject.SetActive(true);
            currentChart = chart;
            updateDataPoints();
        }
    }

    // For one day
    private List<DataPoint> calculateFreqByHour(List<DateTime> dates) {
        List<DataPoint> points = new List<DataPoint>();
        for(int i = 0; i <= 23; i++) // Optimize with group by to achieve a speedup of up to 24
            points.Add(new DataPoint() { x = i, y = dates.Where(d => d.Hour == i).Count() });
        return points;
    }

    // For one month
    private List<DataPoint> calculateFreqByDay(List<DateTime> dates) {
        List<DataPoint> points = new List<DataPoint>();
        for (int i = 1; i <= 31; i++)
            points.Add(new DataPoint() { x = i, y = dates.Where(d => d.Day == i).Count() });
        return points;
    }

    // For one year
    private List<DataPoint> calculateFreqByMonth(List<DateTime> dates) {
        List<DataPoint> points = new List<DataPoint>();
        for (int i = 1; i <= 12; i++)
            points.Add(new DataPoint() { x = i, y = dates.Where(d => d.Month == i).Count() });
        return points;
    }

    // Queries the server for data
    private void fetchLoginData() {

        Msf.Connection.SendMessage((short)CustomMasterServerMSG.requestLoginData, (status, response) => {
            if (status == ResponseStatus.Success) {
                foreach (UserLoginEntryStruct l in response.Deserialize<LoginDataODT>().entries) {
                    loginTimeStamps.Add(new DateTime(l.time));
                }
            }
            updateDataPoints();
        });
        
    }

    private void updateDataPoints() {
        currentChart.DataSource.StartBatch();
        currentChart.DataSource.ClearCategory(dataCategory);

        List<DataPoint> dataPoints;
        if (currentChart == loginsByDate)
            dataPoints = calculateFreqByHour(getLoginsByDate(DateTime.Now.Date));
        else if(currentChart == loginsByMonth)
            dataPoints = calculateFreqByDay(getLoginsByMonth(2018, DateTime.Now.Month));
        else
            dataPoints = calculateFreqByMonth(getLoginsByYear(2018));

        foreach (DataPoint p in dataPoints) 
            currentChart.DataSource.AddPointToCategory(dataCategory, p.x, p.y);

        currentChart.DataSource.EndBatch();
    }

    private void fixTimeLabels() {
        for (int i = 0; i <= 9; i++)
            loginsByDate.HorizontalValueToStringMap.Add(i, "0" + i.ToString());
    }

    private List<DateTime> getLoginsByDate(DateTime date) {
        return loginTimeStamps.Where(x => x.Date == date.Date).ToList();
    }

    private List<DateTime> getLoginsByMonth(int year, int month) {
        return loginTimeStamps.Where(x => x.Month == month && x.Year == year).ToList();
    }

    private List<DateTime> getLoginsByYear(int year) {
        return loginTimeStamps.Where(x => x.Year == year).ToList();
    }

    private struct DataPoint {
        public double x, y;
    }

    /*
    //List<int> testList = simulateLoginHours(500);//new List<int> { 1, 1,1,1,1,1, 3, 23 };
    //List<DataPoint> dataPoints = calculateHourFreqPairs(testList);
    private List<int> simulateLoginHours(int n) {
        List<int> logins = new List<int>();
        System.Random rand = new System.Random();
        for (int i = 0; i < n; i++)
            logins.Add(rand.Next(0, 24));
        return logins;
    }
    */
}
