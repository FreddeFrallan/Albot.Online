using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryManager{

	public class GameMasterHeapManager : MonoBehaviour {

		public GameLogicHeapManager gameLogic;
		public HeapManagerSimulator simulator;
		public MemoryArea memArea;
		public HeapManagerCamera theCamera;
		public InputField sizeText;


		public Button startSimButton, stopSimButton;

		private int memorySize = 1000;
		private bool playing = false;

		void Start(){
			SinglePlayerGameMaster.init (gameLogic.takeTCPInput, restartGame);
			sizeText.text = "1000";
			restartGame ();
		}
			
			
		private IEnumerator sendBoardToPlayer(){
			yield return new WaitForSeconds (1);
			TCPLocalConnection.sendMessage ("Welcome");
		}
			
		public void restartGame(){
			playing = true;
			memArea.init (memorySize);
			theCamera.init (memorySize);
			simulator.stopSimulation ();
			print ("restarting GAME");
		}

		public void restartButtonPressed(){
			memorySize = int.Parse (sizeText.text);
			restartGame ();
		}

		public void startSimulationPressed(){
			restartGame ();
			simulator.startSimulation ();
			startSimButton.interactable = false;
			stopSimButton.interactable = true;
		}

		public void stopSimulationPressed(){
			simulator.stopSimulation ();
			startSimButton.interactable = true;
			stopSimButton.interactable = false;
		}
	}

}