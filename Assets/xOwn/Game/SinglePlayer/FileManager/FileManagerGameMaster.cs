using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace FileManager{

	public class FileManagerGameMaster : MonoBehaviour {

		private static FileManagerGameMaster singleton;
		public FileManagerGameLogic gameLogic;
		public FileManagerBlockSelector selector;
		public GameObject Block8Prefab;
		public List<FileManager8Block> sections = new List<FileManager8Block>();


		private int rows = 4;
		private float block8Width = 24, rowSpacing = 5;


		void Start(){
			singleton = this;
			initStartBlocks (160);
			gameLogic.init (160);
			selector.setStartSelect (sections [0].getBlock(0));
		}


		private void initStartBlocks(int amountOfBlocks){
			int amountCounter = amountOfBlocks;
			int tickCounter = 0;

			while(amountCounter > 0){
				GameObject temp = Instantiate (Block8Prefab, calcSpawnPos (tickCounter), Quaternion.identity);
				FileManager8Block new8 = temp.GetComponent<FileManager8Block> ();

				sections.Add (new8);
				new8.init (tickCounter * 8, Mathf.Min(amountCounter, 8));

				amountCounter -= 8;
				tickCounter++;
			}
		}


		private Vector3 calcSpawnPos(int i){
			int x = i % rows;
			int y = i / rows;

			return new Vector3 (block8Width * x, -rowSpacing * y, 0);
		}
			

		public static FileManagerBlock getBlock(int index){
			int section = index / 8;
			int sectionIndex = index % 8;

			return singleton.sections [section].getBlock (sectionIndex);
		}

		public static Color getStateColor(BlockState state){
			switch (state) {
			case BlockState.Empty:return Color.white; 
			case BlockState.SuperBlock:return Color.magenta; 
			case BlockState.Inode:return Color.red; 
			case BlockState.BitmapInode:return Color.blue; 
			case BlockState.BitmapData:return Color.green; 
			case BlockState.AllocateData:return Color.yellow;
			}
			return Color.white; 
		}
			
	}
}