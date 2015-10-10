using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace VermintideBundleTool.Bundle
{
    public class BundleDatabaseEntry
    {
        public string Hash { get; set; }
        public string FilePath { get; set; }
        public byte Flag { get; set; }

        public void Read(BinaryReader reader)
        {
            uint magic = reader.ReadUInt32();
            Debug.Assert(magic == 0x00000001);

            Hash = ReadString(reader);
            FilePath = ReadString(reader);

            Flag = reader.ReadByte();
            Debug.Assert(Flag == 0x00 || Flag == 0x01);
        }

        private string ReadString(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            return new string(reader.ReadChars(length));
        }

        public override string ToString()
        {
            return string.Format("Hash: {0}, FilePath: {1}, Flag: {2}", Hash, FilePath, Flag);
        }
    }

    public class BundleDatabaseFile
    {
        public List<BundleDatabaseEntry> Entries { get; set; }

        public void Read(Stream input)
        {
            BinaryReader reader = new BinaryReader(input, Encoding.ASCII, false);

            uint magic = reader.ReadUInt32();
            Debug.Assert(magic == 0x00000002);

            uint fileCount = reader.ReadUInt32();

            var entries = new List<BundleDatabaseEntry>();
            for (int i = 0; i < fileCount; i++)
            {
                var entry = new BundleDatabaseEntry();
                entry.Read(reader);
                entries.Add(entry);
            }

            Entries = entries;
        }

    }
}
