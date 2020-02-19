using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ChartAndGraph;
using UnityEngine.UI;
using UnityEngine.Events;

public class BarChartDataController : MonoBehaviour {

    public DatabaseHandler dbHandler;
    public BarChart barChart;
    public Button frequencyGraphButton;

    private string snake = "Snake", connect4 = "Connect4";
    private string life = "This Life", year = "This Year", month = "This Month";
    private bool started = false;

	// Use this for initialization
	void Start () {
        dbHandler.fetchGamesData(updateData);
        started = true;
	}

    void OnEnable() {
        if(started)
            updateData();
    }

    public void addFrequencyGraphListener(UnityAction clicked) {
        frequencyGraphButton.onClick.AddListener(clicked);
    }

    void updateData() {
        barChart.DataSource.StartBatch();
        barChart.DataSource.ClearValues();
        
        DateTime now = DateTime.Now;
        List<DatabaseHandler.GamePlayed> snakeGames = dbHandler.getGames().Where(x => x.gameType == snake).ToList();
        List<DatabaseHandler.GamePlayed> connect4Games = dbHandler.getGames().Where(x => x.gameType == connect4).ToList();
        barChart.DataSource.MaxValue = Math.Max(snakeGames.Count, connect4Games.Count);

        barChart.DataSource.SetValue(snake, life, snakeGames.Count);
        barChart.DataSource.SetValue(snake, year, snakeGames.Where(x => x.startTime.Year == now.Year).ToList().Count);
        barChart.DataSource.SetValue(snake, month, snakeGames.Where(x => x.startTime.Year == now.Year && x.startTime.Month == now.Month).ToList().Count);
        barChart.DataSource.SetValue(connect4, life, connect4Games.Count);
        barChart.DataSource.SetValue(connect4, year, connect4Games.Where(x => x.startTime.Year == now.Year).ToList().Count);
        barChart.DataSource.SetValue(connect4, month, connect4Games.Where(x => x.startTime.Year == now.Year && x.startTime.Month == now.Month).ToList().Count);
        
        barChart.DataSource.EndBatch();
    }

}
