using System.Collections.Generic;
using Barebones.Networking;

namespace Barebones.MasterServer
{
    public class LobbyPropertiesSetPacket : SerializablePacket
    {
        public string LobbyId;
        public Dictionary<string, string> Properties;

        public override void ToBinaryWriter(EndianBinaryWriter writer)
        {
            writer.Write(LobbyId);
            writer.Write(Properties);
        }

        public override void FromBinaryReader(EndianBinaryReader reader)
        {
            LobbyId = reader.ReadString();
            Properties = reader.ReadDictionary();
        }
    }
}