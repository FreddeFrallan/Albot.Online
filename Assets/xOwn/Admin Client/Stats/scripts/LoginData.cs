using ChartAndGraph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginData : MonoBehaviour {

    public GraphChart chart;

    private string DataCategory = "Logins";

    // Use this for initialization
    void Start () {
        chart.DataSource.StartBatch();

        chart.DataSource.ClearCategory(DataCategory);
        
        for(int i = 1; i <= 24; i++)
            chart.DataSource.AddPointToCategory(DataCategory, i, i*i);

        chart.DataSource.EndBatch();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
