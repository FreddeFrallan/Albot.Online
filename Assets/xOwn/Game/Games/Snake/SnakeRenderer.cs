using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snake{

	public class SnakeRenderer : MonoBehaviour {

		public SnakeSmoothRenderer smoothRenderer;
		public GameObject bgBrick, collisionBlock;
		public Material redHead, redBody, blueHead, blueBody;

		private int gridSize = 20;
		private float blockSize = 1.2f;
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
			for (int i = 0; i < updateMsg.blueCoords.Length - 1; i++)
				getBlockFromNumber (updateMsg.blueCoords [i]).setColor (blueBody);
			for (int i = 0; i < updateMsg.redCoords.Length - 1; i++)
				getBlockFromNumber (updateMsg.redCoords [i]).setColor (redBody);

			getBlockFromNumber (updateMsg.blueCoords [updateMsg.blueCoords.Length-1]).setColor (blueHead);
			getBlockFromNumber (updateMsg.redCoords [updateMsg.redCoords.Length-1]).setColor (redHead);

			smoothRenderer.handleGameUpdate (updateMsg);
		}




		#region Old instant vizualisation
		public void visualizeMove(int playerIndex, Vector2 newPos, Vector2 oldPos){
			if (isInit == false)
				return;

			Material body = playerIndex == 0 ? redBody : blueBody;
			Material head = playerIndex == 0 ? redHead : blueHead;

			blocks [(int)oldPos.x, (int)oldPos.y].setColor (body);
			blocks [(int)newPos.x, (int)newPos.y].setColor (head);
		}

		public void visualizeBody(int playerIndex, Vector2 pos){
			if (isInit == false)
				return;

			Material body = playerIndex == 0 ? redBody : blueBody;
			blocks [(int)pos.x, (int)pos.y].setColor (body);
		}

		public void visualizeHead(int playerIndex, Vector2 pos){
			if (isInit == false)
				return;

			Material head = playerIndex == 0 ? redHead : blueHead;
			blocks [(int)pos.x, (int)pos.y].setColor (head);
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
		#endregion
	}

}