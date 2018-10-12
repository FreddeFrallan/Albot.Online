using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {

    public Button buttonByDay, buttonByMonth, buttonByYear, buttonNext, buttonPrevious, buttonGamePopularity;
    public Dropdown dataDropdown;
    public LoginDataController loginDataController;
    public GamesDataController gamesDataController;

    private DataController currentDataController;

    void Start () {
        currentDataController = loginDataController;
        dataDropdown.onValueChanged.AddListener(delegate { switchDataCategory(dataDropdown.value); });
        

        addButtonListeners();

        fixTimeLabels();
    }

    public void addGamePopularityListener(UnityAction clicked) {
        buttonGamePopularity.onClick.AddListener(clicked);
    }

    private void fixTimeLabels() {
        for (int i = 0; i <= 9; i++) {
            currentDataController.graphByDate.HorizontalValueToStringMap.Add(i, "0" + i.ToString());
            currentDataController.graphByMonth.HorizontalValueToStringMap.Add(i, "0" + i.ToString());
            currentDataController.graphByYear.HorizontalValueToStringMap.Add(i, "0" + i.ToString());
        }
    }

    private void switchDataCategory(int value) {
        string dataCat = value == 0 ? "Logins" : "Games";
        switchDataCategory(dataCat);
    }

    private void switchDataCategory(string dataCategory) {
        if (dataCategory == "Logins") {
            if (currentDataController != loginDataController)
                switchToCat(loginDataController);
        } else if (dataCategory == "Games")
            if(currentDataController != gamesDataController)
                switchToCat(gamesDataController);
    }

    private void switchToCat(DataController controller) {
        removeButtonListeners();

        currentDataController.gameObject.SetActive(false);
        currentDataController = controller;
        currentDataController.gameObject.SetActive(true);

        addButtonListeners();
    }

    private void addButtonListeners() {
        buttonByDay.onClick.AddListener(currentDataController.showByDate);
        buttonByMonth.onClick.AddListener(currentDataController.showByMonth);
        buttonByYear.onClick.AddListener(currentDataController.showByYear);
        buttonNext.onClick.AddListener(delegate { currentDataController.timeStep(true); });
        buttonPrevious.onClick.AddListener(delegate { currentDataController.timeStep(false); });
    }

    private void removeButtonListeners() {
        buttonByDay.onClick.RemoveAllListeners();
        buttonByMonth.onClick.RemoveAllListeners();
        buttonByYear.onClick.RemoveAllListeners();
        buttonNext.onClick.RemoveAllListeners();
        buttonPrevious.onClick.RemoveAllListeners();
    }
    
}
