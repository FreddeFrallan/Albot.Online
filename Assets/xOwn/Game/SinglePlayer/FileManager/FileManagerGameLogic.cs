using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FileManager{

	public class FileManagerGameLogic : MonoBehaviour {
		public const int blockSize = 4096;
		const int inodesInBlock = 16;

		public FileManagerUIOverlord UIOverlord;
		public Text saveSize;
		public FileManagerGameMaster master;
		private int inodeCounter = 0;

		private List<FileManagerBlock> inodes = new List<FileManagerBlock>();
		private List<int> bitmapInode = new List<int> ();
		private List<int> bitmapData = new List<int> ();
		private int systemSize, dataStartIndex, inodeStartIndex;
		private int totalBlockAmount;

		#region startSection
		public void init(int blockAmount){
			this.totalBlockAmount = blockAmount;
			FileManagerGameMaster.getBlock (0).setState (BlockState.SuperBlock);
			FileManagerGameMaster.getBlock (1).setState (BlockState.BitmapInode);
			FileManagerGameMaster.getBlock (2).setState (BlockState.BitmapData);
			allocateInodeBlocks (3, 5);
			this.systemSize = blockAmount * (blockSize - 8);
			inodeStartIndex = 3;
			dataStartIndex = 8;
		}
			
		private void allocateInodeBlocks(int startIndex, int amount){
			for (int i = 0; i < amount; i++) {
				FileManagerGameMaster.getBlock (startIndex + i).setAsInodeBlock (inodeCounter);
				inodes.Add (FileManagerGameMaster.getBlock (startIndex + i));
				inodeCounter += 16;
			}
		}
		#endregion

		public void allocateFilePressed(){
			int startSize = 0;
			if (int.TryParse (saveSize.text, out startSize))
				allocateFile (startSize);
			else
				allocateFile (0);
		}


		public void allocateFile(int startSize){
			int inode = findFreeInodeIndex ();
			if (inode == -1) {
				noFreeInodeError ();
				return;
			}
		
			int amountDataBlocksNeeded = Mathf.CeilToInt ((float)startSize / blockSize);
			List<int> foundBlocks = getFreeDataBlocks (amountDataBlocksNeeded);
			if (foundBlocks.Count < amountDataBlocksNeeded) {
				notEnoughSpaceError ();
				return;
			}

			int sizeCounter = startSize;
			foreach (int b in foundBlocks) {
				FileManagerGameMaster.getBlock (b).setAsAllocated (Mathf.Min(sizeCounter, blockSize));
				sizeCounter -= blockSize;
			}

			bitmapInode.Add (inode);
			bitmapData.AddRange (foundBlocks);
			setInodeInformation (inode, foundBlocks, startSize);

			UIOverlord.dataChanged ();
		}


		public void removeFile(int inodeNumber){
			bitmapInode.Remove (inodeNumber);
			FileManagerBlock inodeBlock = getMatchingInodeBlock (inodeNumber);
			inodeBlock.freeInode (inodeNumber);

			foreach (int block in inodeBlock.getInodeInformation (inodeNumber).allocatedBlocks) {
				FileManagerGameMaster.getBlock (block).setState (BlockState.Empty);
				bitmapData.Remove (block);
			}

			InodeInformation newInfo = new InodeInformation (inodeNumber);
			getMatchingInodeBlock (inodeNumber).setInodeInformation (inodeNumber, newInfo);
			UIOverlord.dataChanged ();
		}


		private void noFreeInodeError(){
		//	Debug.LogError ("You have no free Inodes");
		}

		private void notEnoughSpaceError(){
		//	Debug.LogError ("You have not enough space on disk");
		}

		private int findFreeInodeIndex(){
			for (int i = 0; i < inodeCounter; i++) {
				if (bitmapInode.Contains (i) == false)
					return i;	
			}
			return -1;
		}

		private void setInodeInformation(int inode, List<int> allocatedBlocks, int size){
			InodeInformation newInfo = new InodeInformation (inode){ allocatedBlocks = allocatedBlocks, size = size, cTime = System.DateTime.Now.ToString ()};
			getMatchingInodeBlock (inode).setInodeInformation (inode, newInfo);
		}



		private FileManagerBlock getMatchingInodeBlock(int inodeNumber){
			int blockOffset = inodeNumber / inodesInBlock;
			return FileManagerGameMaster.getBlock (inodeStartIndex + blockOffset);
		}


		private List<int> getFreeDataBlocks(int amount){
			List<int> foundBlocks = new List<int> ();

			int i = dataStartIndex;
			while (amount > 0 && i < totalBlockAmount) {
				if (bitmapData.Contains (i) == false) {
					foundBlocks.Add (i);
					amount--;
				}
					
				i++;
			}

			return foundBlocks;
		}


		public List<BitmapInfo> getDataBitmaps(){
			List<BitmapInfo> temp = new List<BitmapInfo> ();
			foreach (int i in bitmapData)
				temp.Add (new BitmapInfo (){ id = i, targetBlockID = i, type = BitmapType.Data});
			return temp;
		}
	
		public List<BitmapInfo> getInodeBitmaps(){
			List<BitmapInfo> temp = new List<BitmapInfo> ();
			foreach (int i in bitmapInode) {
				int blockOffset = Mathf.CeilToInt ((float)i / inodesInBlock);
				temp.Add (new BitmapInfo (){ id = i, targetBlockID = inodeStartIndex + blockOffset, type = BitmapType.Inode});
			}
			return temp;
		}

		public int getFreeBytes(){return systemSize - (bitmapData.Count * blockSize);}

	}


}