using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MemoryManager{

	public class MemorySegment : MonoBehaviour {

		public int pos = 0, size = 0;
		public SpriteRenderer graphics;
		public SegmentState state;
		public GameObject selectionMask, hoverMask, endLinePrefab;

		private Color allocatedColor, freeColor, overheadColor;
		private GameObject currentEndLine;

		public void init(Color allocatedColor,Color freeColor, Color overheadColor){
			this.allocatedColor = allocatedColor;
			this.freeColor = freeColor;
			this.overheadColor = overheadColor;
		}


		public void setSizeAndPos(int pos, int size){
			this.pos = pos;
			this.size = size;
			transform.position = new Vector3 (pos, 0, 0);
			transform.localScale = new Vector3 (size, 1, 1);
			setCurrentEndLinde ();
		}

		private void setCurrentEndLinde(){
			destoyOldLine ();
			currentEndLine = Instantiate (endLinePrefab, transform.position + new Vector3 (size, 0, -1), Quaternion.identity, transform.parent);
		}
			
		public void setState(SegmentState newState){
			state = newState;
			Color newColor;
			if (state == SegmentState.Allocated) newColor = allocatedColor;
			else if (state == SegmentState.Overhead) newColor = overheadColor;
			else newColor = freeColor;

			graphics.color = newColor;
		}

		private void destoyOldLine(){if (currentEndLine != null)Destroy (currentEndLine);}
		public void cleanup(){destoyOldLine ();}
		public void hover(){hoverMask.SetActive (true);}
		public void deHover(){hoverMask.SetActive (false);}
		public void select(){selectionMask.SetActive (true);}
		public void deSelect(){selectionMask.SetActive (false);}
		public override string ToString (){return string.Format ("{2}  Pos: {0},  Size: {1}", pos, size, state);}
	}


	public enum SegmentState{
		Free,
		Allocated,
		Overhead
	}
}