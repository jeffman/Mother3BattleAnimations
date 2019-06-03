using RopeSnake.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RopeSnake.Mother3.Model
{
    public class Mother3Efc
    {
        public class FrameAnimationSequence
        {
            public FrameAnimationSequenceEntry[] Entries { get; }
            public byte[] UnknownA { get; }
            public int UnknownB { get; }

            internal int Offset { get; }

            public FrameAnimationSequence(IEnumerable<FrameAnimationSequenceEntry> entries, byte[] unknownA, int unknownB)
                : this(entries, unknownA, unknownB, 0)
            {

            }

            internal FrameAnimationSequence(IEnumerable<FrameAnimationSequenceEntry> entries, byte[] unknownA, int unknownB, int offset)
            {
                entries.ThrowIfNull(nameof(entries));

                Entries = entries.ToArray();
                UnknownA = unknownA;
                UnknownB = unknownB;
                Offset = offset;
            }
        }

        public class FrameAnimationSequenceEntry
        {
            public FrameAnimationSequenceEntryType Type { get; }
            public byte[] Content { get; }

            public FrameAnimationSequenceEntry(FrameAnimationSequenceEntryType type, byte[] content)
            {
                content.ThrowIfNull(nameof(content));

                Type = type;
                Content = content;
            }
        }

        public class FrameAnimationEntry : FrameAnimationSequenceEntry
        {
            public int AnimationIndex { get; }
            public int GlobalFrameIndex { get; }

            public FrameAnimationEntry(int animationIndex, int globalFrameIndex, byte[] content)
                : base(FrameAnimationSequenceEntryType.FrameAnimation, content)
            {
                AnimationIndex = animationIndex;
                GlobalFrameIndex = globalFrameIndex;
            }
        }

        public class FrameAnimation
        {
            public ushort TilesetIndex { get; }
            public ushort PaletteIndex { get; }
            public FrameStep[] Steps { get; }

            public FrameAnimation(ushort tilesetIndex, ushort paletteIndex, IEnumerable<FrameStep> steps)
            {
                steps.ThrowIfNull(nameof(steps));

                TilesetIndex = tilesetIndex;
                PaletteIndex = paletteIndex;
                Steps = steps.ToArray();
            }
        }

        public class FrameStep
        {
            public ushort TilemapIndex { get; }
            public ushort Duration { get; }

            public FrameStep(ushort tilemapIndex, ushort duration)
            {
                TilemapIndex = tilemapIndex;
                Duration = duration;
            }

            public override string ToString()
            {
                return $"Tilemap index: {TilemapIndex}, duration: {Duration}";
            }
        }

        public class SpriteSequenceHeader
        {
            public ushort SobIndex { get; }
            public ushort TilesetIndex { get; }
            public ushort PaletteIndex { get; }
            public ushort Unknown { get; }

            public SpriteSequenceHeader(ushort sobIndex, ushort tilesetIndex, ushort paletteIndex, ushort unknown)
            {
                SobIndex = sobIndex;
                TilesetIndex = tilesetIndex;
                PaletteIndex = paletteIndex;
                Unknown = unknown;
            }
        }

        public FrameAnimation[] FrameAnimations { get; }
        public SpriteSequenceHeader[] SpriteSequenceHeaders { get; }
        public FrameAnimationSequence[] FrameAnimationSequences { get; }

        public Mother3Efc(
            IEnumerable<FrameAnimation> frameAnimations,
            IEnumerable<SpriteSequenceHeader> spriteSequenceHeaders,
            IEnumerable<FrameAnimationSequence> frameAnimationSequences)
        {
            frameAnimations.ThrowIfNull(nameof(frameAnimations));
            spriteSequenceHeaders.ThrowIfNull(nameof(spriteSequenceHeaders));
            frameAnimationSequences.ThrowIfNull(nameof(frameAnimationSequences));

            FrameAnimations = frameAnimations.ToArray();
            SpriteSequenceHeaders = spriteSequenceHeaders.ToArray();
            FrameAnimationSequences = frameAnimationSequences.ToArray();
        }

        public enum FrameAnimationSequenceEntryType : ushort
        {
            FrameAnimation = 0
        }
    }
}
