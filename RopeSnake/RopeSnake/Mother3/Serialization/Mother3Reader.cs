using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using RopeSnake.Model;
using RopeSnake.Graphics;
using RopeSnake.Gba.Graphics;
using RopeSnake.Gba.Serialization;
using RopeSnake.Helpers;
using RopeSnake.Mother3.Model;

namespace RopeSnake.Mother3.Serialization
{
    public class Mother3Reader
    {
        internal const uint SobHeader = 0x20626F73;
        internal const uint SobFooter = 0x626F737E;
        internal const uint SarHeader = 0x20726173;
        internal const uint SarFooter = 0x7261737E;
        internal const uint EfcHeader = 0x20636665;
        internal const uint EfcFooter = 0x6366657E;
        internal const uint BgHeader = 0x20206762;
        internal const uint BgFooter = 0x2067627E;

        public GbaReader Reader { get; }

        public Mother3Reader(GbaReader reader)
        {
            reader.ThrowIfNull(nameof(reader));

            Reader = reader;
        }

        public Mother3Sob.SpriteEntry ReadSobSpriteEntry(int offset)
        {
            int preambleLength = Reader.ReadUShort(offset);
            var preamble = Reader.ReadBytes(offset + 2, preambleLength * 8);
            offset += 2 + (preambleLength * 8);

            int spriteCount = Reader.ReadUShort(offset);
            offset += 2;

            var sprites = new GbaSprite[spriteCount];

            for (int i = 0; i < spriteCount;i++)
            {
                sprites[i] = Reader.ReadSprite(offset);
                offset += 8;
            }

            return new Mother3Sob.SpriteEntry(preamble, new SpriteGroup(sprites));
        }

        public Mother3Sob.AnimationStep ReadSobAnimationStep(int offset)
        {
            return new Mother3Sob.AnimationStep(Reader.ReadUShort(offset), Reader.ReadUShort(offset + 2));
        }

        public Mother3Sob.Animation ReadSobAnimation(int offset)
        {
            ushort header = Reader.ReadUShort(offset);
            int count = Reader.ReadUShort(offset + 2);

            offset += 4;

            var steps = new Mother3Sob.AnimationStep[count];

            for (int i = 0; i < count; i++)
            {
                steps[i] = ReadSobAnimationStep(offset);
                offset += 4;
            }

            return new Mother3Sob.Animation(header, steps);
        }

        public Mother3Sob ReadSob(int offset)
        {
            int startOffset = offset;

            uint header = Reader.ReadUInt(offset);
            if (header != SobHeader)
                throw new InvalidDataException($"Bad sob header at 0x{offset:X}: 0x{header:X}");

            int spriteEntryCount = Reader.ReadUShort(offset + 4);
            int animationCount = Reader.ReadUShort(offset + 6);

            var spriteEntries = new Mother3Sob.SpriteEntry[spriteEntryCount];
            var animations = new Mother3Sob.Animation[animationCount];

            offset += 8;

            for (int i = 0; i < spriteEntryCount; i++)
            {
                int spriteEntryOffset = Reader.ReadUShort(offset) + startOffset;
                offset += 2;

                spriteEntries[i] = ReadSobSpriteEntry(spriteEntryOffset);
            }

            for (int i = 0; i < animationCount; i++)
            {
                int animationOffset = Reader.ReadUShort(offset) + startOffset;
                offset += 2;

                animations[i] = ReadSobAnimation(animationOffset);
            }

            return new Mother3Sob(spriteEntries, animations);
        }

        public SizedOffsetTable ReadSar(int offset)
        {
            int startOffset = offset;

            uint header = Reader.ReadUInt(offset);
            if (header != SarHeader)
                throw new InvalidDataException($"Bad sar header at 0x{offset:X}: 0x{header:X}");

            int entryCount = Reader.ReadInt(offset + 4);

            var offsets = new int[entryCount];
            var sizes = new int[entryCount];

            offset += 8;

            for (int i = 0; i < entryCount; i++)
            {
                offsets[i] = Reader.ReadInt(offset) + startOffset;
                sizes[i] = Reader.ReadInt(offset + 4);
                offset += 8;
            }

            return new SizedOffsetTable(offsets, sizes);
        }

        public Mother3Efc.FrameStep ReadFrameStep(int offset)
        {
            ushort tilemapIndex = Reader.ReadUShort(offset);
            ushort duration = Reader.ReadUShort(offset + 2);
            return new Mother3Efc.FrameStep(tilemapIndex, duration);
        }

