using UnityEngine;
using UnityEngine.UI;
using Barebones.Networking;
using System;
using System.Collections;
using Barebones.MasterServer;

namespace ClientUI{

    public class ClientLogin : MonoBehaviour{
        public Button LoginButton;
        public Toggle Remember;
        public InputField Username;
		public InputField Password;
		public LoadingScreenUI loadingScreen;
		private bool waitingForLoginResponse = false;
		private Color startInputFieldSelectionColor, invisInputFieldSelectionColor;
		public static event Action LoggedIn;

		protected readonly string USERNAMEPREFKEY = "AlboRememberUsername";
		protected readonly string PASSWORDEPREFKEY = "AlboRememberPasswor";
		protected readonly string REMEMBERTOGGLEEPREFKEY = "AlboRememberToggle";


        private void Start(){
			if (ClientUI.ClientUIOverlord.hasLoaded)return;
			Msf.Connection.StatusChanged += connectionStatusChanged;
			restoreRememberedValues();
			initInputFields ();
        }


		#region Login auth
		//Run on button click. If success the event OnLoggedInWill be triggered
        public void OnLoginClick(){
			loadingScreen.startLoadingScreen ("Login in", "Please wait...", 2, 5, () => {
					handleLoginCallback (false, "Abort");
				},
				() => handleLoginCallback (false, "Login took to long..."));

			setInteracteable (false);
			HandleRemembering();
			waitingForLoginResponse = true;

			Msf.Client.Auth.LoginAsAlbotUser ((accInfo, error) => {
				if (accInfo == null)
					Msf.Events.Fire (Msf.EventNames.ShowDialogBox, DialogBoxData.CreateError (error));
				handleLoginCallback (accInfo != null, error);
			}, 
			Username.text.Trim(), Password.text);
        }

		//Retreive information the server if we could login or not
		private void handleLoginCallback(bool couldLogin, string msg){
			loadingScreen.closeLoadingScreen ();
			if (waitingForLoginResponse == false)
				return;

			if (couldLogin == false) {
				resetToNormalState ();
				return;
			}

			//gameObject.SetActive(false);
			waitingForLoginResponse = false;
			if (LoggedIn != null)
				LoggedIn.Invoke ();
		}
		#endregion
			

		#region visual and states
		public void setInteracteable(bool active){
			Username.interactable = active;
			if (active) {
				Username.selectionColor = invisInputFieldSelectionColor;
				Username.Select ();
				StartCoroutine (moveCursorToEnd ());
			}
				
			Password.interactable = active;
			LoginButton.interactable = active;
			Remember.interactable = active;
		}

		private void initInputFields(){
			startInputFieldSelectionColor = Username.selectionColor;
			invisInputFieldSelectionColor = startInputFieldSelectionColor;
			invisInputFieldSelectionColor.a = 0;
		}

		private IEnumerator moveCursorToEnd(){
			yield return new WaitForEndOfFrame();
			Username.MoveTextEnd (false);
			Username.selectionColor = startInputFieldSelectionColor;
		}

		private void resetToNormalState(){
			loadingScreen.closeLoadingScreen ();
			setInteracteable (true);
		}
			
		public void connectionStatusChanged(ConnectionStatus status){
			if (status == ConnectionStatus.Connected) {
				setInteracteable (true);
			} else {
				waitingForLoginResponse = false;
				setInteracteable (false);
			}
		}
		#endregion


		#region OldMemories
		private void restoreRememberedValues(){
			Username.text = extractOldPlayerPrefsValue (USERNAMEPREFKEY);
			Password.text = extractOldPlayerPrefsValue (PASSWORDEPREFKEY);
			Remember.isOn = extractOldPlayerPrefsValue (REMEMBERTOGGLEEPREFKEY) != "";
		}

		private string extractOldPlayerPrefsValue(string key, string defaultValue = ""){
			if (PlayerPrefs.HasKey (key))
				return PlayerPrefs.GetString (key);
			return defaultValue;
		}
			
		private void HandleRemembering(){
			if (!Remember.isOn){ // Remember functionality is off. Delete all stored values
				PlayerPrefs.DeleteKey(USERNAMEPREFKEY);
				PlayerPrefs.DeleteKey(PASSWORDEPREFKEY);
				PlayerPrefs.DeleteKey(REMEMBERTOGGLEEPREFKEY);
				return;
			}
			// Remember is on
			PlayerPrefs.SetString(USERNAMEPREFKEY, Username.text);
			PlayerPrefs.SetString(PASSWORDEPREFKEY, Password.text);
			PlayerPrefs.SetString(REMEMBERTOGGLEEPREFKEY, "True");
		}
		#endregion
    }
}