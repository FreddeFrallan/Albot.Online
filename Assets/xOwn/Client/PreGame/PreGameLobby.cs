using System.Collections;
using System.Collections.Generic;
using Barebones.MasterServer;
using Game;
using UnityEngine;
using UnityEngine.UI;
using AlbotServer;
using Barebones.Networking;


namespace ClientUI{

	public class PreGameLobby : PreGameBaseLobby{

		private Dictionary<GameType, List<Player2Setting>> gameP2Settings = new Dictionary<GameType, List<Player2Setting>>(){
			{GameType.Battleship, new List<Player2Setting>(){Player2Setting.Opponent, Player2Setting.Computer, Player2Setting.Self}},
			{GameType.Bomberman, new List<Player2Setting>(){Player2Setting.Opponent, Player2Setting.Computer, Player2Setting.Human }},
			{GameType.Breakthrough, new List<Player2Setting>(){Player2Setting.Opponent, Player2Setting.Self }},
			{GameType.Connect4, new List<Player2Setting>(){Player2Setting.Opponent, Player2Setting.Computer, Player2Setting.Self, Player2Setting.Human}},
			{GameType.Othello, new List<Player2Setting>(){Player2Setting.Opponent, Player2Setting.Computer, Player2Setting.Self}},
			{GameType.Soldiers, new List<Player2Setting>(){Player2Setting.Opponent, Player2Setting.Computer}},
			{GameType.Snake, new List<Player2Setting>(){Player2Setting.Opponent, Player2Setting.Computer, Player2Setting.Human}},
			{GameType.BlockBattle, new List<Player2Setting>(){Player2Setting.Opponent, Player2Setting.Human}},
            {GameType.SpeedRunner, new List<Player2Setting>(){Player2Setting.Opponent, Player2Setting.Human}},
        };

		private List<Player2Setting> currentSettings;



		//Called when we enter a preGame 
		void Start(){
			if (hasInit)
				return;
			hasInit = true;
			ClientUIOverlord.onUIStateChanged += (ClientUIStates newState) => {if(newState != ClientUIStates.PreGame) resetPanel();};
			TCPLocalConnection.subscribeToTCPStatus(localBotStatusChanged);

			p2Settings.onValueChanged.AddListener (newp2SettingSelected);
			gameCreator.setCurrentPreLobby (this);
		}

		public override void initPreGameLobby (string gameTittle, Sprite gameSprite, PreGamePlayer[] players, bool isAdmin, int roomId, GameType type){
			base.initPreGameLobby (gameTittle, gameSprite, players, isAdmin, roomId, type);
			handlers.Add(Msf.Connection.SetHandler((short)ServerCommProtocl.PreGameKick, handlePreGameKickMsg));
			initP2Settings (type);
		}

		private void initP2Settings(GameType type){
			currentSettings = gameP2Settings [type];

			p2Settings.ClearOptions ();
			List<string> options = new List<string> ();
			foreach (Player2Setting setting in currentSettings)
				options.Add (setting.ToString ());
			p2Settings.AddOptions (options);
		}


		public override void onStartClick(){Msf.Connection.SendMessage((short)ServerCommProtocl.StartPreGame, new PreGameStartMsg(){roomID = roomId});}
		private void handlePreGameKickMsg(IIncommingMessage message){ClientUIStateManager.requestGotoGameLobby ();}

		public override void setLocalPreGamePlayers (){
			base.setLocalPreGamePlayers ();
			ClientPlayersHandler.addSelf ();
			if (isAdmin == false)
				return;

			Player2Setting selectedSetting = currentSettings [p2Settings.value];
			if (selectedSetting == Player2Setting.Computer)
				LocalTrainingBots.addBot (type);
			else if (selectedSetting == Player2Setting.Self)
				ClientPlayersHandler.addClone ();
			else if (selectedSetting == Player2Setting.Human)
				ClientPlayersHandler.addHuman ();
		}


		public void newp2SettingSelected(int value){
			if (isAdmin == false) //Should never happen
				return;

			PlayerInfo newPlayerInfo = new PlayerInfo();
			PreGameSlotType type = PreGameSlotType.Empty;
			Player2Setting selectedSetting = currentSettings [value];


			if (selectedSetting == Player2Setting.Computer)
				type = PreGameSlotType.TrainingBot;
			else if (selectedSetting == Player2Setting.Self) {
				type = PreGameSlotType.AdminSelfClone;
				AccountInfoPacket ac = ClientUIOverlord.getCurrentAcountInfo ();
				newPlayerInfo.username = "<" +ac.Username + ">";
				newPlayerInfo.iconNumber = int.Parse(ac.Properties ["icon"]);
			}
			else if (selectedSetting == Player2Setting.Human) {
				type = PreGameSlotType.AdminHuman;
				AccountInfoPacket ac = ClientUIOverlord.getCurrentAcountInfo ();
				newPlayerInfo.username = "<Human>";
				newPlayerInfo.iconNumber = int.Parse(ac.Properties ["icon"]);
			}
				
			Msf.Connection.SendMessage((short)ServerCommProtocl.SlotTypeChanged, new PreGameSlotSTypeMsg(){slotID = 1, type = type, roomID = roomId, newPlayerInfo = newPlayerInfo });
		}

	}


	public enum Player2Setting{
		Opponent = 0,
		Computer = 1,
		Self = 2,
		Human = 3,
	}
}