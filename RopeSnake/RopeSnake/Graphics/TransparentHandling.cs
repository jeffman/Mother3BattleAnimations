using System;
using System.Collections.Generic;
using System.Text;

namespace RopeSnake.Graphics
{
    public enum TransparentHandling
    {
        /// <summary>
        /// Skips rendering of transparent pixels. Background will show through.
        /// </summary>
        DrawNothing,

        /// <summary>
        /// Renders transparent pixels as RGBA(0, 0, 0, 0). Erases the background.
        /// </summary>
        DrawTransparent,

        /// <summary>
        /// Renders whatever color is at index 0 in the palette as a solid color.
        /// </summary>
        DrawSolid
    }
}
