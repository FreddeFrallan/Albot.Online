using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FileManager{

	public class FileManagerBlock : MonoBehaviour {

		public SpriteRenderer theRenderer;
		public BlockState state;
		public GameObject hoverMask, selectMask, referenceMask;
		public int id, allocatedBytes = 0;
		public List<InodeInformation> inodes = new List<InodeInformation>();
		public List<BitmapInfo> bitmapes = new List<BitmapInfo>();
		public GameObject Blogo, Dlogo, Ilogo, Slogo;

		public void init(int id){
			this.id = id;
			state = BlockState.Empty;
			setState (state);
		}

		public void setAsInodeBlock(int inodeStartIndex){
			setState (BlockState.Inode);
			for (int i = 0; i < 16; i++)
				inodes.Add (new InodeInformation (i + inodeStartIndex));
		}

		public void freeInode(int inodeNumber){
			foreach (int block in inodes.Find ((x) => x.inodeNumber == inodeNumber).allocatedBlocks)
				FileManagerGameMaster.getBlock (block).deReference ();
		}

		public void setAsAllocated(int allocatedBytes){
			this.allocatedBytes = allocatedBytes;
			setState (BlockState.AllocateData);
		}


		public void setInodeInformation(int inodeNumber, InodeInformation info){
			int index = inodes.FindIndex ((x) => x.inodeNumber == inodeNumber);
			inodes[index] = info;
		}


		public void setState(BlockState newState){
			state = newState;
			theRenderer.color = FileManagerGameMaster.getStateColor (newState);
		
			//Logos get to pixelated
			/*
			Blogo.SetActive (false);
			Dlogo.SetActive (false);
			Ilogo.SetActive (false);
			Slogo.SetActive (false);

			if (newState == BlockState.SuperBlock)
				Slogo.SetActive (true);
			else if (newState == BlockState.Inode)
				Ilogo.SetActive (true);
			else if (newState == BlockState.BitmapData)
				Blogo.SetActive (true);
			else
				Dlogo.SetActive (true);
			*/
		}

		public void hover(){hoverMask.SetActive (true);}
		public void deHover(){hoverMask.SetActive (false);}
		public void select(){selectMask.SetActive (true);}
		public void deSelect(){selectMask.SetActive (false);}
		public void reference(){referenceMask.SetActive (true);}
		public void deReference(){referenceMask.SetActive (false);}
		public InodeInformation getInodeInformation(int inodeNumber){return inodes.Find ((x) => x.inodeNumber == inodeNumber);}
	}



	public enum BlockState{
		SuperBlock,
		Inode,
		BitmapInode,
		BitmapData,
		Empty,
		AllocateData,
		IndirectPointer
	}
		
}