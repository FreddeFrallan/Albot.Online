using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using BumpTag;
using System;
using UnityEngine.Networking;


namespace BumpTag{
	public class BumpTagClientController : Game.ClientController {
		private Game.PlayerColor playerColor;
		[SerializeField]
		private Renderer gameRenderer;
	//	private BumpTag.CommProtocol protocol;


		public override void initProtocol (Game.CommProtocol protocol){ /*this.protocol = new BumpTag.CommProtocol ();*/}
		public override Game.GameType getGameType (){return Game.GameType.BumtTag;}
		protected override void initHandlers (){
			connectionToServer.RegisterHandler ((short)Connect4.CommProtocol.MsgType.boardUpdate, requestMove);
			connectionToServer.RegisterHandler ((short)Connect4.CommProtocol.MsgType.playerInit, initSettings);
			connectionToServer.RegisterHandler ((short)Connect4.CommProtocol.MsgType.RPCMove, handleRPCMove);
			connectionToServer.RegisterHandler ((short)Connect4.CommProtocol.MsgType.gameInfo, handleGameStatus);
			StartCoroutine (handleNetworkMsgQueue ());
			StartCoroutine (findRenderer ());
		}


		IEnumerator findRenderer(){
			while (gameRenderer == null) {
				GameObject foundObj = GameObject.FindGameObjectWithTag ("GameController");
				if (foundObj != null)
					gameRenderer = foundObj.GetComponent<Renderer> ();	

				yield return new WaitForEndOfFrame ();
			}
		}
				

		//InProgress
		public override void onOutgoingLocalMsg (string msg, Game.PlayerColor color){
			sendServerMsg(msg, color, (short)CommProtocol.MsgType.move);
		}

		#region Bunch of Server communication handlers
		public void requestMove(NetworkMessage boardMsg){
			/*
			byte[] bytes = boardMsg.reader.ReadBytesAndSize ();
			string msg = Game.ClientController.Deserialize<Game.CommProtocol.StringMessage> (bytes).msg;

			isListening = true;
			TCPLocalConnection.sendMessage (msg, 0);
			print ("ServerMsg: " + msg);

			string[] words = msg.Split (new char[]{ ' ' });
			PlayerMove moveMsg = new PlayerMove (playerColor, UnityEngine.Random.Range (0, 3), int.Parse(words [0]));

			using (MemoryStream stream = new MemoryStream()){
				new BinaryFormatter().Serialize(stream, moveMsg);
				bytes = stream.ToArray();
				sendServerMsg(bytes, (short)CommProtocol.MsgType.move);
			}
			*/
		}
		public void initSettings(NetworkMessage initMsg){
			byte[] bytes = initMsg.reader.ReadBytesAndSize ();
			GameInfo msg = Game.ClientController.Deserialize<GameInfo> (bytes);

			Debug.LogError ("You play as: " + msg.myColor);
		}
		public void handleRPCMove(NetworkMessage RPCMsg){		
			byte[] bytes = RPCMsg.reader.ReadBytesAndSize ();
			BumpTag.PlayerMove msg = Game.ClientController.Deserialize<BumpTag.PlayerMove> (bytes);
			Debug.LogError ("Color:" + msg.color + "  ID:" + msg.pieceID + "  Steps:" + msg.steps);
		//	gameRenderer.movePice (msg.color, msg.pieceID, msg.steps);
		}
		public void handleGameStatus(NetworkMessage gameStatusMsg){
			byte[] bytes = gameStatusMsg.reader.ReadBytesAndSize ();
			GameInfo msg = Game.ClientController.Deserialize<GameInfo> (bytes);

			if (msg.gameOver) {
				TCPLocalConnection.sendMessage ("GameOver");
			}
		}
		#endregion
	}

}
