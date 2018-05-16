using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryManager{

	public class GameLogicHeapManager : MonoBehaviour {

		public InputField mallocInputField, freeInputField;
		public MemorySegmentClicker segmentClicker;
		public MemoryArea memArea;
		public GameMasterHeapManager master;	
		public HeapManagerSimulator simulator;

		public void takeTCPInput(string input){
			print ("Got msg:" + input);
			string msg = input.Trim ().ToLower ();
			if (msg.Length == 0)
				return;


			foreach (string s in parseCommand(msg, new string[]{"malloc", "alloc", "merge", "free"}))
				activateCommand (s);
		}


		private string[] parseCommand(string msg, string[] seperators){
			List<int> indexes = new List<int> (){0};
			int temp = 0;
			foreach (string sep in seperators) {
				int i = 0;
				int index = msg.IndexOf (sep, i);
				while (index != -1 && temp++ < 10) {
					indexes.Add (index);
					i = index+1;
					index = msg.IndexOf (sep, i);
				}
			}
			indexes.Add (msg.Length);


			List<string> parsedWords = new List<string> ();
			for (int i = 0; i < indexes.Count - 1; i++) {
				string substring = msg.Substring (indexes [i], indexes [i + 1] - indexes [i]).Trim ();
				if(substring.Length > 0)
					parsedWords.Add (substring); 
			}

			return parsedWords.ToArray ();
		}


		private void activateCommand(string command){
			string[] words = command.Split (' ');

			if (words.Length == 1)
				handleSingleWordInput (words [0]);
			else if (words.Length == 2)
				handleTwoWordInput (words);
			else if(words.Length == 3)
				handleThreeeWordInput (words);

			simulator.receivedInput ();
		}

		private void handleThreeeWordInput(string[] msg){
			int arg1 = -1, arg2 = -1;
			if (int.TryParse (msg [1], out arg1) == false || int.TryParse (msg [2], out arg2) == false)
				return;
			
			switch (msg [0]) {
			case "malloc":
			case "alloc": memArea.allocateArea (arg1, arg2); break;
			case "merge": memArea.mergeSegments (arg1, arg2); break;
			}
		}

		private void handleTwoWordInput(string[] msg){
			int freePos = -1;
			if(msg[0] != "free" || int.TryParse(msg[1], out freePos) == false)
				return;

			memArea.freeArea (freePos);
		}

		private void handleSingleWordInput(string msg){
			if (msg == "restart")
				master.restartGame ();
		}


		public void sendMallocPressed(){
			print (mallocInputField.text);
			TCPLocalConnection.sendMessage ("Malloc " + mallocInputField.text);
		}

		public void sendFreePressed(){
			TCPLocalConnection.sendMessage ("Free " + freeInputField.text);
		}

		public void freeButtonPressed(){
			MemorySegment selectedSeg = segmentClicker.getSelectedSegment ();
			if (selectedSeg != null)
				TCPLocalConnection.sendMessage ("Free " + selectedSeg.pos);
		}
	}
}