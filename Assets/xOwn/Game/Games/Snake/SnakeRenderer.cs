using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snake{

	public class SnakeRenderer : MonoBehaviour {

		public SnakeSmoothRenderer smoothRenderer;
		public GameObject bgBrick, collisionBlock;
		public Material redHead, redBody, blueHead, blueBody;

		private int gridSize = 20;
		private float blockSize = 1.15f;
		private SnakeBlock[,] blocks;
		private bool isInit = false;

		// Use this for initialization
		void Start () {
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
					blocks [x, z] = temp.GetComponent<SnakeBlock> ();
				}
		}

		//The protocol should later be re-written so we can loop through all the players
		public void handleBoardUpdate(BoardUpdate updateMsg){
			smoothRenderer.handleGameUpdate (updateMsg);
		}



		public void displayCrash(Vector2 crashPos){
			int extraX = 0, extraY = 0; 
			int posX = setBlockCoord ((int)crashPos.x, ref extraX);
			int posY = setBlockCoord ((int)crashPos.y, ref extraY);


			SnakeBlock block = blocks [posX, posY];
			Vector3 spawnPos = block.transform.position + new Vector3 (extraX, 2, extraY);
			Instantiate (collisionBlock, spawnPos, Quaternion.identity);
		}

		private int setBlockCoord(int coord, ref int extraCoord){
			if (coord < 0) {
				extraCoord = -1;
				return 0;
			}
			if (coord >= gridSize) {
				extraCoord = 1;
				return gridSize - 1;
			}
			return coord;
		}

		public SnakeBlock getBlockFromNumber(int coord){
			int x = coord % gridSize;
			int y = coord / gridSize;
			return blocks [x, y];
		}
	}

}