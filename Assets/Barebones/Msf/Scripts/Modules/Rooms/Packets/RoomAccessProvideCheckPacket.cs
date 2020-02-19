using Barebones.Networking;

namespace Barebones.MasterServer
{
    public class RoomAccessProvideCheckPacket : SerializablePacket
    {
        public int PeerId;
        public string RoomId;
        public string Username = "";

        public override void ToBinaryWriter(EndianBinaryWriter writer)
        {
            writer.Write(PeerId);
            writer.Write(RoomId);
            writer.Write(Username);
        }

        public override void FromBinaryReader(EndianBinaryReader reader)
        {
            PeerId = reader.ReadInt32();
            RoomId = reader.ReadString();
            Username = reader.ReadString();
        }
    }
}