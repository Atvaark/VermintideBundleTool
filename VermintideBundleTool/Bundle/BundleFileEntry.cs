using System.Diagnostics;
using System.IO;

namespace VermintideBundleTool.Bundle
{
    public class BundleFileEntry
    {
        public BundleFileEntryHash Hash { get; set; }
        public byte[] Data { get; set; }

        public void Read(BinaryReader reader)
        {
            Hash = new BundleFileEntryHash();
            Hash.Read(reader);
            int unknown1 = reader.ReadInt32();
            int unknown2 = reader.ReadInt32();
            int unknown3 = reader.ReadInt32();
            int size = reader.ReadInt32();
            Debug.Assert(size > 0);
            int unknown5 = reader.ReadInt32();
            
            byte[] data = reader.ReadBytes(size);
            Data = data;
        }

    }
}