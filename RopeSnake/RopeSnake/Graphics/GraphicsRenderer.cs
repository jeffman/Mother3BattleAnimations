using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using RopeSnake.Helpers;

namespace RopeSnake.Graphics
{
    public sealed class GraphicsRenderer
    {
        public IRenderTarget Target { get; }
        public RenderOptions Options { get; set; }

        public GraphicsRenderer(IRenderTarget target, RenderOptions options)
        {
            target.ThrowIfNull(nameof(target));

            Target = target;
            Options = options ?? new RenderOptions();
        }

        public void RenderTilemap(ITilemap tilemap, ITileset tileset, IPalette palette)
            => RenderTilemap(tilemap, tileset, palette, new Point(0, 0));

        public void RenderTilemap(ITilemap tilemap, ITileset tileset, IPalette palette, Point renderAt)
        {
            for (int y = 0; y < tilemap.HeightInTiles; y++)
            {
                for (int x = 0; x < tilemap.WidthInTiles; x++)
                {
                    var properties = tilemap[x, y];
                    RenderTile(tileset[properties.TileIndex], palette, properties, renderAt, x * 8, y * 8);
                }
            }
        }

        public void RenderSprite(ISprite sprite, ITileset tileset, IPalette palette, Point renderAt)
        {
            for (int y = 0; y < sprite.HeightInTiles; y++)
            {
                for (int x = 0; x < sprite.WidthInTiles; x++)
                {
                    var properties = new TileProperties(sprite.GetTileIndexAt(x, y), sprite.PaletteIndex, sprite.FlipX, sprite.FlipY);
                    RenderTile(tileset[properties.TileIndex], palette, properties, renderAt, sprite.X + x * 8, sprite.Y + y * 8);
                }
            }
        }

        public void RenderSpriteGroup(SpriteGroup group, ITileset tileset, IPalette palette, Point renderAt)
        {
            foreach (var sprite in group)
            {
                RenderSprite(sprite, tileset, palette, renderAt);
            }
        }

        private void RenderTile(
            Tile tile,
            IPalette palette,
            TileProperties properties,
            Point renderAt,
            int offsetX,
            int offsetY)
            => RenderTile(tile, palette, properties, new Point(renderAt.X + offsetX, renderAt.Y + offsetY));

        private unsafe void RenderTile(
            Tile tile,
            IPalette palette,
            TileProperties properties,
            Point renderAt)
        {
            int minX = Math.Max(0, renderAt.X) - renderAt.X;
            int maxX = Math.Min(tile.Width, Target.Width - renderAt.X);

            int minY = Math.Max(0, renderAt.Y) - renderAt.Y;
            int maxY = Math.Min(tile.Height, Target.Height - renderAt.Y);

            for (int y = minY; y < maxY; y++)
            {
                for (int x = minX; x < maxX; x++)
                {
                    int tileX = properties.FlipX ? tile.Width - x - 1 : x;
                    int tileY = properties.FlipY ? tile.Height - y - 1 : y;

                    int pixelIndex = tile[tileX, tileY];

                    if (pixelIndex != 0 || Options.TransparentHandling == TransparentHandling.DrawSolid)
                    {
                        Target.SetPixel(x + renderAt.X, y + renderAt.Y, palette[properties.PaletteIndex, pixelIndex]);
                    }
                    else if (Options.TransparentHandling == TransparentHandling.DrawTransparent)
                    {
                        Target.SetPixel(x + renderAt.X, y + renderAt.Y, Color.Transparent);
                    }
                }
            }
        }
    }
}
