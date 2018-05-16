using System;
using System.Net.Sockets;
using System.Text;

namespace SnakeBot {
	public class MainClass {


		public static CurrentBoard init(){
			return new CurrentBoard ();
		}

		public static string makeMove(string incomingData, CurrentBoard b){
			JSONObject obj = new JSONObject(incomingData);
			JSONObject player = obj.GetField("Player");
			JSONObject enemy = obj.GetField("Player");

			JSONObject blocked = obj.GetField("Blocked");
			b.handleBlockUpdate(blocked.list);

			return DecisionMaker.decideNextMove(b, player);
		}

		/*
		static void Main (string[] args) {

			//Connect to Albot using the port 4000
			TcpClient client = new TcpClient("127.0.1", 4000);
			NetworkStream stream = client.GetStream();

			Byte[] dataBuffer = new byte[1024 * 10];
			Random r = new Random();

			try {
				//Infinite game loop
				while (true) {
					//We decode the incoming message into a string
					Int32 bytes = stream.Read(dataBuffer, 0, dataBuffer.Length);
					string incomingData = Encoding.Default.GetString(dataBuffer, 0, bytes);

					JSONObject obj = new JSONObject(incomingData);
					JSONObject player = obj.GetField("Player");
					JSONObject enemy = obj.GetField("Player");

					JSONObject blocked = obj.GetField("Blocked");
					board.handleBlockUpdate(blocked.list);

					string moveOrder = DecisionMaker.decideNextMove(board, player);
					//Send the response
					byte[] response = Encoding.ASCII.GetBytes(moveOrder);
					stream.Write(response, 0, response.Length);
				}
			} catch { }
		}
		*/
	}
}
