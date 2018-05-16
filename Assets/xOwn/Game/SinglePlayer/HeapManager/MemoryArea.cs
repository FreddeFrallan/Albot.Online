using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MemoryManager{

	public class MemoryArea : MonoBehaviour {

		public GameObject prefabMemSegment;
		public Color allocatedColor, overheadColor, freeColor;

		private List<MemorySegment> memSegments = new List<MemorySegment>();
		private int fullAreaSize;
		private int counter = 0;


		public void init(int size){
			clearMemoryArea ();
			MemorySegment firstSeg = insertNewBlock (0, size, 0);
			firstSeg.setState (SegmentState.Free);
			fullAreaSize = size;
		}

		void Update(){
			if (Input.GetKey (KeyCode.Space))
				allocateArea (counter += 10, 5);

			if (Input.GetKeyDown (KeyCode.A)) {
				freeArea (10);
			}

			if (Input.GetKeyDown (KeyCode.S)) {
				mergeSegments (10, 15);
			}

			if (Input.GetKeyDown (KeyCode.P))
				printMemSegments ();
		}

		#region Free
		public void freeArea(int pos){
			MemorySegment foundSeg = memSegments.Find ((x) => x.pos == pos);
			if (foundSeg == null) {
				Debug.LogError ("Free did not match with any block");
				return;
			}

			foundSeg.setState (SegmentState.Free);
		}

		public void mergeSegments(int pos1, int pos2){
			int foundIndex = memSegments.FindIndex ((x) => x.pos == pos1);
			int foundIndex2 = memSegments.FindIndex ((x) => x.pos == pos2);

			if (foundIndex == -1 || foundIndex2 == -1) {
				Debug.LogError ("Did not find matching mergeSegment: " + pos1 + "  " + pos2);
				return;
			}

			if (foundIndex2 - foundIndex != 1) {
				Debug.LogError ("Blocks not alligned: " + pos1 + "  " + pos2);
				return;
			}

			MemorySegment seg1 = memSegments [foundIndex];
			MemorySegment seg2 = memSegments [foundIndex2];
			int newSize = seg2.pos + seg2.size - seg1.pos;
			seg1.setSizeAndPos (seg1.pos, newSize);
			memSegments.Remove (seg2);
			destroyMemSegment(seg2);
		}
		#endregion


		#region Allocate
		public void allocateArea(int pos, int size){

			if (pos >= fullAreaSize) {
				Debug.LogError ("Out of bounds, tried allocate at " + pos + ", fullSize is only: " + fullAreaSize);
				return;
			}

			int foundSegIndex = getMatchingAllocateBlock (pos, size);
			if (foundSegIndex == -1) {
				Debug.LogError ("Failed to allocate at: " + pos + "   with size: " + size);
				Debug.LogError ("Block found: " + foundSegIndex);
				return;
			}

			MemorySegment foundSeg = memSegments[foundSegIndex];
			MemorySegment nextSegment = (foundSegIndex + 1 < memSegments.Count) ? memSegments[foundSegIndex+1] : null;
			handlePreBlock (foundSeg, pos, ref foundSegIndex);
			foundSeg.setSizeAndPos (pos, size);
			handlePostBlock (foundSeg, nextSegment, size, foundSegIndex);


			foundSeg.setState (SegmentState.Allocated);
		}

		private void handlePreBlock(MemorySegment foundSeg, int allocatePos, ref int foundIndex){
			if (foundSeg.pos == allocatePos)
				return;

			int preSegSize = allocatePos - foundSeg.pos;
			MemorySegment preBlock = insertNewBlock (foundSeg.pos, preSegSize, foundIndex);
			preBlock.setState (SegmentState.Free);
			foundIndex++;
		}

		private void handlePostBlock(MemorySegment foundSeg, MemorySegment nextSeg, int size, int foundIndex){
			int nextSegPos = (nextSeg == null) ? fullAreaSize : nextSeg.pos;
			int allocatedEndPos = foundSeg.pos + size;
			int newBlockSize = nextSegPos - allocatedEndPos;

			if (newBlockSize == 0)
				return;
			
			MemorySegment postBlock = insertNewBlock(allocatedEndPos, newBlockSize, foundIndex+1);
			postBlock.setState (SegmentState.Free);
		}
		#endregion


		#region util
		private MemorySegment insertNewBlock(int pos, int size, int index){
			GameObject newObj = Instantiate (prefabMemSegment, Vector3.zero, Quaternion.identity, this.transform);
			MemorySegment newSeg = newObj.GetComponent<MemorySegment> ();
			memSegments.Insert (index, newSeg);

			newSeg.setSizeAndPos (pos, size);
			newSeg.init (allocatedColor, freeColor, overheadColor);
			return newSeg;
		}


		private int getMatchingAllocateBlock(int pos, int size){
			for (int i = 0; i < memSegments.Count; i++) {
				if (memSegments [i].pos <= pos && memSegments [i].pos + memSegments [i].size >= pos + size)
					return i;
			}
			return -1;
		}


		private void clearMemoryArea(){
			for(int i = memSegments.Count-1; i >= 0; i--)
				destroyMemSegment(memSegments[i]);
			memSegments.Clear ();
		}

		private void destroyMemSegment(MemorySegment seg){
			seg.cleanup ();
			Destroy (seg.gameObject);
		}

		public bool canMergeSegments(){
			for(int i = 0; i < memSegments.Count-1; i++){
				MemorySegment seg = memSegments [i];
				MemorySegment next = memSegments [i + 1];

				if (seg.state != SegmentState.Free || next.state != SegmentState.Free)
					continue;

				if (seg.pos + seg.size == next.pos) {
					print ("CAN MERGE " +  seg.pos + "  " + next.pos);
					return true;
				}
			}
			return false;
		}

		private void printMemSegments(){
			print ("*************************");
			foreach (MemorySegment seg in memSegments)
				print (seg.ToString ());
		}


		public List<MemorySegment> getMemSegments(){return memSegments;}
		#endregion
	}
}