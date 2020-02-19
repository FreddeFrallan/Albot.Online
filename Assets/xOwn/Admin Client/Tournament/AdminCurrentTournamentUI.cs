using Barebones.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;
using AlbotServer;
using System.Linq;
using Tournament;

namespace AdminUI {

    public class AdminCurrentTournamentUI : MonoBehaviour {

        public GameObject playerSlotPrefab;
        public GameObject playerList;
        private List<GameObject> currentPlayerSlots = new List<GameObject>();


        private void Start() {
            Msf.Connection.SetHandler((short)CustomMasterServerMSG.preTournamentUpdate, handlePreTournamentUpdate);
        }

        public void handlePreTournamentUpdate(IIncommingMessage rawMsg) {
            TournamentInfoMsg info = rawMsg.Deserialize<TournamentInfoMsg>();
            showList(info.players);
        }

        public void showList(PlayerInfo[] playerNames) {
            currentPlayerSlots.ForEach(p => Destroy(p));
            currentPlayerSlots.Clear();

            foreach(string s in playerNames.Select(p => p.username)) {
                GameObject temp = Instantiate(playerSlotPrefab, playerList.transform);
                temp.GetComponent<PreTournamentListItem>().init(s);
                currentPlayerSlots.Add(temp);
            }
        }

    }

}