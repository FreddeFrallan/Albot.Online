using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Snake{

	public class SnakeSmoothRenderer : MonoBehaviour{

		public SnakeRenderer theRenderer;
		public GameObject headPrefab;
		public Material[] playerColors;

		private List<SnakeHead> players = new List<SnakeHead>();

		private float interpolationCounter;
		private int updateCounter = 0;
		private float refreshRate = 1;
		private bool isInterpolating = false;




		public void handleGameUpdate(BoardUpdate update){
			//Init the smooth renderer
			if (updateCounter == 0)
				initSmoothRenderer (update);
			else if (updateCounter == 1)//We can't interpolate until we have atleast two updates
				startInterpolation ();

			update.blueCoords.ToList().ForEach ((p) => {distributeNewPosition (players [0], p);});
			update.redCoords.ToList().ForEach ((p) => {distributeNewPosition (players [1], p);});

			players.ForEach ((p) => {p.startNewTargetPos ();});
			interpolationCounter = 0;
			updateCounter++;
		}


		private void initSmoothRenderer(BoardUpdate initUpdate){
			Vector3 firstPosBlue = theRenderer.getBlockFromNumber(initUpdate.blueCoords [0]).getPos(); 
			Vector3 firstPosRed = theRenderer.getBlockFromNumber(initUpdate.redCoords [0]).getPos(); 

			players.Add(Instantiate (headPrefab, firstPosBlue, Quaternion.identity).GetComponent<SnakeHead> ());
			players.Add(Instantiate (headPrefab, firstPosRed, Quaternion.identity).GetComponent<SnakeHead> ());
			for (int i = 0; i < players.Count; i++)
				players [i].init (playerColors [i]);
		}

		private void startInterpolation(){
			isInterpolating = true;
			StartCoroutine (interpolateToNext ());
		}
			
		private void distributeNewPosition(SnakeHead player, int coord){
			Vector3 targetPos = theRenderer.getBlockFromNumber (coord).getPos();
			player.addTargetPos (targetPos);
		}


		private IEnumerator interpolateToNext(){
			while (isInterpolating) {
				interpolationCounter += Time.deltaTime / refreshRate;
				interpolationCounter = Mathf.Clamp (interpolationCounter, 0, 1);

				players.ForEach ((p) => {p.interpolateBetweenPos(interpolationCounter);});
				yield return new WaitForEndOfFrame ();
			}
		}

	}


}