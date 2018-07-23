using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using AlbotServer;
using Barebones.Networking;
using UnityEngine.Networking;

namespace Tournament.Server {

    public class TournamentTree{
        private List<TournamentRound> currentGames = new List<TournamentRound>();
        private List<List<TournamentRound>> treeStructure = new List<List<TournamentRound>>();
        private List<TournamentPlayer> players;
        private TournamentRoundDTO[][] treeDTO;
        public int layers { get; private set; }

        public TournamentTree(List<TournamentPlayer> players) {
            this.players = players;
            layers = (int)Mathf.Ceil(Mathf.Log(players.Count, 2));
            layers = layers <= 0 ? 1 : layers;

            treeStructure = TournamentTreeGenerator.generateTreeStructure(layers);
            TournamentTreeGenerator.addBots(treeStructure, players);
            TournamentTreeGenerator.insertPlayersRandomly(treeStructure, players);
            createTreeDTOBlueprint();
        }

        public void playGame(int col, int row) {treeStructure[col][row].startGame();}
        public void playRow(int row) {
            foreach (TournamentRound g in treeStructure[row])
                g.startGame();
        }


        public TournamentTreeRow[] getFullTreeDTO() {return Enumerable.Range(0, treeDTO.Length).Select(l => createRowDTO(l)).ToArray();}
        public TournamentTreeRow createRowDTO(int row) {
            for (int game = 0; game < treeStructure[row].Count; game++)
                treeDTO[row][game] = treeStructure[row][game].createDTO();

            return new TournamentTreeRow() { treeRow = treeDTO[row]};
        }

        public void createTreeDTOBlueprint() {
            treeDTO = new TournamentRoundDTO[layers][];
            for (int i = 0; i < treeStructure.Count; i++)
                treeDTO[i] = new TournamentRoundDTO[treeStructure[i].Count];
        }

        public List<List<TournamentRound>> getTree() { return treeStructure; }
    }


    public class TournamentTreeRow : MessageBase {
        public TournamentRoundDTO[] treeRow;
        public int rowIndex;
    }



    public class TournamentPlayer {
        public IPeer peer;
        public PlayerInfo info;
        private bool isNPC = false;

        public TournamentPlayer(bool isNPC = false, int botNumber = 0) {
            this.isNPC = isNPC;
            if (isNPC)
                info = new PlayerInfo() { username = "Bot: " + botNumber.ToString() };
        }

        public bool getIsNPC() { return isNPC; }
    }

    public enum GameState {
        Playing,
        Lobby,
        Empty,
        Over,
    }
}