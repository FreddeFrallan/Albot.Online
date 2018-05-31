using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace SpeedRunner {

    public class SpeedRunnerClientController : Game.ClientController {


        private SpeedRunnerProtocol protocol;
        private SpeedRunnerRenderer localRenderer;
        public Player localPlayer;

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

            localPlayer = FindObjectOfType<Player>();
            TCPMessageQueue.readMsgInstant = instantReadTCPMsg;
        }


        private void instantReadTCPMsg(ReceivedLocalMessage msg) {
            if (msg.message == "JUMP")
                localPlayer.activateJump();

            MainThread.fireEventAtMainThread(() => {
                print("Sending msg");
                TCPLocalConnection.sendMessage(formatBoard());
            } );
        }

        private string formatBoard() {
            JSONObject jObj = new JSONObject();
            
            string[] vision = SpeedRunnerPlayerVision.singleton.generateSnapshot();

            List<JSONObject> jRows = new List<JSONObject>();
            foreach (string s in vision) {
                JSONObject temp = new JSONObject();
                temp.type = JSONObject.Type.STRING;
                temp.str = s;
                jRows.Add(temp);
            }
            
            jObj.AddField("Msg", "Empty");
            jObj.AddField("Board", new JSONObject(jRows.ToArray()));
            //jObj.AddField("Board", jRows);
            return jObj.Print();
        }
    }
}