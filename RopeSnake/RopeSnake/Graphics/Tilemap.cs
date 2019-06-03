using System;
using System.Collections.Generic;
using System.Text;

namespace RopeSnake.Graphics
{
    public class Tilemap : ITilemap
    {
        public TileProperties this[int x, int y]
        {
            get => _tileProperties[x, y];
            set => _tileProperties[x, y] = value;
        }

        public int WidthInTiles { get; }
        public int HeightInTiles { get; }

        private TileProperties[,] _tileProperties;

        public Tilemap(int widthInTiles, int heightInTiles)
        {
            if (widthInTiles < 1)
                throw new ArgumentOutOfRangeException(nameof(widthInTiles));

            if (heightInTiles < 1)
                throw new ArgumentOutOfRangeException(nameof(heightInTiles));

            _tileProperties = new TileProperties[widthInTiles, heightInTiles];
            WidthInTiles = widthInTiles;
            HeightInTiles = heightInTiles;
        }
    }
}
