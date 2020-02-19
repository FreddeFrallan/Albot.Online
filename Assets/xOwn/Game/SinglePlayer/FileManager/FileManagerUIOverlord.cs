using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FileManager{

	public class FileManagerUIOverlord : MonoBehaviour {
	
		public FileManagerGameLogic gameLogic;
		public FileManagerBlockSelector selector;
		public GameObject simulationObj, dataInfo, inodeInfo, bitmapInfo, superBlockinfo;
		public FileManagerInodeSection inodePanel;
		public FileManagerDataSection dataPanel;
		public FileManagerBitmapSection bitmapPanel;

		private GameObject currentPanel;
		private UIState state;


		void Start(){
			currentPanel = simulationObj;
		}

		public void blockInfoPressed(){
			currentPanel.SetActive (false);
			state = UIState.BlockInfo;
			activateBlockInfo ();
		}

		public void simulationPressed(){
			currentPanel.SetActive (false);
			simulationObj.SetActive (true);
			state = UIState.Simulation;
			currentPanel = simulationObj;
		}

		public void dataChanged(){
			if (state == UIState.BlockInfo)
				blockInfoPressed ();
		}


		private void activateBlockInfo(){
			FileManagerBlock block = selector.getSelectedBlock ();

			switch (block.state) {
			case BlockState.AllocateData:
				currentPanel = dataInfo;
				dataPanel.init (block.allocatedBytes);
				break;
			case BlockState.BitmapData:
				currentPanel = bitmapInfo;
				bitmapPanel.init ("Mapped Data blocks", gameLogic.getDataBitmaps());
				break;
			case BlockState.BitmapInode:
				currentPanel = bitmapInfo;
				bitmapPanel.init ("Mapped Inodes", gameLogic.getInodeBitmaps());
				break;
			case BlockState.Empty:
				currentPanel = dataInfo;
				dataPanel.init (block.allocatedBytes);
				break;
			case BlockState.Inode:
				currentPanel = inodeInfo;
				inodePanel.init (block.inodes);
				break;
			case BlockState.SuperBlock: 
				currentPanel = superBlockinfo;
				break;
			}

			currentPanel.SetActive (true);
		}
			



		private enum UIState{
			Simulation,
			BlockInfo,
			History
		}
	}
}