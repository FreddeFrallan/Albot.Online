using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Barebones.MasterServer;
using TMPro;

namespace ClientUI{
	public class ClientUserPanelUI : MonoBehaviour {

		[SerializeField]
		private Image icon;
        [SerializeField]
        private TextMeshProUGUI username;
        [SerializeField]
        private TextMeshProUGUI timerText = null, scoreText = null;
		[SerializeField]
		private Color timerActive = Color.black, timerPaused = Color.black;
		private TurnTimer turnTimer;
		private bool timerHasInit = false;

		public void init(){ClientLogin.LoggedIn += onLoggedIn;}
		private void onLoggedIn(){setUserPanel (int.Parse(Msf.Client.Auth.AccountInfo.Properties[AlbotDictKeys.icon]), Msf.Client.Auth.AccountInfo.Username);}
		public void setUserPanel(int iconNumber, string username){
			this.icon.sprite = ClientIconManager.loadIcon(iconNumber);
			this.icon.enabled = true;
            this.username.SetText(username);
		}
		public void clearPanel(){
            this.username.SetText("");
			this.icon.enabled = false;
			if (turnTimer != null) {
				turnTimer.stopTimer ();
				timerText.enabled = false;
			}
		}



		public void initTurnTime(float maxTimePerTurn){
			turnTimer = new TurnTimer (maxTimePerTurn, timerActive, timerPaused, timerText);
			timerHasInit = true;
		}
		public void startTimer(float maxTime){turnTimer.startTimer (maxTime);}
		public void stopTimer(){turnTimer.stopTimer ();}
		void Update(){
			if (timerHasInit)
				turnTimer.updateTimer ();
		}

		public void setScore(int score){
			scoreText.text = score.ToString ();
		}


		private class TurnTimer{
			private float currentTime;
			private bool active = false;
			private TextMeshProUGUI timerText;
			private Color timerActive, timerPaused;
			public TurnTimer(float maxTime, Color timerActive, Color timerPaused, TextMeshProUGUI timerText){
				this.timerActive = timerActive; this.timerPaused = timerPaused;
				this.timerText = timerText;
				timerText.text = Mathf.Round(maxTime).ToString ();
				timerText.color = timerPaused;
				timerText.enabled = true;
			}

			public void updateTimer(){
				if (active == false)
					return;
				currentTime -= Time.deltaTime;
				timerText.text = Mathf.Round(currentTime).ToString ();
			}

			public void startTimer(float maxTime){
				currentTime = maxTime;
				active = true;
				timerText.color = timerActive;
			}

			public void stopTimer(){
				active = false;
				timerText.color = timerPaused;
			}

		}
	}

}