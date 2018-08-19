using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace AlbotServer {

    public class InvalidNames {

        private static int nameMinLenght = 1, nameMaxLenght = 10;
        private static List<string> illegalChars = new List<string>() { "<", ">" };

        public static bool isValidName(string username) {
            string lowerCaseName = username.ToLower();

            if (isValidLenght(username) == false)
                return false;
            if (isTrainingBotName(lowerCaseName))
                return false;
            if (containsIllegalChars(username))
                return false;

            return true;
        }


        private static bool containsIllegalChars(string username) {
            foreach (string c in illegalChars)
                if (username.Contains(c))
                    return true;
            return false;
        }

        private static bool isValidLenght(string username) {return username.Length >= nameMinLenght && username.Length >= nameMinLenght;}
        private static bool isTrainingBotName(string username){return username == LocalTrainingBots.botName.ToLower();}
    }
}