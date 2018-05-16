using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using Barebones.MasterServer;
using AlbotServer;

namespace ClientUI{

	public class PreTrainingLobby : PreGameBaseLobby {

	
		private bool hasStoredSettings = false;
		private int savedSettings;


		void Start(){
			ClientUIOverlord.onUIStateChanged += (ClientUIStates newState) => {if(newState != ClientUIStates.PreGame) resetPanel();};
			TCPLocalConnection.subscribeToTCPStatus (localBotStatusChanged);
		}


		public override void initPreGameLobby (string gameTittle, Sprite gameSprite, PreGamePlayer[] players, bool isAdmin, int roomId, GameType type){
			base.initPreGameLobby (gameTittle, gameSprite, players, isAdmin, roomId, type);

			p2Settings.ClearOptions ();
			p2Settings.AddOptions (LocalTrainingBots.getBotModeNames (type));
			p2Settings.value = LocalTrainingBots.getStandardSettings (type);
			hasStoredSettings = false; 
		}



		public override void setLocalPreGamePlayers (){
			base.setLocalPreGamePlayers ();
			ClientPlayersHandler.addSelf ();

			if (hasStoredSettings == false) {
				savedSettings = p2Settings.value;
				hasStoredSettings = true;
			}

			//Hardcoded check if the Bot Setting is actually Human
			print("Name: " + LocalTrainingBots.getBotSettingsName(type, savedSettings));
			if (LocalTrainingBots.getBotSettingsName (type, savedSettings) == "Human") {
				ClientPlayersHandler.addHuman ();
			}
			else
				LocalTrainingBots.addBot (type, savedSettings);
		}

		public override void onStartClick (){
			Msf.Connection.SendMessage((short)ServerCommProtocl.StartPreGame, new PreGameStartMsg(){roomID = roomId});
		}
			
	}

}