using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlbotServer {

    public class PreGameSlotTypes : MonoBehaviour {

        private static Dictionary<GameType, List<PreGameSlotType>> gameP2Settings = new Dictionary<GameType, List<PreGameSlotType>>(){
            {GameType.Battleship, new List<PreGameSlotType>(){PreGameSlotType.Player, PreGameSlotType.TrainingBot,  PreGameSlotType.SelfClone}},
            {GameType.Bomberman, new List<PreGameSlotType>(){PreGameSlotType.Player, PreGameSlotType.TrainingBot, PreGameSlotType.Human }},
            {GameType.Breakthrough, new List<PreGameSlotType>(){PreGameSlotType.Player, PreGameSlotType.SelfClone }},
            {GameType.Connect4, new List<PreGameSlotType>(){PreGameSlotType.Player, PreGameSlotType.TrainingBot,  PreGameSlotType.SelfClone, PreGameSlotType.Human}},
            {GameType.Othello, new List<PreGameSlotType>(){PreGameSlotType.Player, PreGameSlotType.TrainingBot,  PreGameSlotType.SelfClone}},
            {GameType.Soldiers, new List<PreGameSlotType>(){PreGameSlotType.Player, PreGameSlotType.TrainingBot}},
            {GameType.Snake, new List<PreGameSlotType>(){PreGameSlotType.Player, PreGameSlotType.TrainingBot, PreGameSlotType.Human}},
            {GameType.BlockBattle, new List<PreGameSlotType>(){PreGameSlotType.Player, PreGameSlotType.Human}},
            {GameType.SpeedRunner, new List<PreGameSlotType>(){PreGameSlotType.Player, PreGameSlotType.Human}},
        };

        public static List<PreGameSlotType> getSlotOptions(GameType type) {return gameP2Settings[type];}
        public static PlayerInfo getSelfInfo(PlayerInfo i) { return new PlayerInfo() { iconNumber = i.iconNumber, username = "<" + i.username + ">" }; }
        public static PlayerInfo getHumanInfo(PlayerInfo i) { return new PlayerInfo() { iconNumber = i.iconNumber, username = "<" + "Human" + ">" }; }
    }

    public enum PreGameSlotType {
        Player,
        TrainingBot,
        SelfClone,
        Human,
        Empty,
    }
}