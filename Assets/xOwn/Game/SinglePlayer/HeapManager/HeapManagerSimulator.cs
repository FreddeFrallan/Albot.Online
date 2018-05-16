using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using System.Collections; 
using System.Reflection; 
using UnityEngineInternal; 
using UnityEngine;

namespace MemoryManager{

	public class HeapManagerSimulator : MonoBehaviour {

		public MemoryArea memArea;
		private List<MemorySegment> memSegments;
		private bool isSimulating = false;
		private bool isSending = false;
		private int simCounter = 0;


		#region AllocationSimulation
		private const float maxValue = 102, slope = 20, yBoost = 79.3f, minValue = 1; 
		private float adjustZeroValue = maxValue / (Mathf.Exp (yBoost / slope) + 1);
		#endregion



		List<string> simList = new List<string>();
		public void sendNextSim(){
			TCPLocalConnection.sendMessage (simList[simCounter++]);
			print (simCounter);
		}

		public void startSimulation(){
			memSegments = memArea.getMemSegments();
			isSimulating = true;
			print ("Start simul");
			receivedInput ();
		}


		public void receivedInput(){
			if (isSimulating && isSending == false && memArea.canMergeSegments() == false) {
				isSending = true;
				StartCoroutine (pickRandomAction ());
			}
		}

		private IEnumerator pickRandomAction(){
			yield return new WaitForSeconds (0.05f);
			isSending = false;
			List<MemorySegment> freeList = memSegments.Where ((x) => x.state == SegmentState.Free).ToList();
			List<MemorySegment> allocList = memSegments.Where ((x) => x.state == SegmentState.Allocated).ToList();

			if (allocList.Count == 0)
				simulateMalloc (freeList);
			else {
				if (Random.Range (0, 100) >= 40)
					simulateMalloc (freeList);
				else
					simulateFree (allocList);
			}
		}

		private void simulateMalloc(List<MemorySegment> freeList){
			int allocationSize = calcAllocSize ();
			TCPLocalConnection.sendMessage ("Malloc " + allocationSize);
			print ("Malloc " + allocationSize);
		}

		private int calcAllocSize(){
			int x = Random.Range (0, 100);
			return Mathf.RoundToInt(-Mathf.Log ((maxValue / (x + adjustZeroValue)) - 1) * slope + yBoost);
		}



		private void simulateFree(List<MemorySegment> allocList){
			int freePos = allocList [Random.Range (0, allocList.Count - 1)].pos;
			TCPLocalConnection.sendMessage ("Free " + freePos);
			print ("Free " + freePos);
		}
			


		public void stopSimulation(){
			isSimulating = false;
			isSending = false;
			print ("Stop simul");
			simCounter = 0;
		}

		private int getBiggestFreeSize(List<MemorySegment> freeList){return freeList.OrderByDescending ((x) => x.size).ToArray () [0].size;}
	}
}