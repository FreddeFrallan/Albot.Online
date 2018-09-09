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

        private static Color mutedPurBlue = new Color((36f/255f), (35f/255f), (74f/255f), 1f);
        private static Color pureBlue = new Color((60f / 255f), (53f / 255f), (254f / 255f), 1f);
        private Color emptyColor = mutedPurBlue;
        private Color idleColor = mutedPurBlue;
        private Color lobbyNotReadyColor = Color.yellow;
        private Color lobbyReadyColor = Color.green;
        private Color playingColor = Color.green;
        private Color overLostColor = mutedPurBlue;
        private Color overWonColor = pureBlue;

        private void Awake() {
            setBackgroundColor(null, RoundState.Empty);
        }

        public void setPlayer(TournamentPlayer player, RoundState state) {
            text.text = player.info.username;
            setBackgroundColor(player, state);
        }

        private void setBackgroundColor(TournamentPlayer player, RoundState state) {
            switch (state) {
            case RoundState.Empty: setColor(emptyColor); break;
            case RoundState.Idle: setColor(idleColor); break;
            case RoundState.Playing: setColor(emptyColor); break;
            case RoundState.Over:
            if (player.isWinning) {
                setColor(overWonColor);
            } else {
                setColor(overLostColor);
            }
            break;

            case RoundState.Lobby:
            if (player != null && player.isReady)
                setColor(lobbyReadyColor);
            else
                setColor(lobbyNotReadyColor);
            break;

            }
        }



        private void setColor(Color newColor) {theRenderer.material.color = newColor;}
        
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