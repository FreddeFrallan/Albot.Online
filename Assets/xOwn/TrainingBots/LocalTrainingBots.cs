using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using AlbotServer;


namespace Game{

	public class LocalTrainingBots : MonoBehaviour {

        public static readonly string botName = "Albot.Online";
        public static readonly int botIconNumber = 1;

		public static PlayerInfo StandardTrainingBotInfo = new PlayerInfo (){username = botName, iconNumber = botIconNumber };
        public static PlayerInfo StandardTrainingBotInfoCustomName(string name) { return new PlayerInfo() { username = name, iconNumber = botIconNumber }; }

		private static List<BotSetting> botSettings = new List<BotSetting>();

		public static void addBot(GameType type, int selectedBotMode = -1){
			ClientPlayersHandler.addPlayer (true, false, StandardTrainingBotInfo);

			TrainingBot targetBot = getMatchingBot (type);
			selectedBotMode = selectedBotMode < 0 ? targetBot.defaultSettings() : selectedBotMode;
			Dictionary<string, string> newSettings = targetBot.botSettings () [selectedBotMode];

			botSettings.Add (new BotSetting (StandardTrainingBotInfo.username) {settings = newSettings});
		}

		public static void resetBots(){botSettings.Clear ();}


		public static void initBots(GameType type, List<LocalPlayer> players){
			foreach (LocalPlayer p in players) {
				if (p.isNPC () == false)
					continue;

				BotSetting setting = botSettings.Find (x => x.username == p.info.username);
				p.bot = getMatchingBot (type);
				p.bot.initBot (setting.settings);
			}
		}
			
		private static TrainingBot getMatchingBot(GameType type){
			switch (type) {
			case GameType.Connect4:
				return new Connect4Bot.Connect4Bot ();
			case GameType.Othello:
				return new OthelloBot ();
			case GameType.Battleship:
				return new BattleshipsBot();
			case GameType.Soldiers:
				return new Soldiers.SoldiersTrainingBot ();
			case GameType.Bomberman:
				return new Bomberman.BombermanTrainingBot ();
			case GameType.Breakthrough:
				return new Breakthrough.BreakthroughTrainingBot ();
			case GameType.Snake:
				return new Snake.SnakeTrainingBot ();
			default:
				return null;
			}
		}

		//Not really the cleanest soloution since we acually create a new object just get the data.
		//Perhaps it should be static instead?
		public static List<string> getBotModeNames(GameType type){
			TrainingBot b = getMatchingBot (type);

			List<string> modeNames = new List<string> ();
			foreach(Dictionary<string, string> d in b.botSettings())
				modeNames.Add(d["Name"]);

			return modeNames;
		}
		public static int getStandardSettings(GameType type){return getMatchingBot (type).defaultSettings ();}
		public static string getBotSettingsName(GameType type, int setting){return getMatchingBot (type).botSettings () [setting] ["Name"];}

		private class BotSetting{
			public Dictionary<string, string> settings;
			public string username;
			public BotSetting(string username){this.username = username;}
		}
	}

}