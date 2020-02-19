using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace SpeedRunner {

    public class SpeedRunnerServerController : Game.ServerController {

        private SpeedRunnerGameMaster controller = new SpeedRunnerGameMaster();

        public override GameMaster getController() {return controller;}
        public override void initController(Action<string> printFunc, Action<object, int, short> sendMsgFunc, Action shutdownGameServer, GameWrapper wrapper, List<string> preGamePlayers) {

            controller.init(printFunc, sendMsgFunc, shutdownGameServer, wrapper, preGamePlayers);
        }
    }
}