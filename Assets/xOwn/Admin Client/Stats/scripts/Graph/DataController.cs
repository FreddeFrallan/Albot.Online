﻿using ChartAndGraph;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class DataController : MonoBehaviour {

    public DatabaseHandler dbHandler;
    public GraphChart graphByDate, graphByMonth, graphByYear;
    public Text title;
    public string dataCategoryName;

    protected List<DatabaseHandler.UserData> currentUserData = new List<DatabaseHandler.UserData>();
    protected GraphChart currentGraph;
    protected int timeOffset = 0;

    private bool started = false;
    
    protected virtual void Start() {
        currentGraph = graphByDate;
        started = true;
        retrieveData();
    }

    protected abstract void retrieveData();

    void OnEnable() {
        if(started)
            showByDate();
        //title.text = getTitle(DateTime.Now.Date.ToString("yyyy/MM/dd"));
    }

    public void showByDate() {
        switchChart(graphByDate);
    }
    public void showByMonth() {
        switchChart(graphByMonth);
    }
    public void showByYear() {
        switchChart(graphByYear);
    }
    private void switchChart(GraphChart chart) {
        //if (chart != currentChart) {
            timeOffset = 0;
            currentGraph.gameObject.SetActive(false);
            chart.gameObject.SetActive(true);
            currentGraph = chart;
            updateData();
        //}
    }
    public void timeStep(bool forward) {
        timeOffset += forward ? 1 : -1;

        updateData();
    }

    // Needs specific child class!
    public abstract string getInfoString(int x);

    // For one day
    protected List<DataPoint> calculateFreqByHour(List<DatabaseHandler.UserData> sessions) {
        List<DataPoint> points = new List<DataPoint>();
        for (int i = 0; i <= 23; i++) // Optimize with group by to achieve a speedup of up to 24
            points.Add(new DataPoint() { x = i, y = sessions.Where(s => s.startTime.Hour == i).Count() });
        return points;
    }

    // For one month
    protected List<DataPoint> calculateFreqByDay(List<DatabaseHandler.UserData> sessions) {
        List<DataPoint> points = new List<DataPoint>();
        for (int i = 1; i <= 31; i++)
            points.Add(new DataPoint() { x = i, y = sessions.Where(d => d.startTime.Day == i).Count() });
        return points;
    }

    // For one year
    protected List<DataPoint> calculateFreqByMonth(List<DatabaseHandler.UserData> sessions) {
        List<DataPoint> points = new List<DataPoint>();
        for (int i = 1; i <= 12; i++)
            points.Add(new DataPoint() { x = i, y = sessions.Where(d => d.startTime.Month == i).Count() });
        return points;
    }

    protected void updateData() {
        currentGraph.DataSource.StartBatch();
        currentGraph.DataSource.ClearCategory(dataCategoryName);

        List<DataPoint> dataPoints;
        if (currentGraph == graphByDate) {
            DateTime date = DateTime.Now.AddDays(timeOffset).Date;
            title.text = getTitle(date.ToString("yyyy/MM/dd"));
            currentUserData = getDataByDate(date);
            dataPoints = calculateFreqByHour(currentUserData);
            
        } else if (currentGraph == graphByMonth) {
            DateTime date = DateTime.Now.AddMonths(timeOffset);
            title.text = getTitle(date.ToString("yyyy/MM"));
            currentUserData = getDataByMonth(date);
            dataPoints = calculateFreqByDay(currentUserData);
            
        } else {
            DateTime date = DateTime.Now.AddYears(timeOffset);
            title.text = getTitle(date.ToString("yyyy"));
            currentUserData = getDataByYear(date);
            dataPoints = calculateFreqByMonth(currentUserData);
        }

        int maxY = 0;
        foreach (DataPoint p in dataPoints) {
            currentGraph.DataSource.AddPointToCategory(dataCategoryName, p.x, p.y);
            if (p.y > maxY)
                maxY = (int)Math.Ceiling(p.y);
        }
        currentGraph.DataSource.VerticalViewSize = getVerticalViewSize(maxY);

        currentGraph.DataSource.EndBatch();
    }

    private int getVerticalViewSize(int maxY) {
        return ((maxY / 50) + 1) * 50;
    }

    protected abstract List<DatabaseHandler.UserData> getDataByDate(DateTime date);
    protected abstract List<DatabaseHandler.UserData> getDataByMonth(DateTime date);
    protected abstract List<DatabaseHandler.UserData> getDataByYear(DateTime date);

    protected abstract string getTitle(string date);

    public struct DataPoint {
        public double x, y;
    }
}
