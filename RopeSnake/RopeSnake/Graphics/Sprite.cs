using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace RopeSnake.Graphics
{
    public class Sprite : ISprite
    {
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public int WidthInTiles { get; protected set; }
        public int HeightInTiles { get; protected set; }

        public int TileIndex { get; protected set; }
        public int PaletteIndex { get; protected set; }
        public bool FlipX { get; protected set; }
        public bool FlipY { get; protected set; }

        protected Sprite() { }

        public Sprite(int x, int y, int widthInTiles, int heightInTiles,
            int tileIndex, int paletteIndex, bool flipX, bool flipY)
        {
            if (widthInTiles < 1)
                throw new ArgumentOutOfRangeException(nameof(widthInTiles));

            if (heightInTiles < 1)
                throw new ArgumentOutOfRangeException(nameof(heightInTiles));

            if (tileIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(tileIndex));

            if (paletteIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(paletteIndex));

            X = x;
            Y = y;
            WidthInTiles = widthInTiles;
            HeightInTiles = heightInTiles;

            TileIndex = tileIndex;
            PaletteIndex = paletteIndex;
            FlipX = flipX;
            FlipY = flipY;
        }

        /// <summary>
        /// Gets the adjusted tile index at the given tile location.
        /// 
        /// Takes flips into account. For example, with the following properties:
        /// - <c>FlipX</c>: true
        /// - <c>WidthInTiles</c>: 4
        /// - <c>TileIndex</c>: 8
        /// 
        /// Then, <c>GetTileIndexAt(1, 0)</c> will return 10 and <c>GetTileIndexAt(3, 0)</c> will return 8.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public virtual int GetTileIndexAt(int x, int y)
        {
            if (x < 0 || x >= WidthInTiles)
                throw new ArgumentOutOfRangeException(nameof(x));

            if (y < 0 || y >= HeightInTiles)
                throw new ArgumentOutOfRangeException(nameof(y));

            if (FlipX)
                x = WidthInTiles - x - 1;

            if (FlipY)
                y = HeightInTiles - y - 1;

            return TileIndex + x + (y * WidthInTiles);
        }

        public virtual RectangleR GetBounds()
        {
            return new RectangleR(X, Y, WidthInTiles * 8, HeightInTiles * 8);
        }
    }
}
