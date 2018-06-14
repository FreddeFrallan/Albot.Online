using System;
using System.Net.Sockets;
using System.Text;

namespace SnakeBot {
	public class MainClass {



        /*
		public static Board init(){
			return new Board ();
		}
		public static string makeMove(string incomingData, Board b){
            BoardParser.parseBoard(incomingData, ref b);
            return DecisionMaker.decideNextMove(b);
		}
        */
        private static Random rand = new Random();

		static void Main (string[] args) {
			//Connect to Albot using the port 4000
			TcpClient client = new TcpClient("127.0.0.1", 4000);
			NetworkStream stream = client.GetStream();
			Byte[] dataBuffer = new byte[1024 * 10];
            //TCP_API.initStream(stream);

            //Board currentBoard = new Board();
			//Infinite game loop
			while (true) {
				//We decode the incoming message into a string
				Int32 bytes = stream.Read(dataBuffer, 0, dataBuffer.Length);
				string incomingData = Encoding.Default.GetString(dataBuffer, 0, bytes);

                Console.WriteLine(incomingData);

                byte[] response = Encoding.ASCII.GetBytes(getRandomDir());
                stream.Write(response, 0, response.Length);
                /*
                BoardParser.parseBoard(incomingData, ref currentBoard);

				string move = DecisionMaker.decideNextMove(currentBoard);
                Console.WriteLine("Pre send: " + move);
                TCP_API.makeMove(move);
                */
            }
		}

        private static string getRandomDir() {
            switch(rand.Next(0, 4)){
                case 0: return "Right";
                case 1: return "Up";
                case 2: return "Left";
                default: return "Down";
            }
        }
		
	}
}
