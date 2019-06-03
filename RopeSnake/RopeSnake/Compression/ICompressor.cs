using System;
using System.Collections.Generic;
using System.Text;
using RopeSnake.Serialization;

namespace RopeSnake.Compression
{
    public interface ICompressor
    {
        IBinarySource Compress(IBinarySource source, int offset, int length);
        IBinarySource Decompress(IBinarySource source, int offset);
    }
}
