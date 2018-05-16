using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace FileManager{

	public class FileManagerUIInode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler  {

		public Text theText;
		public Image theImage;
		private InodeInformation info;
		private bool isOver = false;

		public void init(InodeInformation info){
			this.info = info;

			theText.text = info.inodeNumber.ToString();
			transform.localScale = Vector3.one;
			scaleText ();
			setColor ();
		}


		public void clicked(){
			//Quick reference hack
			transform.parent.parent.GetComponent<FileManagerInodeSection> ().displayInodeInformation (info);
		}

		private void setColor(){
			if (info.bitmapAllocated)
				theImage.color = Color.red;
			else
				theImage.color = Color.white;
		}

		#region IPointerEnterHandler implementation

		public void OnPointerEnter (PointerEventData eventData){
			if (isOver)
				return;
			isOver = true;

			foreach (int i in info.allocatedBlocks)
				FileManagerGameMaster.getBlock (i).reference ();
		}


		#region IPointerExitHandler implementation

		public void OnPointerExit (PointerEventData eventData){
			if (isOver == false)
				return;
			isOver = false;

			foreach (int i in info.allocatedBlocks)
				FileManagerGameMaster.getBlock (i).deReference ();
		}



		#endregion
		#endregion

		private void scaleText(){
			switch (theText.text.Length) {
			case 1:	theText.fontSize = 25;break;
			case 2:	theText.fontSize = 21;break;
			case 3:	theText.fontSize = 16;break;
			case 4:	theText.fontSize = 13;break;
			}
		}
	}
}