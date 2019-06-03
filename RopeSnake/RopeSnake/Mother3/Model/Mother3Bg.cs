using System;
using System.Collections.Generic;
using System.Text;
using RopeSnake.Helpers;
using RopeSnake.Graphics;

namespace RopeSnake.Mother3.Model
{
    public class Mother3Bg
    {
        public uint UnknownA { get; }
        public uint UnknownB { get; }
        public Tilemap Tilemap { get; }

        public Mother3Bg(Tilemap tilemap, uint unknownA, uint unknownB)
        {
            tilemap.ThrowIfNull(nameof(tilemap));

            Tilemap = tilemap;
            UnknownA = unknownA;
            UnknownB = unknownB;
        }
    }
}
