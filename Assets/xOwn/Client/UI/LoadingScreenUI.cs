﻿using System;
using System.Collections;
using System.Collections.Generic;
using Barebones.Networking;
using UnityEngine;
using UnityEngine.UI;

namespace Barebones.MasterServer
{
    /// <summary>
    ///     Displays progress of game creation
    /// </summary>
    public class LoadingScreenUI : MonoBehaviour
    {
        public Button AbortButton;

        protected SpawnRequestController Request;
        public Image RotatingImage;

		public Text title;
        public Text StatusText;
		private Action abortCall;
		private Action errorCall;
		private bool runningLoadingScreen = false;

        
        private void Update()
        {
            RotatingImage.transform.Rotate(Vector3.forward, Time.deltaTime*360*2);
        }

        public void OnAbortClick()
        {
			abortCall.Invoke ();
			this.gameObject.SetActive (false);
			runningLoadingScreen = false;
        }

		public void startLoadingScreen(string title, string infoText, float abortDelay, float errorTime, Action abortCall,  Action errorCall){
			this.title.text = title;
			StatusText.text = infoText;
			this.gameObject.SetActive (true);
			runningLoadingScreen = true;
		}

		public void closeLoadingScreen(){
			if (runningLoadingScreen) {
				StopCoroutine (runLoadingScreen(0));
				this.gameObject.SetActive (false);
			}
		}

		private IEnumerator runLoadingScreen(float errorTime){
			yield return new WaitForSeconds (errorTime);
			if (runningLoadingScreen) {
				errorCall.Invoke ();
				runningLoadingScreen = false;
			}
		}
    }
}