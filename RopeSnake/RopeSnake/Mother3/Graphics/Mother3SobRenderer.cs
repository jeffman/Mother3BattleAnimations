using RopeSnake.Gba.Serialization;
using RopeSnake.Graphics;
using RopeSnake.Helpers;
using RopeSnake.Model;
using RopeSnake.Mother3.Model;
using RopeSnake.Mother3.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace RopeSnake.Mother3.Graphics
{
    public class Mother3SobRenderer
    {
        public IEnumerable<RenderSequenceStep> RenderAnimation(Mother3Sob sob, ITileset tileset, IPalette palette, int animationIndex)
        {
            sob.ThrowIfNull(nameof(sob));
            tileset.ThrowIfNull(nameof(tileset));
            palette.ThrowIfNull(nameof(palette));

            var animation = sob.Animations[animationIndex];

            int stepIndex = 0;
            foreach (var step in animation.Steps)
            {
                var spriteGroup = sob.SpriteEntries[step.SpriteGroupIndex].Sprites;
                yield return new RenderSequenceStep(renderTarget =>
                {
                    var renderer = new GraphicsRenderer(renderTarget, new RenderOptions
                    {
                        TransparentHandling = TransparentHandling.DrawNothing
                    });
                    renderer.RenderSpriteGroup(spriteGroup, tileset, palette, new System.Drawing.Point(renderTarget.Width / 2, renderTarget.Height / 2));
                }, stepIndex);
                stepIndex++;
            }
        }
    }
}
