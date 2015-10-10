using System.IO;

namespace VermintideBundleTool.Bundle
{
    public class BundleFileEntryHash
    {
        public ulong ExtensionHash;
        public ulong NameHash;

        public void Read(BinaryReader reader)
        {
            ExtensionHash = reader.ReadUInt64();
            NameHash = reader.ReadUInt64();
        }

        public override string ToString()
        {
            return ExtensionHash.ToString() + NameHash.ToString();
        }
    }
}