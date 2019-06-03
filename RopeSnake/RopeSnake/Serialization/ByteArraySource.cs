using System;
using System.Collections.Generic;
using System.Text;
using RopeSnake.Helpers;

namespace RopeSnake.Serialization
{
    public sealed class ByteArraySource : IBinarySource
    {
        private byte[] _data { get; }

        internal byte[] Data => _data;

        public byte this[int offset]
        {
            get => _data[offset];
            set => _data[offset] = value;
        }

        public int Length => _data.Length;

        /// <summary>
        /// Creates a new <c>ByteArraySource</c> around the given array. Does not create a copy; simply wraps the reference.
        /// </summary>
        /// <param name="data">array to wrap</param>
        public ByteArraySource(byte[] data)
        {
            data.ThrowIfNull(nameof(data));
            _data = data;
        }

        /// <summary>
        /// Creates a new <c>ByteArraySource</c> with the given byte length.
        /// </summary>
        /// <param name="length">number of bytes</param>
        public ByteArraySource(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            _data = new byte[length];
        }

        /// <summary>
        /// Creates a copy of an existing <c>IBinarySource</c> using the given starting offset and length.
        /// </summary>
        /// <param name="copyFrom">source to copy from</param>
        /// <param name="startOffset">offset to start copying from</param>
        /// <param name="length">number of bytes to copy</param>
        public ByteArraySource(IBinarySource copyFrom, int startOffset, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (startOffset + length >= copyFrom.Length)
                throw new ArgumentOutOfRangeException("Source range exceeds source length");

            _data = new byte[length];
            for (int i = 0; i < length; i++)
            {
                _data[i] = copyFrom[i + startOffset];
            }
        }

        /// <summary>
        /// Explicit conversion from a byte array to a <c>ByteArraySource</c>.
        /// </summary>
        /// <param name="bytes">byte array to convert</param>
        public static explicit operator ByteArraySource(byte[] bytes)
        {
            return new ByteArraySource(bytes);
        }
    }
}
