using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlbotServer{

	public class InduvidualGameData{
		public static Dictionary<Game.GameType, GameSettings> games = new Dictionary<Game.GameType, GameSettings>(){
			{Game.GameType.Battleship, new GameSettings(2, "Battleships", "BattleshipsGame")},
			{Game.GameType.Connect4, new GameSettings(2, "Connect4", "Connect4Game")},
			{Game.GameType.Othello, new GameSettings(2, "Othello", "OthelloGame")},
			{Game.GameType.Soldiers, new GameSettings(2, "Soldiers", "SoldiersGame", true)},
			{Game.GameType.Bomberman, new GameSettings(2, "Bomberman", "BombermanGame", true)},
			{Game.GameType.Breakthrough, new GameSettings(2, "Brakethrough", "BrakethroughGame")},
			{Game.GameType.Snake, new GameSettings(2, "Snake", "SnakeGame", true)},
			{Game.GameType.BlockBattle, new GameSettings(2, "BlockBattle", "BlockBattleGame", true)},
            {Game.GameType.SpeedRunner, new GameSettings(2, "SpeedRunner", "SpeedRunnerGame", true)},
        };
	}


	public class GameSettings{
		public int maxPlayers;
		public string mapName, sceneName;
		public bool isRealTime;
		public GameSettings(int maxPlayers, string mapName, string sceneName, bool isRealTime = false){
			this.maxPlayers = maxPlayers;
			this.mapName = mapName;
			this.sceneName = sceneName;
			this.isRealTime = isRealTime;
		}
	}

}