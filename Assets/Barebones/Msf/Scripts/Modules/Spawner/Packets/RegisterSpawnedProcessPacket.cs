using Barebones.Networking;

namespace Barebones.MasterServer
{
    public class RegisterSpawnedProcessPacket : SerializablePacket
    {
        public string SpawnId;
        public string SpawnCode;

        public override void ToBinaryWriter(EndianBinaryWriter writer)
        {
            writer.Write(SpawnId);
            writer.Write(SpawnCode);
        }

        public override void FromBinaryReader(EndianBinaryReader reader)
        {
            SpawnId = reader.ReadString();
            SpawnCode = reader.ReadString();
        }
    }
}