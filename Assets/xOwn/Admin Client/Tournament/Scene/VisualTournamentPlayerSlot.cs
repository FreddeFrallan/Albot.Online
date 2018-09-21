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
        private RoundType slotType;

        private void Awake() {
            setBackgroundColor(null, RoundState.Empty);
        }

        public void setPlayer(TournamentPlayer player, RoundState state) {
            text.text = player.info.username;
            setBackgroundColor(player, state);
        }

        public void setRoundType(RoundType roundType) {
            slotType = roundType;
            setBackgroundColor(null, RoundState.Empty);
        }

        private void setBackgroundColor(TournamentPlayer player, RoundState state) {
            switch (state) {
                case RoundState.Empty:
                if (slotType == RoundType.final) {
                    setColor(TournamentColors.emptyGoldTint, true);
                }
                else if(slotType == RoundType.loser) {
                    setColor(TournamentColors.emptyRedTint, false);
                }
                else {
                    setColor(TournamentColors.empty, false);
                }
                break;
                case RoundState.Idle: setColor(TournamentColors.idle, false); break;
                case RoundState.Playing: setColor(TournamentColors.playing, false); break;
                case RoundState.Over:
                if (player.isWinning) {
                    if (slotType == RoundType.final)
                        setColor(TournamentColors.overOverGoldTint, true);
                    else {
                        setColor(TournamentColors.overWon, false);
                    }
                }
                else {
                    if (slotType == RoundType.loser)
                        setColor(TournamentColors.overLostRedTint, false);
                    else if (slotType == RoundType.final)
                        setColor(TournamentColors.overLostSilverTint, true);
                    else
                        setColor(TournamentColors.overLost, false);
                }
                break;
                case RoundState.Lobby:
                if (player != null && player.isReady)
                    setColor(TournamentColors.lobbyReady, true);
                else
                    setColor(TournamentColors.lobbyNotReady, true);
                break;
            }
        }

        private void setColor(Color newColor, bool darkText) {
            theRenderer.material.color = newColor;
            text.color = (darkText ? TournamentColors.darkText : TournamentColors.lightText);
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