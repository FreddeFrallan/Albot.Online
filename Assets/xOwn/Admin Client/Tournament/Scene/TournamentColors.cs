using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tournament.Client {
    public class TournamentColors : MonoBehaviour {

        private static Color darkPurple = new Color((17f / 255f), (16f / 255f), (50f / 255f), 1f);
        private static Color RedTint = new Color((90f / 255f), (16f / 255f), (50f / 255f), 1f);
        private static Color SilverTint = new Color((162f / 255f), (162f / 255f), (162f / 255f), 1f);
        private static Color GoldTint = new Color((164f / 255f), (137f / 255f), (0f / 255f), 1f);
        private static Color paleOrange = new Color((255f / 255f), (146f / 255f), (53f / 255f), 1f);
        private static Color mutedPurBlue = new Color((36f / 255f), (35f / 255f), (74f / 255f), 1f);
        private static Color pureBlue = new Color((60f / 255f), (53f / 255f), (254f / 255f), 1f);
        private static Color lightBlue = new Color((82f / 255f), (231f / 255f), (254f / 255f), 1f);
        public static Color empty = darkPurple;
        public static Color emptyRedTint = RedTint;
        public static Color emptyGoldTint = GoldTint;
        public static Color idle = mutedPurBlue;
        public static Color lobbyNotReady = paleOrange;
        public static Color lobbyReady = lightBlue;
        public static Color playing = pureBlue;
        public static Color overLost = darkPurple;
        public static Color overLostRedTint = RedTint;
        public static Color overLostSilverTint = SilverTint;
        public static Color overWon = pureBlue;
        public static Color overOverGoldTint = GoldTint;
        public static Color lightText = Color.white;
        public static Color darkText = Color.black;
    }
}