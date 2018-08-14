using System.Collections.Generic;
using System.Text;
using System;
using AlbotServer;
using Barebones.MasterServer;


public class GameHistory{


	private List<byte> gameLog = new List<byte> ();
    private List<GameLogState> rawGameLog = new List<GameLogState>();
    private GameServerSpectatorModule spectatorModule;
    private int updateCounter = 0;

    public void init(GameServerSpectatorModule spectatorModule) {
        this.spectatorModule = spectatorModule;
    }

    #region Push to log
    public void pushToRawLog(int data){ pushToRawLog(data.ToString());}
	public void pushToRawLog(string data){ pushToRawLog(new string[] { data }); }
    public void pushToRawLog(string[] data){
        GameLogState newUpdate = new GameLogState() {
            updateNumber = updateCounter,
            log = data
        };
        rawGameLog.Add(newUpdate);
        updateCounter++;

        spectatorModule.updateAdded(newUpdate);
    }
    #endregion




    #region getters & setters
    public GameLogState[] getFullLog(){return rawGameLog.ToArray ();}

    public GameLogState[] getSpecificStates(int[] targets) {
        List<GameLogState> states = new List<GameLogState>();
        foreach (int t in targets)
            states.Add(rawGameLog.Find(g => g.updateNumber == t));
        return states.ToArray();
    }
	#endregion
}
