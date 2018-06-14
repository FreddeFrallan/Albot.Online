using System;
using System.Collections.Generic;

namespace SnakeBot {
	public class Board {

        private JSONObject jPlayer;
        private Player[] players = new Player[2];
		private int[,] grid;

        public Board(int[,] oldGrid, Player[] oldPlayers, JSONObject oldJPLayer) {
            this.grid = (int[,])oldGrid.Clone();
            this.players = (Player[])oldPlayers.Clone();
            this.jPlayer = oldJPLayer;
        }
		public Board() {
			grid = new int[Constants.BOARD_SIZE, Constants.BOARD_SIZE];
			iterateGrid((x, y) => grid[x, y] = 0);
		}


        #region Utils
        private void iterateGrid (Action<int, int> f) {
			for (int y = 0; y < Constants.BOARD_SIZE; y++)
				for (int x = 0; x < Constants.BOARD_SIZE; x++)
					f(x, y);
		}
        #endregion



        public void handleUpdate (List<JSONObject> blocks, JSONObject player, JSONObject enemy) {
			foreach (JSONObject b in blocks)
				grid[(int)b.GetField(Constants.Fields.posX).n, (int)b.GetField(Constants.Fields.posY).n] = Constants.BLOCKED_BOARD_VALUE;
            this.jPlayer = player;

            insertPlayerPos(player, Constants.PLAYER_ID);
            insertPlayerPos(enemy, Constants.ENEMY_ID);
        }

		public void insertPlayerPos (JSONObject p, int id) {
            int posX = (int)p.GetField(Constants.Fields.posX).n;
            int posY = (int)p.GetField(Constants.Fields.posY).n;

            grid[posX, posY] = idToBoardValue(id);
            players[id].x = posX;
            players[id].y = posY;
		}

        public void printBoard() {
            Console.WriteLine("*************");
            Console.WriteLine("Player: " + players[0].x + "." + players[0].y);
            for (int y = 0; y < Constants.BOARD_SIZE; y++) {
                String row = "";
                for (int x = 0; x < Constants.BOARD_SIZE; x++)
                    row += grid[x, y] + "\t";
                Console.WriteLine(row);
            }
        }


        public Board deepCopy() {return new Board(grid, players, jPlayer);}
        public JSONObject getPlayerJObj() { return jPlayer; }
		public int[,] getGrid () {return grid;}
        public int idToBoardValue(int id) { return (id == 0 ? Constants.PLAYER_BOARD_VALUE : Constants.ENEMY_BOARD_VALUE); }
	}

    public struct Player {
        public int x, y;
        public int id;
    }
}
