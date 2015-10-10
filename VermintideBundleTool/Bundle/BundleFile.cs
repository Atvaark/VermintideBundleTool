using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Ionic.Zlib;

namespace VermintideBundleTool.Bundle
{
    public class BundleFile
    {
        public List<BundleFileEntry> Entries { get; set; }

        public void Read(Stream input, Stream temporaryFile)
        {
            BinaryReader reader = new BinaryReader(input, Encoding.ASCII, false);

            uint magic = reader.ReadUInt32();
            Debug.Assert(magic == 0xF0000004);

            int uncompressedSize = reader.ReadInt32();
            uint padding = reader.ReadUInt32();
            Debug.Assert(padding == 0x00000000);

            // TODO: Don't create a huge buffer. Just stream the chunks
            int dstOffset = 0;
            while (dstOffset < uncompressedSize)
            {
                // 65536 byte chunks
                int compressedSize = reader.ReadInt32();
                byte[] compressedData = reader.ReadBytes(compressedSize);
                byte[] uncompressedData = ZlibStream.UncompressBuffer(compressedData);
                int length = dstOffset + uncompressedData.Length > uncompressedSize
                    ? uncompressedSize - dstOffset
                    : uncompressedData.Length;
                temporaryFile.Write(uncompressedData, 0, length);
                dstOffset += length;
            }
            temporaryFile.Position = 0;
            ReadEntries(temporaryFile);
        }

        private void ReadEntries(Stream input)
        {
            BinaryReader reader = new BinaryReader(input, Encoding.ASCII, true);
            int entryCount = reader.ReadInt32();
            // Skip the unknown header section
            input.Position += 256;

            var tableEntries = new List<BundleFileEntryHash>();
            for (int i = 0; i < entryCount; i++)
            {
                var tableEntry = new BundleFileEntryHash();
                tableEntry.Read(reader);
                tableEntries.Add(tableEntry);
            }

            var entries = new List<BundleFileEntry>();
            for (int i = 0; i < entryCount; i++)
            {
                var entry = new BundleFileEntry();
                entry.Read(reader);
                entries.Add(entry);
            }

            Entries = entries;
        }
    }
}
