using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;



namespace SnakeBot {

    public enum Move {Right, Up, Left,Down}

    public class TCP_API {

        private static NetworkStream stream;
        public static void initStream(NetworkStream albotStream) { stream = albotStream; }


        public static void makeMove(string move) {
            try {
                if (string.IsNullOrEmpty(move.Trim()))
                    sendMsg(" ");
                else
                    makeMove(stringToMove(move));
            }
            catch { sendMsg(" "); }
        }
        public static void makeMove(int move) { makeMove(intToMove(move)); }
        public static void makeMove(Move move) {
            JSONObject jMsg = new JSONObject();
            jMsg.AddField(Constants.Actions.makeMove, move.ToString());
            sendMsg(move.ToString());
        }

        
        public static void sendMsg(string msg) {
            Console.WriteLine("Msg: " + msg);
            byte[] response = Encoding.ASCII.GetBytes(msg);
            stream.Write(response, 0, response.Length);
        }

        private static Move intToMove(int dir) {
            switch (dir) {
                case 0: return Move.Right;
                case 1: return Move.Up;
                case 2: return Move.Left;
                default: return Move.Down;
            }
        }
        private static Move stringToMove(string dir) {
            switch (dir) {
                case "Right": return Move.Right;
                case "Up": return Move.Up;
                case "Left": return Move.Left;
                default: return Move.Down;
            }
        }
    }
}
