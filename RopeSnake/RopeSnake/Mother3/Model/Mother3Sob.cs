using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using RopeSnake.Graphics;
using RopeSnake.Helpers;

namespace RopeSnake.Mother3.Model
{
    public sealed class Mother3Sob
    {
        public class SpriteEntry
        {
            public byte[] Preamble { get; }
            public SpriteGroup Sprites { get; }

            public SpriteEntry(byte[] preamble, SpriteGroup sprites)
            {
                preamble.ThrowIfNull(nameof(preamble));
                sprites.ThrowIfNull(nameof(sprites));

                Preamble = preamble.ToArray();
                Sprites = sprites;
            }
        }

        public class AnimationStep
        {
            public ushort FrameIndex { get; }
            public ushort SpriteGroupIndex { get; }

            public AnimationStep(ushort frameIndex, ushort spriteGroupIndex)
            {
                FrameIndex = frameIndex;
                SpriteGroupIndex = spriteGroupIndex;
            }
        }

        public class Animation : IEnumerable<AnimationStep>
        {
            public ushort Header { get; }
            public AnimationStep[] Steps { get; }

            public Animation(ushort header, IEnumerable<AnimationStep> steps)
            {
                steps.ThrowIfNull(nameof(steps));

                Header = header;
                Steps = steps.ToArray();
            }

            public IEnumerator<AnimationStep> GetEnumerator()
            {
                return ((IEnumerable<AnimationStep>)Steps).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<AnimationStep>)Steps).GetEnumerator();
            }
        }

        public SpriteEntry[] SpriteEntries { get; }
        public Animation[] Animations { get; }

        public Mother3Sob(IEnumerable<SpriteEntry> spriteEntries, IEnumerable<Animation> animations)
        {
            spriteEntries.ThrowIfNull(nameof(spriteEntries));
            animations.ThrowIfNull(nameof(animations));

            SpriteEntries = spriteEntries.ToArray();
            Animations = animations.ToArray();
        }

        public RectangleR GetAnimationBounds(int animationIndex)
        {
            var bounds = new RectangleR();
            var animation = Animations[animationIndex];
            return animation.Aggregate(bounds, (b, s) => b.Union(SpriteEntries[s.SpriteGroupIndex].Sprites.GetBounds()));
        }
    }
}
