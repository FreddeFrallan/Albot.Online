using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using System;

namespace SpeedRunner {

    public class SpeedRunnerProtocol : CommProtocol {

        public enum MsgType : short {
            gameInfo = 800,
            gameInit = 801,
            gameUpdate = 802,
            playerMove = 803,
        }

        public enum PlayerCommand {
            Jump,
        }

        public override void init(Action<object, int, short> sendMsgFunc) {
            this.sendMsg = sendMsgFunc;


        }

        public void sendGameUpdate(int targetID, GameUpdate board) { sendMsg(board, targetID, (short)MsgType.gameUpdate); }

    }

    [Serializable]
    public class InitSettings {
        public int mapSeed;
        public InitSettings(int seed) {
            mapSeed = seed;
        }
    }

    [Serializable]
    public class GameUpdate {
        public string updateMsg = "Not yet implimented";
    }

    [Serializable]
    public class GameCommand {
        public SpeedRunnerProtocol.PlayerCommand command;
        public Game.PlayerColor myColor;
        public GameCommand(Game.PlayerColor myColor, SpeedRunnerProtocol.PlayerCommand command) {
            this.command = command;
            this.myColor = myColor;
        }
    }
}