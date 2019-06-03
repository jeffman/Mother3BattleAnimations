using System;
using System.Collections.Generic;
using System.Text;
using RopeSnake.Helpers;

namespace RopeSnake.Serialization
{
    public sealed class BinaryReader
    {
        /// <summary>
        /// Gets the <c>BinarySource</c> associated with this reader.
        /// </summary>
        public IBinarySource Source { get; }

        /// <summary>
        /// Gets the endianness associated with this reader.
        /// </summary>
        public Endianness Endianness { get; }

        private Func<int, short> _readShortFunc;
        private Func<int, ushort> _readUShortFunc;
        private Func<int, int> _readIntFunc;
        private Func<int, uint> _readUIntFunc;

        public BinaryReader(IBinarySource source, Endianness endianness = Endianness.Little)
        {
            source.ThrowIfNull(nameof(source));
            Source = source;

            Endianness = endianness;
            AssignReaders();
        }

        private void AssignReaders()
        {
            switch (Endianness)
            {
                case Endianness.Little:
                    _readShortFunc = ReadShortLittle;
                    _readUShortFunc = ReadUShortLittle;
                    _readIntFunc = ReadIntLittle;
                    _readUIntFunc = ReadUIntLittle;
                    break;

                case Endianness.Big:
                    _readShortFunc = ReadShortBig;
                    _readUShortFunc = ReadUShortBig;
                    _readIntFunc = ReadIntBig;
                    _readUIntFunc = ReadUIntBig;
                    break;

                default:
                    throw new InvalidOperationException($"Invalid endianness: {Endianness}");
            }
        }

        /// <summary>
        /// Reads a single unsigned byte.
        /// </summary>
        /// <param name="offset">source offset</param>
        /// <returns>unsigned byte at <c>offset</c></returns>
        public byte ReadByte(int offset) => Source[offset];

        /// <summary>
        /// Reads a single signed byte.
        /// </summary>
        /// <param name="offset">source offset</param>
        /// <returns>signed byte at <c>offset</c></returns>
        public sbyte ReadSByte(int offset) => (sbyte)ReadByte(offset);

        /// <summary>
        /// Reads a signed short.
        /// </summary>
        /// <param name="offset">source offset</param>
        /// <returns>signed short at <c>offset</c></returns>
        public short ReadShort(int offset) => _readShortFunc(offset);

        /// <summary>
        /// Reads an unsigned short.
        /// </summary>
        /// <param name="offset">source offset</param>
        /// <returns>unsigned short at <c>offset</c></returns>
        public ushort ReadUShort(int offset) => _readUShortFunc(offset);

        /// <summary>
        /// Reads a signed int.
        /// </summary>
        /// <param name="offset">source offset</param>
        /// <returns>signed int at <c>offset</c></returns>
        public int ReadInt(int offset) => _readIntFunc(offset);

        /// <summary>
        /// Reads an unsigned int.
        /// </summary>
        /// <param name="offset">source offset</param>
        /// <returns>signed int at <c>offset</c></returns>
        public uint ReadUInt(int offset) => _readUIntFunc(offset);

        /// <summary>
        /// Reads a sequence of bytes as a new byte array.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] ReadBytes(int offset, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            var data = new byte[length];

            if (Source is ByteArraySource byteSource)
            {
                Array.Copy(byteSource.Data, offset, data, 0, length);
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    data[i] = Source[offset + i];
                }
            }

            return data;
        }

        private short ReadShortLittle(int offset) => (short)(ReadByte(offset) | (ReadByte(offset + 1) << 8));
        private short ReadShortBig(int offset) => (short)((ReadByte(offset) << 8) | ReadByte(offset + 1));

        private ushort ReadUShortLittle(int offset) => (ushort)ReadShortLittle(offset);
        private ushort ReadUShortBig(int offset) => (ushort)ReadShortBig(offset);

        private int ReadIntLittle(int offset) =>
            ReadByte(offset) |
            (ReadByte(offset + 1) << 8) |
            (ReadByte(offset + 2) << 16) |
            (ReadByte(offset + 3) << 24);

        private int ReadIntBig(int offset) =>
            (ReadByte(offset) << 24) |
            (ReadByte(offset + 1) << 16) |
            (ReadByte(offset + 2) << 8) |
            ReadByte(offset + 3);

        private uint ReadUIntLittle(int offset) => (uint)ReadIntLittle(offset);
        private uint ReadUIntBig(int offset) => (uint)ReadIntBig(offset);
    }
}
