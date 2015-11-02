using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VermintideBundleTool.Bundle
{
    public class BundleFileEntry
    {
        public class BundleFileEntryData
        {
            public int LanguageId { get; set; }
            public byte[] Data { get; set; }
        }

        private class BundleFileEntryHeader
        {
            public int LanguageId { get; set; }
            public int Size { get; set; }
            public int Unknown { get; set; }
        }

        private List<BundleFileEntryHeader> EntryHeaders { get; set; }

        public ulong ExtensionHash;

        public ulong NameHash;

        public long DataOffset { get; set; }

        public void Read(BinaryReader reader)
        {
            ExtensionHash = reader.ReadUInt64();
            NameHash = reader.ReadUInt64();

            int count = reader.ReadInt32();
            int unknown1 = reader.ReadInt32();
            var headers = new List<BundleFileEntryHeader>(count);
            for (int i = 0; i < count; i++)
            {
                var header = new BundleFileEntryHeader
                {
                    LanguageId = reader.ReadInt32(),
                    Size = reader.ReadInt32(),
                    Unknown = reader.ReadInt32()
                };
                headers.Add(header);
            }
            EntryHeaders = headers;
            DataOffset = reader.BaseStream.Position;

            var size = headers.Sum(c => c.Size);
            reader.BaseStream.Position += size;
        }


        public IEnumerable<BundleFileEntryData> ReadData(Stream input)
        {
            input.Position = DataOffset;

            foreach (var entryHeader in EntryHeaders)
            {
                byte[] data = new byte[entryHeader.Size];
                input.Read(data, 0, entryHeader.Size);
                yield return new BundleFileEntryData
                {
                    LanguageId = entryHeader.LanguageId,
                    Data = data
                };
            }
        }
    }
}
