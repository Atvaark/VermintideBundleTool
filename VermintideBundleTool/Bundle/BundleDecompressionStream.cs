using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Ionic.Zlib;

namespace VermintideBundleTool.Bundle
{
    public class BundleDecompressionStream : Stream
    {
        public BundleDecompressionStream(Stream innerStream)
        {
            _reader = new BinaryReader(innerStream, Encoding.ASCII, true);
            _internalBuffer = new byte[0];
        }

        private readonly BinaryReader _reader;

        private bool _isHeaderRead;
        private bool _isEndOfStream;

        private long _uncompressedSize;
        private long _uncompressedBytesRead;
        private long _uncompressedPosition;

        private byte[] _internalBuffer;
        private int _internalBufferOffset;

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException("Seek is not supported");
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException("SetLength is not supported");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!_isHeaderRead)
            {
                ReadHeader();
            }

            return ReadInternal(buffer, offset, count);
        }

        private void ReadHeader()
        {
            uint magic = _reader.ReadUInt32();
            Debug.Assert(magic == 0xF0000004);

            _uncompressedSize = _reader.ReadInt32();
            uint padding = _reader.ReadUInt32();
            Debug.Assert(padding == 0x00000000);

            _isHeaderRead = true;
        }

        private int ReadInternal(byte[] buffer, int offset, int size)
        {
            int bytesRead = 0;

            while (size > 0)
            {
                // Read next buffered chunk.
                int internalBufferLength = _internalBuffer.Length - _internalBufferOffset;
                if (internalBufferLength > 0)
                {
                    var copyRemainingBytes = size <= internalBufferLength;
                    int bytesToCopy = copyRemainingBytes
                        ? size
                        : internalBufferLength;

                    Buffer.BlockCopy(_internalBuffer, _internalBufferOffset, buffer, offset, bytesToCopy);

                    size -= bytesToCopy;
                    offset += bytesToCopy;
                    bytesRead += bytesToCopy;
                    _internalBufferOffset += bytesToCopy;
                    _uncompressedPosition += bytesToCopy;

                    if (copyRemainingBytes)
                    {
                        break;
                    }
                }

                if (_isEndOfStream)
                {
                    break;
                }

                ReadNextChunk();
            }

            return bytesRead;
        }

        private void ReadNextChunk()
        {
            int compressedSize = _reader.ReadInt32();
            byte[] compressedData = _reader.ReadBytes(compressedSize);
            _internalBuffer = Uncompress(compressedData);
            _uncompressedBytesRead += _internalBuffer.Length;
            _internalBufferOffset = 0;

            if (_uncompressedBytesRead >= _uncompressedSize)
            {
                _isEndOfStream = true;
                // Remove the padding in the last chunk.
                int internalBufferSize = (int)(_internalBuffer.Length - (_uncompressedBytesRead - _uncompressedSize));
                Array.Resize(ref _internalBuffer, internalBufferSize);
            }
        }

        private byte[] Uncompress(byte[] data)
        {
            return ZlibStream.UncompressBuffer(data);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException("Write is not supported");
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return _uncompressedSize; }
        }

        public override long Position
        {
            get
            {
                return _uncompressedPosition;
            }
            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }
    }
}