using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientUI{
	public class AutoRefresh : MonoBehaviour {
		public GameSelectionUI gameSelect;
		private float refreshRate = 3;
		private float refreshPoint = 0;

		void Update(){
			if (Time.time >= refreshPoint) {
				if(gameSelect.isActiveAndEnabled)
					gameSelect.OnRefreshClick ();

				refreshPoint = Time.time + refreshRate;
			}
		}
	}

}