using System.Collections;
using System.Collections.Generic;
using UnityEngine;


	namespace Pickup{
	public class AvailableSearch{

		private static List<int[]> searchedPos = new List<int[]> ();
		private static int size;

		public static bool validMap(string[,] map, List<int[]> connectItems){
			List<int[]> tempItems = new List<int[]> ();
			tempItems.AddRange (connectItems);

			searchedPos.Clear ();
			size = map.GetLength (0);
			int[] startPos = tempItems [0];
			tempItems.RemoveAt (0);
			depthSearch (map, startPos, tempItems);

			return tempItems.Count == 0;
		}



		private static void depthSearch(string[,] map, int[] currentPos,  List<int[]> connectItems){
			searchedPos.Add (currentPos);

			foreach (int[] move in getAvailableMoves(currentPos, map)) {
				if (connectItems.Count == 0) //Tiny optimization
					return;
				
				if (alreadySearchedPos (move))
					continue;
				if(matchingItem(move, connectItems))
					removePos(move, connectItems);
				depthSearch (map, move, connectItems);
			}
		}


		private static List<int[]> getAvailableMoves(int[] currentPos, string[,] map){
			List<int[]> availableMoves = new List<int[]> (), validMoves = new List<int[]> ();;
			availableMoves.Add( new int[]{ currentPos [0] - 1, currentPos [1] });
			availableMoves.Add( new int[]{ currentPos [0] + 1, currentPos [1] });
			availableMoves.Add( new int[]{ currentPos [0], currentPos [1] + 1});
			availableMoves.Add( new int[]{ currentPos [0], currentPos [1] -1});

			foreach (int[] p in availableMoves) {
				if (p [0] < 0 || p [0] >= size || p [1] < 0 || p [1] >= size)
					continue;
				if (map [p [0], p [1]] == "X")
					continue;
				validMoves.Add (p);
			}

			return validMoves;
		}


		#region utils
		private static bool alreadySearchedPos(int[] p){return searchedPos.Find (x => x [0] == p [0] && x [1] == p [1]) != null;}
		private static bool matchingItem(int[] p, List<int[]> items){return items.Find (x => x [0] == p [0] && x [1] == p [1]) != null;}
		private static void removePos(int[] p, List<int[]> items){items.Remove( items.Find (x => x [0] == p [0] && x [1] == p [1]));}
		#endregion
	}
}