using System.IO;

namespace VermintideBundleTool.Bundle
{
    public class BundleFileEntryHash
    {
        public ulong Hash1;
        public ulong Hash2;

        public void Read(BinaryReader reader)
        {
            Hash1 = reader.ReadUInt64();
            Hash2 = reader.ReadUInt64();
        }

        public override string ToString()
        {
            return Hash1.ToString() + Hash2.ToString();
        }
    }
}