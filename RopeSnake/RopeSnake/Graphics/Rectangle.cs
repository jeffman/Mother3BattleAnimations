using System;
using System.Collections.Generic;
using System.Text;

namespace RopeSnake.Graphics
{
    /// <summary>
    /// A rectangle defined by its upper-left corner and dimensions.
    /// </summary>
    public struct RectangleR
    {
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public RectangleR(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Gets the smallest rectangle that encloses both this rectangle and the given rectangle.
        /// </summary>
        /// <param name="with">rectangle with which to union</param>
        /// <returns>smallest rectangle containing both rectangles</returns>
        public RectangleR Union(RectangleR with)
        {
            int minX = Math.Min(X, with.X);
            int minY = Math.Min(Y, with.Y);
            int maxX = Math.Max(X, with.X);
            int maxY = Math.Max(Y, with.Y);

            return new RectangleR(minX, minY, maxX - minX, maxY - minY);
        }
    }
}
