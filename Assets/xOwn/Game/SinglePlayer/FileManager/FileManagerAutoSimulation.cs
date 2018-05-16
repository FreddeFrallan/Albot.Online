using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FileManager{

	public class FileManagerAutoSimulation : MonoBehaviour {

		public FileManagerGameLogic gameLogic;
		public List<Button> startButtons, stopButtons;

		private bool isSimulating = false;
		private float simulationSpeed = 0.1f;
		private float addRemoveRatio = 0.6f;


		public void stopSimulation(){
			setInterractable (startButtons, true);
			setInterractable (stopButtons, false);

			isSimulating = false;
			StopCoroutine (simulationTick ());
		}

		public void startSimulation(){
			setInterractable (startButtons, false);
			setInterractable (stopButtons, true);

			isSimulating = true;
			StartCoroutine (simulationTick ());
		}

		private void setInterractable(List<Button> buttons, bool state){
			foreach (Button b in buttons)
				b.interactable = state;
		}


		private void simulateCommand(){
			if (gameLogic.getFreeBytes () <= 0 || Random.value >= addRemoveRatio)
				simulateRemoveFile ();
			else
				simulateAddNewFile ();
		}


		private void simulateRemoveFile(){
			List<BitmapInfo> bitmaps = gameLogic.getInodeBitmaps ();
			if (bitmaps.Count == 0)
				return;
			gameLogic.removeFile (bitmaps [Random.Range (0, bitmaps.Count-1)].id);
		}

		private void simulateAddNewFile(){
			int availableSpace = gameLogic.getFreeBytes();
			int newFileSize = Random.Range (500, Mathf.Min (availableSpace, 40000));
			gameLogic.allocateFile(newFileSize);
		}

		private IEnumerator simulationTick(){
			while (isSimulating) {
				simulateCommand ();
				yield return new WaitForSeconds (simulationSpeed);
			}
		}
	}


}