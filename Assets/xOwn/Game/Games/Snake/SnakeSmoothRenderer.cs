using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Game;

namespace Snake{

	public class SnakeSmoothRenderer : MonoBehaviour{

		public SnakeRenderer theRenderer;
		public GameObject headPrefab;
		public Material[] playerColors;

		private List<Position2D>[] addedPlayerPos = new List<Position2D>[2];
		private List<SnakeHead> players = new List<SnakeHead>();

		private float interpolationCounter;
		private int updateCounter = 0;
		private bool isInterpolating = false;


		//The protocol should later be re-written so we can loop through all the players
		public void handleGameUpdate(BoardUpdate update){
			if (updateCounter == 0)
				initSmoothRenderer (update);
			else if (updateCounter == 1)//We can't interpolate until we have atleast two updates
				startInterpolation ();

			foreach (Position2D i in update.blueCoords.Distinct().Where((c) => !addedPlayerPos[0].Contains(c)))
				distributeNewPosition (0, i);
			foreach (Position2D i in update.redCoords.Distinct().Where((c) => !addedPlayerPos[1].Contains(c)))
				distributeNewPosition (1, i);

			addedPlayerPos [0].AddRange (update.blueCoords);
			addedPlayerPos [1].AddRange (update.redCoords);

			players.ForEach ((p) => {p.startNewTargetPos ();});
			interpolationCounter = 0;
			updateCounter++;
		}


		private void initSmoothRenderer(BoardUpdate initUpdate){
			addedPlayerPos [0] = new List<Position2D> ();
			addedPlayerPos [1] = new List<Position2D> ();

			Vector3 firstPosBlue = theRenderer.getBlockFromPos(initUpdate.blueCoords [0]).getPos(); 
			Vector3 firstPosRed = theRenderer.getBlockFromPos(initUpdate.redCoords [0]).getPos(); 

			players.Add(Instantiate (headPrefab, firstPosBlue, Quaternion.identity).GetComponent<SnakeHead> ());
			players.Add(Instantiate (headPrefab, firstPosRed, Quaternion.identity).GetComponent<SnakeHead> ());
			for (int i = 0; i < players.Count; i++) 
				players [i].init (playerColors [i]);
		}

		private void startInterpolation(){
			isInterpolating = true;
			StartCoroutine (interpolateToNext ());
		}
			
		private void distributeNewPosition(int playerIndex, Position2D coord){
			Vector3 targetPos = theRenderer.getBlockFromPos(coord).getPos();
			players[playerIndex].addTargetPos (targetPos);
			addedPlayerPos [playerIndex].Add (coord);
		}


		private IEnumerator interpolateToNext(){
			while (isInterpolating) {
				interpolationCounter += Time.deltaTime / SnakeGameLogic.refreshRate;
				interpolationCounter = Mathf.Clamp (interpolationCounter, 0, 1);

				players.ForEach ((p) => {p.interpolateBetweenPos(interpolationCounter);});
				yield return new WaitForEndOfFrame ();
			}
		}

	}


}