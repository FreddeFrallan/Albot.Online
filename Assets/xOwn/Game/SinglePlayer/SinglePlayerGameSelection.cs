using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Barebones.MasterServer;
using ClientUI;
using Barebones.Networking;
using Barebones.Utils;
using System;

namespace ClientUI{

	public class SinglePlayerGameSelection : MonoBehaviour {

		public SinglePlayerLobby lobby;
		public Dropdown exerciseDropDown;
		public List<ExerciseMapSelection> maps;


		public void newGameSelected(){
			if (exerciseDropDown.value == 0)
				return;

			ExerciseMapSelection m = getSelectedMap (exerciseDropDown.value);
			resetSelection ();
            ClientUIStateManager.requestGotoState(ClientUIStates.PreGame);
			lobby.initGameLobby (m.Name, m.picture, m.type, m.Scene);
		}

		private void resetSelection(){
			exerciseDropDown.value = 0;
			exerciseDropDown.Hide ();

			//QUICK FIX DELUXE
			Destroy (GameObject.Find ("Dropdown List"));
		}


		private ExerciseMapSelection getSelectedMap(int selectedValue){return maps [selectedValue -1];}

		[Serializable]
		public class ExerciseMapSelection{
			public int maxPlayers;
			public string Name;
			public SceneField Scene;
			public Sprite picture;
			public Game.GameType type;
			public bool isRealTime = false;
		}
			
	}

}