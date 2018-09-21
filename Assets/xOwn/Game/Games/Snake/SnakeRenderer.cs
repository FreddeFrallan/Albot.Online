using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using Barebones.MasterServer;

namespace Snake{

	public class SnakeRenderer : GameRenderer {

		public SnakeSmoothRenderer smoothRenderer;
		public GameObject bgBrick, collisionBlock, gridContainer;
		public Material redHead, redBody, blueHead, blueBody;

		private float blockSize = 1.02f;
		private SnakeBlock[,] blocks;
		private bool isInit = false;
        private int gridSize;

		// Use this for initialization
		void Start () {
            gridSize = SnakeGameLogic.GRID_SIZE;
			blocks = new SnakeBlock[gridSize, gridSize];
			createBackground ();
			isInit = true;
		}

		private void createBackground(){
			for (int z = 0; z < gridSize; z++)
				for (int x = 0; x < gridSize; x++) {
					float posX = -(gridSize * blockSize / 2) + blockSize * x;
					float posZ = -(gridSize * blockSize / 2) + blockSize * z;
					GameObject temp = Instantiate (bgBrick, new Vector3(posX, 0, posZ), bgBrick.transform.rotation);
                    temp.transform.parent = gridContainer.transform;
					blocks [x, z] = temp.GetComponent<SnakeBlock> ();
				}
		}

		//The protocol should later be re-written so we can loop through all the players
		public void handleBoardUpdate(BoardUpdate updateMsg){
			smoothRenderer.handleGameUpdate (updateMsg);
		}



		public void displayCrash(Vector2 crashPos){
            Position2D extraPos = new Position2D() { x = 0, y = 0 };
            Position2D pos = setBlockCoords(crashPos, ref extraPos);

            SnakeBlock block = getBlockFromPos(pos);
            Vector3 spawnPos = block.transform.position + new Vector3 (extraPos.x, 2, extraPos.y);
			Instantiate (collisionBlock, spawnPos, Quaternion.identity);
		}

        public void explodeLoser(PlayerColor loser) {
            if (loser == PlayerColor.None) {
                smoothRenderer.explodePlayer(0);
                smoothRenderer.explodePlayer(1);
            } else if (loser == PlayerColor.Red)
                smoothRenderer.explodePlayer(1);
            else
                smoothRenderer.explodePlayer(0);
        }

		private Position2D setBlockCoords(Vector2 coord, ref Position2D extraCoord){
			if (coord.x < 0)
                extraCoord.x = -1;
            if(coord.x >= gridSize)
                extraCoord.x = 1;
            if (coord.y < 0)
                extraCoord.y = 1;
            if (coord.y >= gridSize)
                extraCoord.y = -1;

            return new Position2D() {x = (int)Mathf.Clamp(coord.x, 0, gridSize-1), y = (int)Mathf.Clamp(coord.y, 0, gridSize - 1) };
		}


        public SnakeBlock getBlockFromPos(Position2D p) {return blocks[p.x, gridSize - p.y -1];}
		public SnakeBlock getBlockFromNumber(int coord){
			int x = coord % gridSize;
			int y = gridSize - (coord / gridSize);
			return blocks [x, y];
		}

        protected override void displayNewUpdate(GameLogState update) {
            if (gameOver)
                return;

            BoardUpdate state = SnakeGameLogProtocol.parseGameState(update.log[0]);
            if (state.gameOver)
                handleAdminGameOver(state);
            else
                handleBoardUpdate(state);
        }

        public void handleAdminGameOver(BoardUpdate state) {
            gameOver = true;
            if (state.winnerColor == PlayerColor.None) {
                smoothRenderer.explodePlayer(0);
                smoothRenderer.explodePlayer(1);
            } else if (state.winnerColor == PlayerColor.Red)
                smoothRenderer.explodePlayer(1);
            else
                smoothRenderer.explodePlayer(0);
        }
    }

}