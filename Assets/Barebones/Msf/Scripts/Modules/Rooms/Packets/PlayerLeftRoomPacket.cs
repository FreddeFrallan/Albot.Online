using Barebones.Networking;

namespace Barebones.MasterServer
{
    public class PlayerLeftRoomPacket : SerializablePacket
    {
        public int PeerId;
        public string RoomId;

        public override void ToBinaryWriter(EndianBinaryWriter writer)
        {
            writer.Write(PeerId);
            writer.Write(RoomId);
        }

        public override void FromBinaryReader(EndianBinaryReader reader)
        {
            PeerId = reader.ReadInt32();
            RoomId = reader.ReadString();
        }
    }
}