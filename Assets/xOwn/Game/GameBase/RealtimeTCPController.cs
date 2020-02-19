using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game{

	public class RealtimeTCPController{

		private static List<LocalRealtimePlayer> localPlayers = new List<LocalRealtimePlayer>();
		private static LocalRealtimePlayer TCPPlayer;

		public static void resetController(){
			localPlayers.Clear ();
			TCPPlayer = null;
		}


		public static void registerLocalPlayer(int team, Action<string> sendNewBoard, bool isTCPPLayer = false){
			localPlayers.Add (new LocalRealtimePlayer (team, sendNewBoard));

			if (isTCPPLayer)
				TCPPlayer = localPlayers [localPlayers.Count - 1];
		}


		public static void gotNewBoard(int team, string board){
			localPlayers.Find (x => x.team == team).inputNewBoard (board);
		}

		public static void requestBoard(int team, bool isTCPPlayer = false){
			if (isTCPPlayer)
				TCPPlayer.requstNewBoard ();
			else
				localPlayers.Find (x => x.team == team).requstNewBoard ();
		}





		private class LocalRealtimePlayer{
			public int team;
			private Action<string> sendNewBoard;
			private bool waitingForNewBoard = true, gotNewBoard = false;
			private string lastBoard = "";

			public LocalRealtimePlayer(int team, Action<string> sendNewBoard){
				this.team = team; this.sendNewBoard = sendNewBoard;
			}

			public void inputNewBoard(string msg){
				lastBoard = msg;
				if (waitingForNewBoard)
					sendNewBoard (lastBoard);
				else
					gotNewBoard = true;
				
				waitingForNewBoard = false;
			}

			public void requstNewBoard(){
				if (gotNewBoard) {
					sendNewBoard (lastBoard);
					gotNewBoard = false;
				} else
					waitingForNewBoard = true;
			}
		}
	}

}