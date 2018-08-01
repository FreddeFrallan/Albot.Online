using AlbotServer;
using Barebones.MasterServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using AdminUI;

public class GameRenderer : MonoBehaviour{


    private GameUI theUI;
    private List<GameLogState> historyLog = new List<GameLogState>();
    protected int updateCounter = 0;
    private bool pullingFromLog = true;
    protected bool gameOver = false;


    public virtual void init(GameUI theUI) {this.theUI = theUI;}
    public void adminInit() {theUI = GetComponent<GameUI>();}

    public virtual void initPlayerSlots(PlayerInfo[] players) {
        foreach (PlayerInfo p in players)
            theUI.initPlayerSlot(p);
    }


    #region LogSystem
    public void addLogMsg(SpectatorGameLog rawMsg) {addLogMsg(rawMsg.gameLog);}
    public void addLogMsg(GameLogState[] logStates) {
        historyLog.AddRange(logStates);
        if (pullingFromLog == false)
            return;
        runAllGameHistory();
    }

    private void runAllGameHistory() {
        for(int i = updateCounter; i < historyLog.Count; i++) {
            GameLogState state = historyLog[i];

            if (state.updateNumber != updateCounter) {
                requestMissingUpdates();
                return;
            }

            displayNewUpdate(state);
            updateCounter++;
        }
    }

    protected virtual void displayNewUpdate(GameLogState update) {}
    #endregion

    #region Missing Updates
    private void requestMissingUpdates() {
        pullingFromLog = false;
        AdminUpdateManager.requestSpecificLogMessages(getMissingUpdates());
    }

    private int[] getMissingUpdates() {
        historyLog = historyLog.OrderBy(g => g.updateNumber).ToList();
        List<int> missing = new List<int>();
        int updateCounter = 0;

        foreach(GameLogState update in historyLog) { 
            if(update.updateNumber != updateCounter) {
                Enumerable.Range(updateCounter, update.updateNumber).ToList().ForEach(m => missing.Add(m));
                updateCounter = update.updateNumber+1;
            }else
                updateCounter++;
        }
        return missing.ToArray();
    }

    public void addMissingUpdates(GameLogState[] updates) {
        historyLog.AddRange(updates);
        if (getMissingUpdates().Length == 0) {
            pullingFromLog = true;
            runAllGameHistory();
        }
        else
            Debug.LogError("Got missing updates, but still missing some..");
    }
    #endregion
}
