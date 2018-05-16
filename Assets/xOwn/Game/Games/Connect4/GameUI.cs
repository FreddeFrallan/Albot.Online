using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ClientUI;
using Barebones.MasterServer;
using System.Linq;

public class GameUI : MonoBehaviour {
	
	private int gameID;
	public ClientUserPanelUI[] panels;
	private Dictionary<Game.PlayerColor, ClientUserPanelUI> playerColorToUserPanel = new Dictionary<Game.PlayerColor, ClientUserPanelUI>();

	public List<Game.PlayerColor> panelOrder = new List<Game.PlayerColor> ();

	void Awake(){
		for (int i = 0; i < panelOrder.Count; i++)
			playerColorToUserPanel.Add (panelOrder [i], panels [i]);
	}


	public void initPlayerSlot(Game.PlayerColor color, string username, int iconNumber)
		{playerColorToUserPanel [color].setUserPanel (iconNumber, username);
	}
	public void removeConnectedPlayer (Game.PlayerColor color, string username, int iconNumber){
		playerColorToUserPanel [color].clearPanel ();
	}

	#region turnbased timers	//Should later be moved somewhere else perhaps, or made generic to fit realtime games.
	public void initTimer(Game.PlayerColor color, float maxTime){
		playerColorToUserPanel [color].initTurnTime (maxTime);
	}
		
	public void stopAllTimers(){
		foreach (ClientUserPanelUI panel in panels)
			panel.stopTimer ();
	}

	public void startTimer(Game.PlayerColor color, float maxTime){
		activateNewSetTimer (color, maxTime);
	}
	private void activateNewSetTimer(Game.PlayerColor color, float maxTime){
		foreach (KeyValuePair<Game.PlayerColor, ClientUserPanelUI> storedValue in playerColorToUserPanel) {
			if (storedValue.Key == color)
				playerColorToUserPanel [storedValue.Key].startTimer (maxTime);
			else
				playerColorToUserPanel [storedValue.Key].stopTimer ();
		}
	}
	#endregion 
}
