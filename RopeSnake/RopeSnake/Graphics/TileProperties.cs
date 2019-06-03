using System;
using System.Collections.Generic;
using System.Text;

namespace RopeSnake.Graphics
{
    public struct TileProperties
    {
        public int TileIndex { get; }
        public int PaletteIndex { get; }
        public bool FlipX { get; }
        public bool FlipY { get; }

        public TileProperties(int tileIndex, int paletteIndex, bool flipX, bool flipY)
        {
            TileIndex = tileIndex;
            PaletteIndex = paletteIndex;
            FlipX = flipX;
            FlipY = flipY;
        }
    }
}
