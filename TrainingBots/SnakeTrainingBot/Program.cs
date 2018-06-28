using System;
using System.Net.Sockets;
using System.Text;

namespace SnakeBot {
	public class MainClass {

        private static Random rand = new Random();

        public static Board init(){
			return new Board ();
		}
		public static string makeMove(string incomingData, Board b){
            BoardParser.parseBoard(incomingData, ref b);
            return DecisionMaker.decideNextMove(b);
		}

        
	}
}
