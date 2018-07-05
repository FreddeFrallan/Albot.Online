using System;
using System.Net.Sockets;
using System.Text;

namespace SnakeBot {
	public class MainClass {

        private static Random rand = new Random();
        private static Action<string> printFunc;

        public static Board init(){
			return new Board ();
		}
		public static string makeMove(string incomingData, Board b){
            BoardParser.parseBoard(incomingData, ref b);
            return DecisionMaker.decideNextMove(b);
		}

        public static void setPrintFunction(Action<string> printFunc) {
            MainClass.printFunc = printFunc;
        }

        public static void debugPrint(string msg) {
            printFunc("SnakeTrainingBot DebugLog: " + msg);
        }

        
	}
}
