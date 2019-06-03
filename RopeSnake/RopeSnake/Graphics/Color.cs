using System;
using System.Collections.Generic;
using System.Text;

namespace RopeSnake.Graphics
{
    public struct Color
    {
        public static readonly Color Transparent = new Color(0, 0, 0, 0);

        public byte R { get; }
        public byte G { get; }
        public byte B { get; }
        public byte A { get; }
        public int Argb { get; }

        public Color(byte r, byte g, byte b) : this(r, g, b, 255) { }

        public Color(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
            Argb = b | (g << 8) | (r << 16) | (a << 24);
        }
    }
}
