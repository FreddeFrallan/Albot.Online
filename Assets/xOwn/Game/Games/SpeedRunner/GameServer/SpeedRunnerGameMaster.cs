using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace SpeedRunner {
    public class SpeedRunnerGameMaster : Game.GameMaster {

        private SpeedRunnerProtocol protocol = new SpeedRunnerProtocol();
        private SpeedRunnerUpdater updater;

        public override GameType getGameType() {return GameType.SpeedRunner;}
        public override CommProtocol getProtocol() {return protocol;}
        public override bool isRealtime() {return true;}
        public override int maxNbrPlayers() {return 2;}
        public override void onPlayerLeft(ConnectedPlayer newPlayer) {
            
        }

        public override void startGame() {
            throw new NotImplementedException();
        }

        protected override void initProtocol(Action<object, int, short> sendMsgFunc) {
            protocol.init(sendMsgFunc);
            colorOrder = new List<PlayerColor>() { PlayerColor.White, PlayerColor.Red };
            updater = new SpeedRunnerUpdater(players, this, protocol);
        }
    }
}