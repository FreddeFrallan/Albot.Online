using ChartAndGraph;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;
using UserData;
using Barebones.Networking;

public class LoginData : MonoBehaviour {

    public GraphChart chart;

    private string dataCategory = "Logins";

    // Use this for initialization
    void Start() {
        fixTimeLabels();

        // x-axis unit: seconds
        addLoginData();
        //List<int> testList = simulateLoginHours(500);//new List<int> { 1, 1,1,1,1,1, 3, 23 };
        //List<DataPoint> dataPoints = calculateHourFreqPairs(testList);

        //chart.DataSource.AddPointToCategory(dataCategory, DateTime.Now.Hour, 3);
        /*
        foreach(DataPoint p in dataPoints)
            chart.DataSource.AddPointToCategory(dataCategory, p.x, p.y);
            */
        
    }

    private List<int> simulateLoginHours(int n) {
        List<int> logins = new List<int>();
        System.Random rand = new System.Random();
        for (int i = 0; i < n; i++) 
            logins.Add(rand.Next(0, 24));
        return logins;
    }

    private List<DataPoint> calculateHourFreqPairs(List<int> hours) {
        List<DataPoint> points = new List<DataPoint>();
        for(int i = 0; i <= 23; i++) // Optimize with group by to achieve a speedup of up to 24
            points.Add(new DataPoint() { x = i, y = hours.Where(h => h == i).Count() });
        return points;
    }
    
    // Queries the server for data
    private void addLoginData() {
        List<int> loginHours = new List<int>();

        Msf.Connection.SendMessage((short)CustomMasterServerMSG.requestLoginData, (status, response) => {
            if (status == ResponseStatus.Success) {
                foreach (UserLoginEntryStruct l in response.Deserialize<LoginDataODT>().entries) {
                    loginHours.Add(extractHour(l.time));
                }
            }
            addNewDataPoints(loginHours);
        });
        
    }

    private void addNewDataPoints(List<int> loginHours) {
        chart.DataSource.StartBatch();
        chart.DataSource.ClearCategory(dataCategory);

        List<DataPoint> dataPoints = calculateHourFreqPairs(loginHours);

        foreach (DataPoint p in dataPoints) 
            chart.DataSource.AddPointToCategory(dataCategory, p.x, p.y);

        chart.DataSource.EndBatch();
    }

    private void fixTimeLabels() {
        for (int i = 0; i <= 9; i++)
            chart.HorizontalValueToStringMap.Add(i, "0" + i.ToString());
    }

    private int extractHour(long ticks) {
        DateTime dateTime = new DateTime(ticks);
        return dateTime.Hour;
    }

    private struct DataPoint {
        public double x, y;
    }
}
