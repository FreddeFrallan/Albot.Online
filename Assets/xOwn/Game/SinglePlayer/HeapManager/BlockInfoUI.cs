using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FileManager;

namespace MemoryManager{

	public class BlockInfoUI : MonoBehaviour {

		public Text blockConsole;


		public void displayNewBlockInfo(MemorySegment segment){
			string infoText = "<b>Block ID:</b> ";
			infoText += "<i>" + "1" + "</i>\n\n";

			infoText += "<b>Position:</b> ";
			infoText += "<i>" + segment.pos + "</i>\n\n";

			infoText += "<b>Size:</b> ";
			infoText += "<i>" + segment.size + "</i>\n\n";

			blockConsole.text = infoText;
		}


		public void displayNewFileBlock(FileManagerBlock block){
			string infoText = "<b>Block ID:</b> ";
			infoText += "<i>" + block.id  + "</i>\n\n";

			infoText += "<b>Type:</b> "; 
			infoText += "<i>" + block.state + "</i>"; 

			blockConsole.text = infoText;
		}
	}

}