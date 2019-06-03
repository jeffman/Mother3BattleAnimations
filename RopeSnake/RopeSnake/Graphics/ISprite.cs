using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace RopeSnake.Graphics
{
    public interface ISprite
    {
        /// <summary>
        /// Top-left X coordinate, in pixels.
        /// </summary>
        int X { get; }

        /// <summary>
        /// Top-left Y coordinate, in pixels.
        /// </summary>
        int Y { get; }

        /// <summary>
        /// Width, in 8-pixel tiles.
        /// </summary>
        int WidthInTiles { get; }

        /// <summary>
        /// Height, in 8-pixel tiles.
        /// </summary>
        int HeightInTiles { get; }

        /// <summary>
        /// Gets the bounds of the sprite, in pixels.
        /// </summary>
        /// <returns>sprite bounds</returns>
        RectangleR GetBounds();

        int TileIndex { get; }
        int PaletteIndex { get; }
        bool FlipX { get; }
        bool FlipY { get; }

        /// <summary>
        /// Returns the one-dimensional tile index at the given tile coordinates.
        /// </summary>
        /// <param name="x">x tile coordinate</param>
        /// <param name="y">y tile coordinate</param>
        /// <returns>tile index</returns>
        int GetTileIndexAt(int x, int y);
    }
}
