using System;
using System.Collections.Generic;
using System.Text;

namespace RopeSnake.Graphics
{
    public interface IRenderTarget
    {
        int Width { get; }
        int Height { get; }
        void SetPixel(int x, int y, Color color);
    }
}
