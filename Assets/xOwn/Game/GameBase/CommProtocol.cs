using System.Collections;
using System.Collections.Generic;
using System;

namespace Game{

	public abstract class CommProtocol{
		protected Action<Object, int, short> sendMsg;
		protected List<AlbotMessage> currentProtocol = new List<AlbotMessage>();
		protected List<AlbotMessage> currentHandlers = new List<AlbotMessage>();


        public void sendPingMsg(int targetID, Game.PlayerColor color) {sendString(targetID, (short)AlbotServer.ServerCommProtocl.Ping, "PING", color);}
        public void sendString(int targetID, short type, string str, Game.PlayerColor color) {sendMsg(new StringMessage(str, color), targetID, type);}


        public abstract void init(Action<Object, int, short> sendMsgFunc);
		public Type getMatchingType(int id){ return currentProtocol.Find (x => x.id == id).type;}
		public void subscribeHandler(ConnectedClient target, Action<object, ConnectedClient> func, short msgType, GameWrapper wrapper, Type type = null){
			if (type == null)
				type = typeof(StringMessage);
			wrapper.registerHandler (new AlbotHandler (msgType, func, type), target.peerID);
		}
			
		public struct AlbotMessage{
			public Type type;
			public short id;
			public AlbotMessage(Type type, short id){
				this.type = type;
				this.id = id;
			}
		}
		public struct AlbotHandler{
			public short msgType;
			public Action<object, ConnectedClient> func;
			public Type type;
			public AlbotHandler(short msgType, Action<object, ConnectedClient> func, Type type){
				this.func = func;
				this.msgType = msgType;
				this.type = type;
			}
		}

		[Serializable]
		public class StringMessage{ 
			public string msg;  
			public Game.PlayerColor color;
			public StringMessage(string msg, PlayerColor color = PlayerColor.None){this.msg = msg; this.color = color;}
		}
	}

}