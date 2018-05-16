using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FileManager{

	public class FileManagerDataSection : MonoBehaviour {

		public RectTransform allocated, fullMemory;
		private float allocatedPercentage = 0.7f;


		public void init(int allocatedBytes){
			print ("Allocated: " + allocatedBytes);
			allocatedPercentage =  (float)allocatedBytes / FileManagerGameLogic.blockSize;
			print ("Percentage: " + allocatedPercentage);
		}

		// Update is called once per frame
		void Update () {
			Vector2 temp = allocated.offsetMax;
			temp.x = -fullMemory.rect.width * (1 -allocatedPercentage);
			allocated.offsetMax = temp;
		}
	}
}