using System;
using System.Collections.Generic;
using System.Text;
using RopeSnake.Helpers;

namespace RopeSnake.Graphics
{
    public class Tileset : ITileset
    {
        public Tile this[int index]
        {
            get => _tiles[index];
            set => _tiles[index] = value;
        }

        public int Count => _tiles.Count;

        private List<Tile> _tiles;

        private Tileset()
        {
            _tiles = new List<Tile>();
        }

        public Tileset(IEnumerable<Tile> tiles) : this()
        {
            tiles.ThrowIfNull(nameof(tiles));
            foreach (var tile in tiles)
            {
                _tiles.Add(tile);
            }
        }
    }
}
