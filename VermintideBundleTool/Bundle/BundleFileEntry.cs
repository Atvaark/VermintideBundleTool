using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace VermintideBundleTool.Bundle
{
    public class BundleFileEntry
    {
        private class BundleFileEntryChunk
        {
            public int Index { get; set; }
            public int Size { get; set; }
            public int Unknown { get; set; }
        }
        private List<BundleFileEntryChunk> Chunks { get; set; }

        public ulong ExtensionHash;

        public ulong NameHash;

        public long DataOffset { get; set; }

        public void Read(BinaryReader reader)
        {
            ExtensionHash = reader.ReadUInt64();
            NameHash = reader.ReadUInt64();

            int chunkCount = reader.ReadInt32();
            int unknown1 = reader.ReadInt32();
            var chunks = new List<BundleFileEntryChunk>(chunkCount);
            for (int i = 0; i < chunkCount; i++)
            {
                var chunk = new BundleFileEntryChunk
                {
                    Index = reader.ReadInt32(),
                    Size = reader.ReadInt32(),
                    Unknown = reader.ReadInt32()
                };
                chunks.Add(chunk);
            }
            Chunks = chunks;
            DataOffset = reader.BaseStream.Position;

            var size = chunks.Sum(c => c.Size);
            reader.BaseStream.Position += size;
        }


        public byte[] ReadData(Stream input)
        {
            input.Position = DataOffset;
            var size = Chunks.Sum(c => c.Size);
            byte[] data = new byte[size];
            input.Read(data, 0, size);
            return data;
        }
    }
}
