using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientUI{

	public class AutoRefresh : MonoBehaviour {

		public GameSelectionUI gameSelect;
		private float refreshRate = 3;
		private float refreshPoint = 0;
        private bool refreshGames = false;
        

		void Update(){
            if (refreshGames == false)
                return;

			if (Time.time >= refreshPoint) {
				if(gameSelect.isActiveAndEnabled)
					gameSelect.OnRefreshClick ();

				refreshPoint = Time.time + refreshRate;
			}
		}


        public void activateRefresh() { refreshGames = true; }
        public void deActivateRefresh() { refreshGames = false; }
    }

}