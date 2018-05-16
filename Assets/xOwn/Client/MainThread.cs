using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainThread : MonoBehaviour {

	private static MainThread singleton;
	private static List<Action> events = new List<Action>();
	private static List<timedAction> timedActions = new List<timedAction>();
	private static float lastTime;

	// Use this for initialization
	void Start () {
		if (singleton != null && singleton != this)
			Destroy (gameObject);
		
		singleton = this;
		DontDestroyOnLoad (gameObject);
	}

	#region Main thread
	void Update(){
		lastTime = Time.time;
		sendEvents ();
		checkForTimedActions ();
	}
	private void sendEvents(){
		for (int i = 0; i < events.Count; i++) {
			try{events[i].Invoke ();}
			catch{
			}
		}
		events.Clear ();
	}
	private void checkForTimedActions(){
		for(int i = timedActions.Count-1; i >= 0; i--)
			if (Time.time >= timedActions[i].waitTime) {
				timedActions [i].theAction.Invoke ();
				timedActions.RemoveAt (i);
			}
	}
	#endregion


	void OnApplicationQuit(){
		TCPLocalConnection.OnApplicationQuit ();
	}


	public static void fireEventAtMainThread(Action theEvent){
		events.Add (theEvent);
	}
	public static void createTimedAction(Action a, float wt){
		timedActions.Add(new timedAction(){waitTime = lastTime + wt, theAction = a});
	}
		

	private struct timedAction{
		public float waitTime;
		public Action theAction;
	}
}

