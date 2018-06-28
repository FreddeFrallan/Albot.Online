using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

using static SnakeBot.Constants;

namespace SnakeBot {

    public enum Move {right, up, left,down}

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
            jMsg.AddField(Actions.makeMove, move.ToString());
            sendMsg(move.ToString());
        }

        
        public static void sendMsg(string msg) {
            Console.WriteLine("Msg: " + msg);
            byte[] response = Encoding.ASCII.GetBytes(msg);
            stream.Write(response, 0, response.Length);
        }

        private static Move intToMove(int dir) {
            switch (dir) {
                case 0: return Move.right;
                case 1: return Move.up;
                case 2: return Move.left;
                default: return Move.down;
            }
        }
        private static Move stringToMove(string dir) {
            switch (dir) {
                case Fields.right: return Move.right;
                case Fields.up: return Move.up;
                case Fields.left: return Move.left;
                default: return Move.down;
            }
        }
    }
}
