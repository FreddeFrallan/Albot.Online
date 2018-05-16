using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MemoryManager;

namespace FileManager{

	public class FileManagerBlockSelector : MonoBehaviour {

		public Camera theCamera;
		public FileManagerUIOverlord UIOverlord;

		public List<BlockInfoUI> infoUI;
		private Collider lastCollider;
		private FileManagerBlock lastHoverBlock, lastSelectBlock;

		private bool hasLastHoverSeg = false, hasLastSelectSeg = false;

		// Update is called once per frame
		void Update () {
			setCurrentHoverSeg ();
			if (Input.GetMouseButtonDown (0))
				click ();
		}


		private void click(){
			if (hasLastHoverSeg == false)
				return;
			
			if (hasLastSelectSeg)
				lastSelectBlock.deSelect ();
			
			hasLastSelectSeg = hasLastHoverSeg;
			lastSelectBlock = lastHoverBlock;
			lastSelectBlock.select ();
			UIOverlord.blockInfoPressed ();
		}

		#region hover & selection
		private void setCurrentHoverSeg(){
			RaycastHit hit;
			Ray theRay = theCamera.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (theRay, out hit)) {
				if (hasLastHoverSeg == false) {
					hoverNewSegment (hit);
					return;
				}
				if (lastCollider == hit.collider)
					return;

				lastHoverBlock.deHover ();
				hoverNewSegment(hit);

			} else if(hasLastHoverSeg){
				hasLastHoverSeg = false;
				lastHoverBlock.deHover ();
			}
		}

		private void hoverNewSegment(RaycastHit hit){
			lastCollider = hit.collider;
			lastHoverBlock = hit.collider.GetComponent<FileManagerBlock> ();
			lastHoverBlock.hover ();
			foreach(BlockInfoUI infoBlock in infoUI)
				infoBlock.displayNewFileBlock (lastHoverBlock);
			hasLastHoverSeg = true;
		}
		#endregion


		public FileManagerBlock getSelectedBlock(){return  lastSelectBlock;}
		public void setStartSelect(FileManagerBlock firstSelect){lastSelectBlock = firstSelect;}

	}
}