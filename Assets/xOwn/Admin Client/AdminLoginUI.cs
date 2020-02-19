using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Barebones.Networking;
using Barebones.MasterServer;
using TMPro;


namespace AdminUI{
	
	public class AdminLoginUI : MonoBehaviour {

		public TMP_InputField pwField;
		public Button loginButton;
		public Toggle Remember;
		private Color startInputFieldSelectionColor, invisInputFieldSelectionColor;

		void Start(){
			Msf.Connection.StatusChanged += ((status) => {setInteracteable(status == ConnectionStatus.Connected);});
		}



		#region Visual UI
		public void setInteracteable(bool active){
			pwField.interactable = active;
			if (active) {
				pwField.selectionColor = invisInputFieldSelectionColor;
				pwField.Select ();
				StartCoroutine (moveCursorToEnd ());
			}
				
			loginButton.interactable = active;
			Remember.interactable = active;
		}

		private void initColors(){
			startInputFieldSelectionColor = pwField.selectionColor;
			invisInputFieldSelectionColor = startInputFieldSelectionColor;
			invisInputFieldSelectionColor.a = 0;
		}

		private IEnumerator moveCursorToEnd(){
			yield return new WaitForEndOfFrame();
			pwField.MoveTextEnd (false);
			pwField.selectionColor = startInputFieldSelectionColor;
		}
		#endregion



		public void loginButtonPressed(){
			Msf.Connection.SendMessage ((short)CustomMasterServerMSG.adminLogin, pwField.text, ((r, m) => {
				if(r != ResponseStatus.Success){
					Msf.Events.Fire(Msf.EventNames.ShowDialogBox, DialogBoxData.CreateError(m.AsString()));
					Debug.LogError(m.ToString());
				}
				else
					handleValidLogin();
			}));

            pwField.text = "";
		}

		private void handleValidLogin(){
			AdminUIManager.requestLogin ();
		}
	}
}