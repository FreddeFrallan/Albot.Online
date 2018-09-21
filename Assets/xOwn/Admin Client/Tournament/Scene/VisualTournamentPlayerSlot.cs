using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Tournament.Server;

namespace Tournament.Client {

    public class VisualTournamentPlayerSlot : MonoBehaviour {

        [SerializeField]
        private MeshRenderer theRenderer;
        [SerializeField]
        private TextMeshPro text;

        private static Color darkPurple = new Color((17f / 255f), (16f / 255f), (50f / 255f), 1f);
        private static Color paleOrange = new Color((255f / 255f), (146f / 255f), (53f / 255f), 1f);
        private static Color mutedPurBlue = new Color((36f / 255f), (35f / 255f), (74f / 255f), 1f);
        private static Color pureBlue = new Color((60f / 255f), (53f / 255f), (254f / 255f), 1f);
        private static Color lightBlue = new Color((82f / 255f), (231f / 255f), (254f / 255f), 1f);
        private Color emptyColor = darkPurple;
        private Color idleColor = mutedPurBlue;
        private Color lobbyNotReadyColor = paleOrange;
        private Color lobbyReadyColor = lightBlue;
        private Color playingColor = pureBlue;
        private Color overLostColor = darkPurple;
        private Color overWonColor = pureBlue;
        private Color lightTextColor = Color.white;
        private Color darkTextColor = Color.black;

        private void Awake() {
            setBackgroundColor(null, RoundState.Empty);
        }

        public void setPlayer(TournamentPlayer player, RoundState state) {
            text.text = player.info.username;
            setBackgroundColor(player, state);
        }

        private void setBackgroundColor(TournamentPlayer player, RoundState state) {
            switch (state) {
                case RoundState.Empty: setColor(emptyColor, false); break;
                case RoundState.Idle: setColor(idleColor, false); break;
                case RoundState.Playing: setColor(playingColor, false); break;
                case RoundState.Over:setColor(player.isWinning ? overWonColor : overLostColor, false);break;
                case RoundState.Lobby:
                if (player != null && player.isReady)
                    setColor(lobbyReadyColor, true);
                else
                    setColor(lobbyNotReadyColor, true);
                break;
            }
        }

        private void setColor(Color newColor, bool darkText) {
            theRenderer.material.color = newColor;
            text.color = (darkText ? darkTextColor: lightTextColor);
        }

        /*
        public void setState(RoundState state) {
            this.state = state;

            switch (state) {
            case RoundState.Idle: setColor(Color.white); break;
            case RoundState.Lobby: setColor(Color.blue); break;
            case RoundState.Playing: setColor(Color.red); break;
            case RoundState.Over: setColor(Color.yellow); break;
            }
        }
        */
    }
}