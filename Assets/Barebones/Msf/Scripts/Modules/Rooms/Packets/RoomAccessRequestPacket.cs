using System.Collections.Generic;
using Barebones.Networking;

namespace Barebones.MasterServer{

    public class RoomAccessRequestPacket : SerializablePacket{
        public string RoomId;
        public string Password = "";
        public Dictionary<string, string> Properties;

        public override void ToBinaryWriter(EndianBinaryWriter writer)
        {
            writer.Write(RoomId);
            writer.Write(Password);
            writer.Write(Properties);
        }

        public override void FromBinaryReader(EndianBinaryReader reader)
        {
            RoomId = reader.ReadString();
            Password = reader.ReadString();
            Properties = reader.ReadDictionary();
        }
    }
}