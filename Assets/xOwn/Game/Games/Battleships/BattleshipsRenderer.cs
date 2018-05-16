using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using AlbotServer;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Battleship{

	public class BattleshipsRenderer : MonoBehaviour{

		public GameBoardCreator gameBoardCreator;
		public PlayerBoard p1Board, p2Board;
		public ClientUI.ClientUserPanelUI[] panels;
		protected BattleshipProtocol protocol;
		private GameUI localGameUI;
		private Game.PlayerColor myColor;
		private List<Game.PlayerColor> initColors = new List<Game.PlayerColor> ();


		public void playMove(int target, Game.PlayerColor currentPlayerColor, char targetStatus, ShipType targetType, int[] startPos, bool horizontal){
			PlayerBoard targetedPlayer = currentPlayerColor == Game.PlayerColor.Blue ? p2Board : p1Board;
			targetedPlayer.fireAtGrid (target, myColor != currentPlayerColor, targetStatus, targetType, startPos, horizontal);
			panels [0].setScore(p1Board.calculateHP ());
			panels [1].setScore(p2Board.calculateHP ());
		}


		public void initBoard(char[,] charBoard, Game.PlayerColor currentColor){
			myColor = currentColor;
			PlayerBoard currentPlayer = currentColor == Game.PlayerColor.Blue ? p1Board : p2Board;
			gameBoardCreator.initPlayerBoard (charBoard, currentPlayer);
			initColors.Add (currentColor);
		}


		public static void printCharBoard(char[,] charboard){
			for (int y = 0; y < 10; y++) {
				string temp = "";
				for (int x = 0; x < 10; x++)
					temp += charboard [x, y] + " ";
			}
		}
			
	}
}