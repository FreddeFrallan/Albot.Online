using Barebones.Networking;

namespace Barebones.MasterServer
{
    public class IntPairPacket : SerializablePacket
    {
        public string A;
        public int B;

        public override void ToBinaryWriter(EndianBinaryWriter writer)
        {
            writer.Write(A);
            writer.Write(B);
        }

        public override void FromBinaryReader(EndianBinaryReader reader)
        {
            A = reader.ReadString();
            B = reader.ReadInt32();
        }
    }
}