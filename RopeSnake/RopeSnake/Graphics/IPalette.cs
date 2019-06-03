using System;
using System.Collections.Generic;
using System.Text;

namespace RopeSnake.Graphics
{
    public interface IPalette
    {
        Color this[int index] { get; set; }
        Color this[int subPalette, int index] { get; set; }

        int SubPaletteCount { get; }
        int ColorsPerSubPalette { get; }
        int TotalColorCount { get; }
    }
}
