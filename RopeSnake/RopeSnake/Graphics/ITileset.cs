using System;
using System.Collections.Generic;
using System.Text;

namespace RopeSnake.Graphics
{
    public interface ITileset
    {
        Tile this[int index] { get; set; }
        int Count { get; }
    }
}
