using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MemoryManager{

	public class MemorySegmentClicker : MonoBehaviour {

		public Camera theCamera;
		public BlockInfoUI infoUI;
		private Collider lastCollider;
		private MemorySegment lastHoverSeg, lastSelectSeg;

		private bool hasLastHoverSeg = false, hasLastSelectSeg = false;

		// Update is called once per frame
		void Update () {
			setCurrentHoverSeg ();
			if (Input.GetMouseButtonDown (0))
				click ();
		}


		private void click(){
			if (hasLastSelectSeg)
				lastSelectSeg.deSelect ();

			hasLastSelectSeg = hasLastHoverSeg;
			if (hasLastHoverSeg == false)
				return;

			lastSelectSeg = lastHoverSeg;
			lastSelectSeg.select ();
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

				lastHoverSeg.deHover ();
				hoverNewSegment(hit);

			} else if(hasLastHoverSeg){
				hasLastHoverSeg = false;
				lastHoverSeg.deHover ();
			}
		}

		private void hoverNewSegment(RaycastHit hit){
			lastCollider = hit.collider;
			lastHoverSeg = hit.collider.GetComponent<MemorySegment> ();
			lastHoverSeg.hover ();
			infoUI.displayNewBlockInfo (lastHoverSeg);
			hasLastHoverSeg = true;
		}
		#endregion


		public MemorySegment getSelectedSegment(){return  lastSelectSeg;}
	}
}