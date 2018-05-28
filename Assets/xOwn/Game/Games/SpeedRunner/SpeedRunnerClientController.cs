using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace SpeedRunner {

    public class SpeedRunnerClientController : Game.ClientController {


        private SpeedRunnerProtocol protocol;
        private SpeedRunnerRenderer localRenderer;

        public override GameType getGameType() { return GameType.SpeedRunner; }
        public override void initProtocol(CommProtocol protocol) {this.protocol = (SpeedRunnerProtocol)protocol;}

        public override void onOutgoingLocalMsg(string msg, PlayerColor color) {
            sendServerMsg(msg, (short)SpeedRunnerProtocol.MsgType.playerMove);
        }


        protected override void initHandlers() {
            base.initHandlers();

            StartCoroutine(findAndInitRenderer<SpeedRunnerRenderer>((x) => localRenderer = x));
            StartCoroutine(handleNetworkMsgQueue());
            RealtimeTCPController.resetController();

            TCPMessageQueue.readMsgInstant = readTCPMsg;
        }

    }
}