using System;
using System.Collections.Generic;
using System.Text;

namespace RopeSnake.Graphics
{
    public interface ITilemap
    {
        TileProperties this[int x, int y] { get; set; }
        int WidthInTiles { get; }
        int HeightInTiles { get; }
    }
}
