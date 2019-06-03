using System;
using System.Collections.Generic;
using System.Text;

namespace RopeSnake.Graphics
{
    public class Tile
    {
        /// <summary>
        /// Returns the tile width, in pixels;
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Returns the tile height, in pixels;
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Gets or sets the pixel value at a given location.
        /// </summary>
        /// <param name="x">horizontal location, in pixels</param>
        /// <param name="y">vertical location, in pixels</param>
        /// <returns>pixel value</returns>
        public byte this[int x, int y]
        {
            get => _pixels[x, y];
            set => _pixels[x, y] = value;
        }

        private byte[,] _pixels;

        public Tile(int width, int height)
        {
            if (width < 1 || width > 8)
                throw new ArgumentOutOfRangeException(nameof(width));

            if (height < 1 || height > 8)
                throw new ArgumentOutOfRangeException(nameof(height));

            _pixels = new byte[width, height];
            Width = width;
            Height = height;
        }
    }
}
