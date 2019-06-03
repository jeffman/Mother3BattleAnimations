using System;
using System.Collections.Generic;
using System.Text;
using RopeSnake.Graphics;
using RopeSnake.Serialization;
using RopeSnake.Gba.Graphics;

namespace RopeSnake.Gba.Serialization
{
    public class GbaReader
    {
        /// <summary>
        /// <c>BinaryReader</c> that this reader is based off.
        /// </summary>
        public BinaryReader Reader { get; }

        public GbaReader(BinaryReader baseReader)
        {
            Reader = baseReader;
        }

        #region Forwarded methods

        public byte ReadByte(int offset) => Reader.ReadByte(offset);
        public sbyte ReadSByte(int offset) => Reader.ReadSByte(offset);
        public short ReadShort(int offset) => Reader.ReadShort(offset);
        public ushort ReadUShort(int offset) => Reader.ReadUShort(offset);
        public int ReadInt(int offset) => Reader.ReadInt(offset);
        public uint ReadUInt(int offset) => Reader.ReadUInt(offset);
        public byte[] ReadBytes(int offset, int length) => Reader.ReadBytes(offset, length);

        #endregion

        /// <summary>
        /// Read an 8x8 graphics tile.
        /// </summary>
        /// <param name="offset">source offset</param>
        /// <param name="bitDepth">bits per pixel (must be 4 or 8)</param>
        /// <param name="bytesRead">return the number of bytes read</param>
        /// <returns>tile</returns>
        public Tile ReadTile(int offset, int bitDepth, out int bytesRead)
        {
            switch (bitDepth)
            {
                case 4:
                    bytesRead = 32;
                    return ReadTile4Bpp(offset);

                case 8:
                    bytesRead = 64;
                    return ReadTile8Bpp(offset);

                default:
                    throw new ArgumentException($"Invalid bit depth: {bitDepth}");
            }
        }

        /// <summary>
        /// Read an 8x8 graphics tile.
        /// </summary>
        /// <param name="offset">source offset</param>
        /// <param name="bitDepth">bits per pixel (must be 4 or 8)</param>
        /// <returns>tile</returns>
        public Tile ReadTile(int offset, int bitDepth) => ReadTile(offset, bitDepth, out _);

        /// <summary>
        /// Reads a tileset.
        /// </summary>
        /// <param name="offset">source offset</param>
        /// <param name="tileCount">number of tiles to read</param>
        /// <param name="bitDepth">bits per pixel (must be 4 of 8)</param>
        /// <returns></returns>
        public Tileset ReadTileset(int offset, int tileCount, int bitDepth)
        {
            if (tileCount < 0)
                throw new ArgumentOutOfRangeException(nameof(tileCount));

            var tiles = new Tile[tileCount];
            for (int i = 0; i < tileCount; i++)
            {
                tiles[i] = ReadTile(offset, bitDepth, out int bytesRead);
                offset += bytesRead;
            }
            return new Tileset(tiles);
        }

        /// <summary>
        /// Reads the entire underlying <c>Source</c> associated with this reader as a tileset.
        /// </summary>
        /// <param name="bitDepth"></param>
        /// <returns>tileset</returns>
        public Tileset ReadTileset(int bitDepth)
        {
            int tileCount = Reader.Source.Length / bitDepth / 8;
            if (tileCount * 8 * bitDepth != Reader.Source.Length)
                throw new System.IO.InvalidDataException("The data's length must be a multiple of bitDepth * 8");

            return ReadTileset(0, tileCount, bitDepth);
        }

        /// <summary>
        /// Reads a single 15-bit color.
        /// </summary>
        /// <param name="offset">source offset</param>
        /// <returns>color</returns>
        public Color ReadColor(int offset)
        {
            ushort rgb = Reader.ReadUShort(offset);
            byte r = (byte)((rgb & 0x1F) * 8);
            byte g = (byte)(((rgb >> 5) & 0x1F) * 8);
            byte b = (byte)(((rgb >> 10) & 0x1F) * 8);
            return new Color(r, g, b);
        }

        /// <summary>
        /// Reads a 1D palette.
        /// </summary>
        /// <param name="offset">source offset</param>
        /// <param name="totalColorCount">number of colors</param>
        /// <returns>palette</returns>
        public Palette ReadPalette(int offset, int totalColorCount) => ReadPalette(offset, 1, totalColorCount);

