using AlbotServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game {

    public class GameLogProtocol {

        public class Protocol {
            public const string players = "Players";

            public const string moves = "Moves";
            public const string moveID = "MoveID";
        }

        public static string generatePlayerInfoHeader(PlayerInfo[] players) {
            string msg = "";
            foreach (PlayerInfo p in players)
                msg += string.Format("{0} {1} {2} ", p.username, p.iconNumber, p.color);
            return msg;
        }

        public static PlayerInfo[] parsePlayerInfoHeader(string msg) {
            string[] words = msg.Split(' ');

            List<PlayerInfo> players = new List<PlayerInfo>();
            for (int i = 0; i + 2 < words.Length; i += 3)
                players.Add(parsePlayer(words[i], words[i + 1], words[i + 2]));

            return players.ToArray();
        }

        private static PlayerInfo parsePlayer(string s1, string s2, string s3) {
            return new PlayerInfo() {
                username = s1,
                iconNumber = int.Parse(s2),
                color = GameUtils.stringToColor(s3)
            };
        }
    }
}