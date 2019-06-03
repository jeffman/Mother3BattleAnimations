using System;
using System.Collections.Generic;
using System.Text;
using RopeSnake.Graphics;

namespace RopeSnake.Gba.Graphics
{
    public class GbaSprite : Sprite
    {
        private static readonly Dictionary<(GbaSpriteShape shape, int size), (int width, int height)>
            _spriteSizeLookup = new Dictionary<(GbaSpriteShape shape, int size), (int width, int height)>();

        public int Priority { get; }

        static GbaSprite()
        {
            _spriteSizeLookup[(GbaSpriteShape.Square, 0)] = (1, 1);
            _spriteSizeLookup[(GbaSpriteShape.Square, 1)] = (2, 2);
            _spriteSizeLookup[(GbaSpriteShape.Square, 2)] = (4, 4);
            _spriteSizeLookup[(GbaSpriteShape.Square, 3)] = (8, 8);

            _spriteSizeLookup[(GbaSpriteShape.Horizontal, 0)] = (2, 1);
            _spriteSizeLookup[(GbaSpriteShape.Horizontal, 1)] = (4, 1);
            _spriteSizeLookup[(GbaSpriteShape.Horizontal, 2)] = (4, 2);
            _spriteSizeLookup[(GbaSpriteShape.Horizontal, 3)] = (8, 4);

            _spriteSizeLookup[(GbaSpriteShape.Vertical, 0)] = (1, 2);
            _spriteSizeLookup[(GbaSpriteShape.Vertical, 1)] = (1, 4);
            _spriteSizeLookup[(GbaSpriteShape.Vertical, 2)] = (2, 4);
            _spriteSizeLookup[(GbaSpriteShape.Vertical, 3)] = (4, 8);
        }

        public GbaSprite(int x, int y, GbaSpriteShape shape, int sizeIndex,
            int tileIndex, int paletteIndex, bool flipX, bool flipY,
            int priority)
        {
            if (x < -256 || x > 255)
                throw new ArgumentOutOfRangeException(nameof(x));

            if (y < -128 || y > 127)
                throw new ArgumentOutOfRangeException(nameof(y));

            if (shape < 0 || (int)shape > 2)
                throw new ArgumentOutOfRangeException(nameof(shape));

            if (sizeIndex < 0 || sizeIndex > 3)
                throw new ArgumentOutOfRangeException(nameof(sizeIndex));

            if (tileIndex < 0 || tileIndex > 1023)
                throw new ArgumentOutOfRangeException(nameof(tileIndex));

            if (paletteIndex < 0 || paletteIndex > 15)
                throw new ArgumentOutOfRangeException(nameof(paletteIndex));

            if (priority < 0 || priority > 3)
                throw new ArgumentOutOfRangeException(nameof(priority));

            X = x;
            Y = y;
            TileIndex = tileIndex;
            PaletteIndex = paletteIndex;
            FlipX = flipX;
            FlipY = flipY;

            (WidthInTiles, HeightInTiles) = _spriteSizeLookup[(shape, sizeIndex)];
            Priority = priority;
        }
    }
}
