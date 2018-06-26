using Barebones.Networking;

namespace Barebones.MasterServer
{
    public class KillSpawnedProcessPacket : SerializablePacket
    {
        public string SpawnerId;
        public string SpawnId;

        public override void ToBinaryWriter(EndianBinaryWriter writer)
        {
            writer.Write(SpawnerId);
            writer.Write(SpawnId);
        }

        public override void FromBinaryReader(EndianBinaryReader reader)
        {
            SpawnerId = reader.ReadString();
            SpawnId = reader.ReadString();
        }
    }
}