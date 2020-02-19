using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChartHandler : MonoBehaviour {

    public GameObject barChartHolder, graphChartHolder;
    public UIHandler graphUIHandler;
    public BarChartDataController barUIHandler;
	
	void Start () {
        graphUIHandler.addGamePopularityListener(switchChart);
        barUIHandler.addFrequencyGraphListener(switchChart);
	}

    void switchChart() {
        if (barChartHolder.activeSelf) {
            barChartHolder.SetActive(false);
            graphChartHolder.SetActive(true);
        } else {
            graphChartHolder.SetActive(false);
            barChartHolder.SetActive(true);
        }
    }
	
}
