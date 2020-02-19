using Barebones.Networking;

namespace Barebones.MasterServer
{
    public class SpawnStatusUpdatePacket : SerializablePacket
    {
        public string SpawnId;
        public SpawnStatus Status;

        public override void ToBinaryWriter(EndianBinaryWriter writer)
        {
            writer.Write(SpawnId);
            writer.Write((int) Status);
        }

        public override void FromBinaryReader(EndianBinaryReader reader)
        {
            SpawnId = reader.ReadString();
            Status = (SpawnStatus)reader.ReadInt32();
        }
    }
}