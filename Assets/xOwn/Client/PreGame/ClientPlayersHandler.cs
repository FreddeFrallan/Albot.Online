using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlbotServer;
using Barebones.MasterServer;
using System.Linq;
using System.Threading;
using ClientUI;
using System;

namespace Game{

	public class ClientPlayersHandler{

		private static List<LocalPlayer> players = new List<LocalPlayer> ();
		private static List<PlayerColor> currentPlayerQ = new List<PlayerColor>();
		private static List<TrainingBot> currentBots = new List<TrainingBot> ();



		private static ClientController theClientController;
		public static void init(ClientController c) {
			theClientController = c;
			LocalTrainingBots.initBots (theClientController.getGameType (), players);
			foreach (LocalPlayer p in players)
				if (p.isNPC ())
					p.bot.initClientController (c);
		}

		public static void addUIStateListner(){
			ClientUIOverlord.onUIStateChanged += ((ClientUIStates newState) => {if (newState == ClientUIStates.GameLobby)resetLocalPLayers (); });
		}


		public static PlayerInfo[] generatePlayersInfoArray(){return players.Select (iten => iten.info).ToArray ();}
		public static void addPlayer(bool isNPC, bool isHuman, PlayerInfo info){players.Add(new LocalPlayer(isNPC, isHuman, info));}
		public static void resetLocalPLayers(){
			currentPlayerQ.Clear ();
			players.Clear ();
			LocalTrainingBots.resetBots ();
		}

		public static void onReceiveServerMsg(string msg, PlayerColor color){
			LocalPlayer p = players.Find (x => x.info.color == color);
			if (p == null) {
				Debug.LogError ("Got a msg for: " + color + "   But got no stored players matching that color");
				return;
			}
			if (p.isNPC () == false) 
				currentPlayerQ.Add (p.info.color);
			
			p.takeInput (msg);
		}


		public static void onReceiveLocalTCPMsg(string msg){
			if (currentPlayerQ.Count == 0)
				return;

			LocalPlayer currentPlayer = players.Find (x => x.info.color == getCurrentPlayerColor());
			if (currentPlayer == null) {
				Debug.LogError ("Got a local TCP msg but currentPlayer == NULL");
				return;
			}
			currentPlayerQ.RemoveAt (0);
			theClientController.onOutgoingLocalMsg (msg, currentPlayer.info.color);
		}

		public static void initPlayerColor(string username, PlayerColor color){
			LocalPlayer p = players.Find (x => x.info.username == username);
			if (p == null) {
				Debug.LogError ("Init msg for " + username + "   But got no stored players matching that username");
				return;
			}
			p.info.color = color;
			if (p.isNPC())
				p.bot.initColor (p.info.color);
		}


		public static void addSelf(){
			AccountInfoPacket playerInfo = ClientUI.ClientUIOverlord.getCurrentAcountInfo ();
			ClientPlayersHandler.addPlayer (false, false, new AlbotServer.PlayerInfo {
				username = playerInfo.Username,
				iconNumber = int.Parse(playerInfo.Properties ["icon"])
			});
		}
		public static void addClone(){
			AccountInfoPacket playerInfo = ClientUI.ClientUIOverlord.getCurrentAcountInfo ();
			ClientPlayersHandler.addPlayer (false, false, new AlbotServer.PlayerInfo {
				username = "<" + playerInfo.Username + ">",
				iconNumber = int.Parse(playerInfo.Properties ["icon"])
			});
		}
		public static void addHuman(){
			AccountInfoPacket playerInfo = ClientUI.ClientUIOverlord.getCurrentAcountInfo ();
			ClientPlayersHandler.addPlayer (false, true, new AlbotServer.PlayerInfo {
				username = "<Human>",
				iconNumber = int.Parse(playerInfo.Properties ["icon"])
			});
		}


		public static bool hasLocalPlayerOfColor(PlayerColor color){return players.Find (x => x.info.color == color) != null;}
		public static LocalPlayer getPlayerFromColor(PlayerColor color){return players.Find (x => x.info.color == color);}
		public static LocalPlayer getCurrentPlayer(){return getPlayerFromColor (getCurrentPlayerColor ());}
		public static bool hasRequestedPlayerMoves(){return currentPlayerQ.Count > 0;}
		public static PlayerColor getCurrentPlayerColor(){return currentPlayerQ [0];}
		public static PlayerColor sendFromCurrentPlayer(){
			PlayerColor temp = getCurrentPlayerColor ();
			currentPlayerQ.RemoveAt (0);
			return temp;
		}

		public static void killBots(){
			foreach (TrainingBot b in currentBots)
				b.killBot ();
			currentBots.Clear ();
		}
	}



	public class LocalPlayer{
		public bool NPC = false, Human = false;
		public bool isNPC(){return NPC;}
		public PlayerInfo info;
		public TrainingBot bot;

		public LocalPlayer(bool isBot, bool isHuman, PlayerInfo info){
			this.NPC = isBot; this.Human = isHuman; this.info = info;
		}

		public void takeInput(string msg){
			if (NPC == false)
				TCPLocalConnection.sendMessage (msg);
			else
				bot.onReceiveInput (msg);
		}

		public Action<string> getTakeInputFunc(){
			if (Human)
				return ((string s) => {}); 
			else if (NPC == false)
				return TCPLocalConnection.sendMessage;
			else
				return bot.onReceiveInput;
		}

		public bool isMainPlayer(){
			return !(NPC || Human);
		}
	}


}