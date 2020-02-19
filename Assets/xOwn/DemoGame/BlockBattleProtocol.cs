using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BlockBattle{

	public class BlockBattleProtocol : Game.CommProtocol {

		#region MsgID
		//Defines all messages and their ID
		//It's good practice to explicity write the corresponding ID, since it might come helpful in debuging.
		public enum MsgType : short {
			gameInfo = 800,
			gameInit = 801,
			gameUpdate = 802,
			playerMove = 803,
		}
		#endregion	


		#region Init
		//Initialize the message handler and adds all of the types of outgoing messages.
		public override void init (System.Action<object, int, short> sendMsgFunc){
			this.sendMsg = sendMsgFunc;

			currentProtocol.Add(new AlbotMessage(typeof(GameInfo), (short)MsgType.gameInfo));
			currentProtocol.Add(new AlbotMessage(typeof(GameInfo), (short)MsgType.gameInit));
			currentProtocol.Add(new AlbotMessage(typeof(GameUpdate), (short)MsgType.gameUpdate));
		}


		public void sendGameInfo(int targetID, GameInfo gi) {sendMsg(gi, targetID, (short)MsgType.gameInfo);}
		public void sendGameInit(int targetID, GameInfo gi) {sendMsg(gi, targetID, (short)MsgType.gameInit);}
		public void sendBoard(int targetID, GameUpdate board){sendMsg(board, targetID, (short)MsgType.gameUpdate);}
		#endregion

	}
	#region Message Classes
	//Used for gameInfo and gameInit messages
	[Serializable]
	public class GameInfo{
		public string username;
		public Game.PlayerColor myColor;
		public bool gameOver;
		public Game.PlayerColor winnerColor;
		public GameInfo(string username, Game.PlayerColor myColor, bool gameOver = false, 
			Game.PlayerColor winnerColor = Game.PlayerColor.None){

			this.username = username;
			this.myColor = myColor;
			this.gameOver = gameOver;
			this.winnerColor = winnerColor;
		}
	}


	//Containing all the information about the current state of the game
	//Will be sent every update from the server to all the clients
	[Serializable]
	public class GameUpdate{
		public int[] redCoords, blueCoords;
		public uint updateNumber;
		public Game.PlayerColor myColor;

		public GameUpdate (){}
		public GameUpdate(uint updateNumber, int[] redC, int[] blueC, 
			Game.PlayerColor myColor = Game.PlayerColor.None, bool gameOver = false, 
			Game.PlayerColor winnerColor = Game.PlayerColor.None){

			this.myColor = myColor;
			this.updateNumber = updateNumber;
			this.redCoords = redC;
			this.blueCoords = blueC;
		}
	}

	// A msg from the client to the server containing information about the requested move from the player
	[Serializable]
	public class PlayerMove{
		public int[] move;
		public Game.PlayerColor player;
		public PlayerMove (){}
		public PlayerMove(int[] targetCoords, Game.PlayerColor playerColor){
			this.move = targetCoords;
			this.player = playerColor;
		}
	}

	#endregion
	
}