        /// <summary>
        /// Reads a 2D palette.
        /// </summary>
        /// <param name="offset">source offset</param>
        /// <param name="subPaletteCount">number of sub-palettes</param>
        /// <param name="colorsPerSubPalette">number of colors per sub-palette</param>
        /// <returns>palette</returns>
        public Palette ReadPalette(int offset, int subPaletteCount, int colorsPerSubPalette)
        {
            var palette = new Palette(subPaletteCount, colorsPerSubPalette);

            for (int subPaletteIndex = 0; subPaletteIndex < subPaletteCount; subPaletteIndex++)
            {
                for (int colorIndex = 0; colorIndex < colorsPerSubPalette; colorIndex++)
                {
                    palette[subPaletteIndex, colorIndex] = ReadColor(offset);
                    offset += 2;
                }
            }

            return palette;
        }

        /// <summary>
        /// Reads a 16-bit tile property chunk.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns>tile properties</returns>
        public TileProperties ReadTileProperties(int offset)
        {
            ushort properties = Reader.ReadUShort(offset);
            int tileIndex = properties & 0x3FF;
            int paletteIndex = (properties >> 12) & 0xF;
            bool flipX = (properties & 0x400) != 0;
            bool flipY = (properties & 0x800) != 0;
            return new TileProperties(tileIndex, paletteIndex, flipX, flipY);
        }

        /// <summary>
        /// Reads a <c>Tilemap</c>.
        /// </summary>
        /// <param name="offset">source offset</param>
        /// <param name="widthInTiles">width, in tiles</param>
        /// <param name="heightInTiles">height, in tiles</param>
        /// <returns>tilemap</returns>
        public Tilemap ReadTilemap(int offset, int widthInTiles, int heightInTiles)
        {
            var tilemap = new Tilemap(widthInTiles, heightInTiles);

            for (int y = 0; y < heightInTiles; y++)
            {
                for (int x = 0; x < widthInTiles; x++)
                {
                    tilemap[x, y] = ReadTileProperties(offset);
                    offset += 2;
                }
            }

            return tilemap;
        }

        /// <summary>
        /// Reads the entire underlying <c>Source</c> associated with this reader as a tilemap.
        /// </summary>
        /// <param name="widthInTiles">width, in tiles</param>
        /// <param name="heightInTiles">height, in tiles</param>
        /// <returns>tilemap</returns>
        public Tilemap ReadTilemap(int widthInTiles, int heightInTiles)
        {
            int tileCount = widthInTiles * heightInTiles;
            if (tileCount * 2 != Reader.Source.Length)
                throw new System.IO.InvalidDataException("The data's length must be a multiple of width * height * 8");

            return ReadTilemap(0, widthInTiles, heightInTiles);
        }

        /// <summary>
        /// Reads an OAM-formatted sprite.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public GbaSprite ReadSprite(int offset)
        {
            ushort chunk0 = ReadUShort(offset);
            ushort chunk1 = ReadUShort(offset + 2);
            ushort chunk2 = ReadUShort(offset + 4);

            int y = ((chunk0 & 0xFF) << 24) >> 24;
            var shape = (GbaSpriteShape)((chunk0 >> 14) & 0x3);

            int x = ((chunk1 & 0x1FF) << 23) >> 23;
            bool flipX = (chunk1 & 0x1000) != 0;
            bool flipY = (chunk1 & 0x2000) != 0;
            int sizeIndex = (chunk1 >> 14) & 0x3;

            int tileIndex = chunk2 & 0x3FF;
            int priority = (chunk2 >> 10) & 0x3;
            int paletteIndex = (chunk2 >> 12) & 0xF;

            return new GbaSprite(x, y, shape, sizeIndex, tileIndex, paletteIndex, flipX, flipY, priority);
        }

        /// <summary>
        /// Creates a new <c>GbaReader</c> from an LZ77-compressed data block.
        /// </summary>
        /// <param name="offset">source offset</param>
        /// <returns>new <c>GbaReader</c> operating on the decompressed data</returns>
        public GbaReader FromLz77Compressed(int offset)
        {
            var compressor = new Lz77Compressor(true);
            return new GbaReader(new BinaryReader(compressor.Decompress(Reader.Source, offset)));
        }

        private Tile ReadTile4Bpp(int offset)
        {
            var tile = new Tile(8, 8);

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x += 2)
                {
                    byte pair = Reader.ReadByte(offset);
                    offset++;

                    tile[x, y] = (byte)(pair & 0xF);
                    tile[x + 1, y] = (byte)((pair >> 4) & 0xF);
                }
            }

            return tile;
        }

        private Tile ReadTile8Bpp(int offset)
        {
            var tile = new Tile(8, 8);

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    tile[x, y] = Reader.ReadByte(offset);
                    offset++;
                }
            }

            return tile;
        }
    }
}
