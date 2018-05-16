using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.IO;
using Barebones.MasterServer;
using Barebones.Networking;

public class GameHistory{

	private string gameHistoryFolder = "PlayedGames";
	private List<byte> gameLog = new List<byte> ();
	private List<string> rawGameLog = new List<string>();
	private bool broadcastToSpectators = false;
	private int broadcastID;

	public void initHandlers(){
		Msf.Connection.SetHandler ((short)CustomMasterServerMSG.spectateStatus, handleSetSubscribeStatus);
		Msf.Connection.SetHandler ((short)CustomMasterServerMSG.requestFullGameLog, handleRequestFullGameLog);
		MsfArgs.runAfterArgsInit (() => {
			broadcastToSpectators = bool.Parse(Msf.Args.Spectators);
			broadcastID = Msf.Args.SpawnId;
		});
	}


	#region handlers
	public void handleSetSubscribeStatus(IIncommingMessage msg){
		SpectatorSubscriptionsMsg subMsg = msg.Deserialize <SpectatorSubscriptionsMsg> ();
		broadcastToSpectators = subMsg.active;
		broadcastID = subMsg.broadcastID;
	}

	private void handleRequestFullGameLog(IIncommingMessage msg){
		if (rawGameLog == null) {
			msg.Respond (ResponseStatus.Error);
			return;
		}
		if (msg.Deserialize<SpectatorSubscriptionsMsg> ().active)
			handleSetSubscribeStatus (msg);

		SpectatorGameLog logMsg = new SpectatorGameLog ();
		logMsg.gameLog = rawGameLog.ToArray ();
		msg.Respond (logMsg, ResponseStatus.Success);
	}
	#endregion

		
	#region getters & setters
	public string[] getRawCurrentLog(){return rawGameLog.ToArray ();}
	public void pushToRawLog(string[] data){
		rawGameLog.AddRange (data);
		broadcastUpdate ();
	}
	public void pushToRawLog(string data){
		rawGameLog.Add (data);	
		broadcastUpdate ();
	}
	public void pushToRawLog(int data){
		rawGameLog.Add (data.ToString());
		broadcastUpdate ();
	}
	
	public byte[] getCurrentLog(){return gameLog.ToArray ();}
	public void pushToLog(string data){gameLog.AddRange (Encoding.ASCII.GetBytes (data));	}
	public void pushToLog(int data){gameLog.AddRange (BitConverter.GetBytes (data));}
	#endregion

	private void broadcastUpdate(){
		if (broadcastToSpectators == false)
			return;

		string newUpdate = rawGameLog [rawGameLog.Count - 1];
		SpectatorGameLog msg = new SpectatorGameLog ();
		msg.id = broadcastID;
		msg.gameLog = new string[]{ newUpdate };
		msg.updateNumber = rawGameLog.Count;
		msg.initLog = false;
		Msf.Connection.SendMessage ((short)CustomMasterServerMSG.spectateLogUpdate, msg, ((status, r) => {
			if(status == ResponseStatus.Failed || status == ResponseStatus.Error)
				broadcastToSpectators = false;
		}));
	}



	#region flush log to disk
	public void flushByteLog(string filename, string extension = ""){
		if (gameLog.Count == 0)
			return;
		string fullFilename = getFullFilename (filename, extension);
		File.WriteAllBytes (fullFilename, gameLog.ToArray());
	}


	public void flushRawLog(string filename, string extension = ""){
		if (rawGameLog.Count == 0)
			return;
		string fullFilename = getFullFilename (filename, extension);
		File.WriteAllLines (fullFilename, rawGameLog.ToArray());
	}

	private string getFullFilename(string filename, string extension){
		if (Directory.Exists (gameHistoryFolder) == false)
			Directory.CreateDirectory (gameHistoryFolder);

		extension = extension.Insert (0, ".");
		string filePath = gameHistoryFolder + "/" + filename;
		int indexCounter = 0;
		string fullFilename;

		do {
			fullFilename = filePath + (indexCounter > 0 ? indexCounter.ToString () : "") + extension;
			indexCounter++;
		} while(File.Exists (fullFilename));

		return fullFilename;
	}
	#endregion




	#region PlayerInfo & Startmsg

	public void addInitLogMsg(List<ConnectedPlayer> players){
		string initMsg = "";
		foreach (ConnectedPlayer p in players)
			initMsg += ":" + p.color + " " + p.username + " " + p.iconNumber;
		pushToRawLog (initMsg);
	}

	#endregion
}
