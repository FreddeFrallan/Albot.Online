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
    private List<DateTime> loginTimeStamps = new List<DateTime>();
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
        TimeZone zone = TimeZone.CurrentTimeZone;
        Msf.Connection.SendMessage((short)CustomMasterServerMSG.requestLoginData, (status, response) => {
            if (status == ResponseStatus.Success) {
                foreach (UserLoginEntryStruct l in response.Deserialize<LoginDataODT>().entries) {
                    loginTimeStamps.Add(zone.ToLocalTime(new DateTime(l.time)));
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
            dataPoints = calculateFreqByHour(getLoginsByDate(date));
            titleHandler.setDate(date.ToString("yyyy/MM/dd"));
        } else if (currentChart == loginsByMonth) {
            DateTime date = DateTime.Now.AddMonths(timeOffset);
            dataPoints = calculateFreqByDay(getLoginsByMonth(date.Year, date.Month));
            titleHandler.setDate(date.ToString("yyyy/MM"));
        } else {
            DateTime date = DateTime.Now.AddYears(timeOffset);
            dataPoints = calculateFreqByMonth(getLoginsByYear(date.Year + timeOffset));
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

}
