using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using RopeSnake.Serialization;

namespace RopeSnake.Helpers
{
    public static class Extensions
    {
        public static BinaryReader ToReader(this byte[] bytes)
        {
            return new BinaryReader((ByteArraySource)bytes);
        }

        public static string Dump(this byte[] bytes)
        {
            return String.Join(" ", bytes.Select(b => b.ToString("X2")).ToArray());
        }
    }
}
