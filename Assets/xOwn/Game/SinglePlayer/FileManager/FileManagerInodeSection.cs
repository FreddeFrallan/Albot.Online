using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace FileManager{

	public class FileManagerInodeSection : MonoBehaviour {

		public FileManagerGameLogic gameLogic;
		public List<FileManagerUIInode> inodes;
		public Button deleteButton;
		public Text infoText;

		private InodeInformation selectedInode;

		public void init(List<InodeInformation> newInodes){
			List<BitmapInfo> inodeBitmaps = gameLogic.getInodeBitmaps ();

			foreach (InodeInformation info in newInodes)
				info.bitmapAllocated = inodeBitmaps.Find ((x) => x.id == info.inodeNumber) != null;

			for (int i = 0; i < inodes.Count; i++)
				inodes [i].init (newInodes [i]);
			
			deleteButton.interactable = false;
			infoText.text = "";
			selectedInode = null;
		}


		public void displayInodeInformation(InodeInformation info){
			deleteButton.interactable = info.bitmapAllocated;
			infoText.text = info.ToString ();
			selectedInode = info;
		}


		public void deleteFileClicked(){
			if (selectedInode == null)
				return;

			gameLogic.removeFile (selectedInode.inodeNumber);
			deleteButton.interactable = false;
		}

	}

	public class InodeInformation{
		public bool bitmapAllocated;
		public string cTime;
		public int inodeNumber, size, time, mTime, dTime, groupId, linkCount, blocks;
		public List<int> allocatedBlocks = new List<int>();

		public InodeInformation(int inodeNumber){
			bitmapAllocated = false;
			this.inodeNumber = inodeNumber;
			size = 0;
			time = 0;
			cTime = "";
			mTime = 0;
			dTime = 0;
			groupId = 0;
			linkCount = 0;
			blocks = 0;
		}

		public override string ToString (){
			string s = "";

			s += "<b>Inode Number:</b> <i>" + inodeNumber + "</i>\n\n";
			s += "<b>File Size:</b> <i>" + size + "</i>\n\n";
			s += "<b>Time Created:</b> <i>\n\n" + cTime + "</i>";

			return s;
		}
	}
}