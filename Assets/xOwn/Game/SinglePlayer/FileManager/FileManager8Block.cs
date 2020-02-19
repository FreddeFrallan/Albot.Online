using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FileManager{

	public class FileManager8Block : MonoBehaviour {

		public GameObject blockPrefab;
		private FileManagerBlock[] blocks;
		private float blockSpace = 0.138f;
		private float postScale = 20;

		public void init(int startIndex, int amount){
			blocks = new FileManagerBlock[amount];
	
			for (int i = 0; i < amount; i++) {
				Vector3 spawnPos = transform.position + new Vector3 (blockSpace * i, 0, 0);
				GameObject temp = Instantiate (blockPrefab, spawnPos, Quaternion.identity, transform);

				blocks [i] = temp.GetComponent<FileManagerBlock> ();
				blocks [i].init (startIndex + i);
			}
		
			transform.localScale = new Vector3 (postScale, postScale, 0);
		}


		public FileManagerBlock getBlock(int index){
			return blocks [index];
		}
	}

}