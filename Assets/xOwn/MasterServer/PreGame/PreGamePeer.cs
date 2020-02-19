using Barebones.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlbotServer {

    public class PreGamePeer {

        public List<int> playerSlots = new List<int>();
        public bool isReady = false;
        public PlayerInfo info;
        public IPeer peer;

        private Action<PreGamePeer> onPeerLeftCall;


        public PreGamePeer(IPeer peer, PlayerInfo info, Action<PreGamePeer> onPeerLeftCall) {
            this.peer = peer;
            this.info = info;
            this.onPeerLeftCall = onPeerLeftCall;

            peer.Disconnected += onPeerLeft;
        }

        #region Leave & Remove
        private void removePlayer() {
            peer.SendMessage((short)ServerCommProtocl.PreGameKick);
            onPeerLeft(peer);
        }

        public void clearReferences() {peer.Disconnected -= onPeerLeft;}
        private void onPeerLeft(IPeer peer) {onPeerLeftCall(this);}
        #endregion

        #region PlayerSlots
        public void removePlayerSlot(int slotID) {
            playerSlots.Remove(slotID);
            if (playerSlots.Count == 0)
                removePlayer();
        }

        public void addPlayerSlot(int slotID) {
            playerSlots.Add(slotID);
        }
        #endregion
    }
}