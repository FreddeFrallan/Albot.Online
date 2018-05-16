using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FileManager{

	public class FileManagerUIObj : MonoBehaviour {

		public BitmapInfo info;
		public Text theText;
		public Image theImage;

		public void init(BitmapInfo info, Color newColor, string logoText = ""){
			this.info = info;
			theImage.color = newColor;

			if (logoText == "")
				theText.text = info.id.ToString ();
			else
				theText.text = logoText;
			transform.localScale = Vector3.one;
			scaleText ();
		}



		private void scaleText(){
			switch (theText.text.Length) {
			case 1:	theText.fontSize = 25;break;
			case 2:	theText.fontSize = 22;break;
			case 3:	theText.fontSize = 18;break;
			case 4:	theText.fontSize = 13;break;
			}
		}
	}

}