        public Mother3Efc.FrameAnimation ReadFrameSequence(ref int offset)
        {
            ushort tilesetIndex = Reader.ReadUShort(offset);
            ushort paletteIndex = Reader.ReadUShort(offset + 2);
            int stepCount = Reader.ReadInt(offset + 4);

            var steps = new Mother3Efc.FrameStep[stepCount];

            offset += 8;

            for (int i = 0; i < stepCount; i++)
            {
                steps[i] = ReadFrameStep(offset);
                offset += 4;
            }

            return new Mother3Efc.FrameAnimation(tilesetIndex, paletteIndex, steps);
        }

        public Mother3Efc.SpriteSequenceHeader ReadSpriteSequenceHeader(int offset)
        {
            ushort sobIndex = Reader.ReadUShort(offset);
            ushort tilesetIndex = Reader.ReadUShort(offset + 2);
            ushort paletteIndex = Reader.ReadUShort(offset + 4);
            ushort unknown = Reader.ReadUShort(offset + 6);
            return new Mother3Efc.SpriteSequenceHeader(sobIndex, tilesetIndex, paletteIndex, unknown);
        }

        public Mother3Efc.FrameAnimationSequenceEntry ReadFrameAnimationSequenceEntry(ref int offset)
        {
            ushort length = Reader.ReadUShort(offset);
            var type = (Mother3Efc.FrameAnimationSequenceEntryType)Reader.ReadUShort(offset + 2);
            byte[] content = Reader.ReadBytes(offset + 4, length - 4);
            offset += length;

            var contentReader = content.ToReader();

            switch (type)
            {
                case Mother3Efc.FrameAnimationSequenceEntryType.FrameAnimation:
                    return new Mother3Efc.FrameAnimationEntry(contentReader.ReadUShort(4), contentReader.ReadUShort(0), content);

                default:
                    return new Mother3Efc.FrameAnimationSequenceEntry(type, content);
            }
        }

        public Mother3Efc.FrameAnimationSequence ReadFrameAnimationSequence(ref int offset)
        {
            int startOffset = offset;

            byte[] unknownA = Reader.ReadBytes(offset, 10);
            ushort entryCount = Reader.ReadUShort(offset + 10);
            int unknownB = Reader.ReadInt(offset + 12);

            var entries = new Mother3Efc.FrameAnimationSequenceEntry[entryCount];
            offset += 16;

            for (int i = 0; i < entryCount; i++)
            {
                entries[i] = ReadFrameAnimationSequenceEntry(ref offset);
            }

            return new Mother3Efc.FrameAnimationSequence(entries, unknownA, unknownB, startOffset);
        }

        public Mother3Efc ReadEfc(int offset)
        {
            uint header = Reader.ReadUInt(offset);
            if (header != EfcHeader)
                throw new InvalidDataException($"Bad efc header at 0x{offset:X}: 0x{header:X}");

            ushort frameAnimationCount = Reader.ReadUShort(offset + 4);
            ushort spriteSequenceCount = Reader.ReadUShort(offset + 6);
            ushort frameAnimationSequenceCount = Reader.ReadUShort(offset + 8);

            var frameAnimations = new Mother3Efc.FrameAnimation[frameAnimationCount];
            var spriteSequenceHeaders = new Mother3Efc.SpriteSequenceHeader[spriteSequenceCount];
            var frameAnimationSequences = new Mother3Efc.FrameAnimationSequence[frameAnimationSequenceCount];

            offset += 12;

            for (int i = 0; i < frameAnimationCount; i++)
            {
                frameAnimations[i] = ReadFrameSequence(ref offset);
            }

            for (int i = 0; i < spriteSequenceCount; i++)
            {
                spriteSequenceHeaders[i] = ReadSpriteSequenceHeader(offset);
                offset += 8;
            }

            for (int i = 0; i < frameAnimationSequenceCount; i++)
            {
                frameAnimationSequences[i] = ReadFrameAnimationSequence(ref offset);
            }

            return new Mother3Efc(frameAnimations, spriteSequenceHeaders, frameAnimationSequences);
        }

        public Mother3Bg ReadBg(int offset)
        {
            uint header = Reader.ReadUInt(offset);
            if (header != BgHeader)
                throw new InvalidDataException($"Bad bg header at 0x{offset:X}: 0x{header:X}");

            uint unknownA = Reader.ReadUInt(offset + 4);
            uint unknownB = Reader.ReadUInt(offset + 8);

            offset += 12;

            var tilemap = Reader.FromLz77Compressed(offset).ReadTilemap(32, 32);

            return new Mother3Bg(tilemap, unknownA, unknownB);
        }
    }
}
