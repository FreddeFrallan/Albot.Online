using System;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;

namespace BrakethroughBot {
	public class MainClass {

		private static NetworkStream stream;


		public static void initBot (int depth) {Search.init(depth);}
		public static string playBotMove (string boardMsg) {
			MoveHandler.playMove(formatBoard(boardMsg));
			return MoveHandler.getMoveString();
		}


		public static void Main (string[] args) {
			/*
			*/
			Search.init(3);
			benchStart();
			return;

			int port = 4000;// int.Parse (Console.ReadLine ());
			string ip = "127.0.0.1";
			TcpClient client = new TcpClient(ip, port);
			stream = client.GetStream();


			while (true) {
				Byte[] data = new byte[256];
				string incomingData = "";

				Int32 bytes = stream.Read(data, 0, data.Length);
				incomingData = Encoding.Default.GetString(data, 0, bytes);
				MoveHandler.playMove(formatBoard(incomingData));
				playMove(MoveHandler.getMoveString());
			}
		}

		private static void benchStart () {
			Stopwatch s = new Stopwatch();
			int amount = 20;
			double average = 0;
			for (int i = 0; i < amount; i++) {
				s.Reset();
				s.Start();
				MoveHandler.playMove(formatBoard(generateStartBoard()));
				s.Stop();
				Console.WriteLine("Time: " + s.ElapsedMilliseconds);
				average += s.ElapsedMilliseconds;
			}

			Console.WriteLine("Average: " + average / amount);
		}


		public static void playMove (int[] start, int[] target) {
			string moveString = Util.coordToStr(start) + Util.coordToStr(target);
			byte[] response = Encoding.Default.GetBytes(moveString);
			stream.Write(response, 0, response.Length);
		}

		public static void playMove (string msg) {
			byte[] response = Encoding.Default.GetBytes(msg);
			stream.Write(response, 0, response.Length);
		}


		private static int[,] formatBoard (string input) {
			string[] words = input.Split(' ');
			int size = (int)Math.Round(Math.Sqrt(words.Length));
			int[,] board = new int[Board.xSize, Board.ySize];

			Util.iterateOverBoard((x, y) => {
				board[x, y] = int.Parse(words[x + y * Board.xSize]);
			});

			return board;
		}

		private static string generateStartBoard () {
			string s = "";
			Util.iterateOverBoard((x, y) => {
				if (y < 2)
					s += "1 ";
				else if (y >= Board.ySize-2)
					s += "-1 ";
				else
					s += "0 ";
			});
			return s;
		}
	}
}
