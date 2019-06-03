using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Linq;
using RopeSnake.Model;
using RopeSnake.Graphics;
using RopeSnake.Gba.Serialization;
using RopeSnake.Mother3.Model;
using RopeSnake.Helpers;
using RopeSnake.Mother3.Serialization;

namespace RopeSnake.Mother3.Graphics
{
    public class Mother3EfcRenderer
    {
        public Mother3Reader Reader { get; }
        public GbaReader GbaReader => Reader.Reader;
        public Mother3Efc Efc { get; }
        public SizedOffsetTable Sar { get; }

        public Mother3EfcRenderer(Mother3Reader reader, Mother3Efc efc, SizedOffsetTable sar)
        {
            reader.ThrowIfNull(nameof(reader));
            efc.ThrowIfNull(nameof(efc));
            sar.ThrowIfNull(nameof(sar));
            
            Reader = reader;
            Efc = efc;
            Sar = sar;
        }

        public IEnumerable<RenderSequenceStep> RenderFrameAnimation(int animationIndex, int counterBase = 0)
        {
            var animation = Efc.FrameAnimations[animationIndex];
            var tileset = GetTileset(animation.TilesetIndex);
            var palette = GetPalette(animation.PaletteIndex, false);

            foreach (var step in animation.Steps)
            {
                var tilemapOffset = Sar.GetOffset(step.TilemapIndex);
                var tilemap = Reader.ReadBg(tilemapOffset);

                yield return new RenderSequenceStep(renderTarget =>
                {
                    var renderer = new GraphicsRenderer(renderTarget, new RenderOptions
                    {
                        TransparentHandling = TransparentHandling.DrawSolid
                    });
                    renderer.RenderTilemap(tilemap.Tilemap, tileset, palette);
                }, counterBase);

                counterBase++;
            }
        }

        public IEnumerable<RenderSequenceStep> RenderFrameAnimationSequence(int sequenceIndex)
        {
            var animationSequence = Efc.FrameAnimationSequences[sequenceIndex];
            var resolution = ResolveFrameAnimationSequence(animationSequence.Entries);

            var gfx = resolution
                .Where(r => r.animation != null)
                .Select(r => r.animation)
                .Distinct()
                .ToDictionary(a => a, a => new
                {
                    Tileset = GetTileset(a.TilesetIndex),
                    Palette = GetPalette(a.PaletteIndex, false)
                });

            int index = 0;
            foreach (var globalIndex in resolution)
            {
                var animation = globalIndex.animation;
                if (animation != null)
                {
                    int frameIndex = globalIndex.frameIndex;
                    var tileset = gfx[animation].Tileset;
                    var palette = gfx[animation].Palette;

                    var step = animation.Steps[frameIndex];
                    var tilemapOffset = Sar.GetOffset(step.TilemapIndex);
                    var tilemap = Reader.ReadBg(tilemapOffset);

                    yield return new RenderSequenceStep(renderTarget =>
                    {
                        var renderer = new GraphicsRenderer(renderTarget, new RenderOptions
                        {
                            TransparentHandling = TransparentHandling.DrawSolid
                        });
                        renderer.RenderTilemap(tilemap.Tilemap, tileset, palette);
                    }, index);
                }
                index++;
            }
        }

        public IEnumerable<IEnumerable<RenderSequenceStep>> RenderSpriteSequence(int sequenceIndex)
        {
            var header = Efc.SpriteSequenceHeaders[sequenceIndex];
            var sob = GetSob(header.SobIndex);
            var tileset = GetTileset(header.TilesetIndex);
            var palette = GetPalette(header.PaletteIndex, true);

            for (int i = 0; i < sob.Animations.Length; i++)
            {
                yield return new Mother3SobRenderer().RenderAnimation(sob, tileset, palette, i);
            }
        }

        private Tileset GetTileset(int tilesetIndex)
        {
            int tilesetOffset = Sar.GetOffset(tilesetIndex);
            int tileCount = Sar.GetSize(tilesetIndex) / 32;
            return GbaReader.ReadTileset(tilesetOffset, tileCount, 4);
        }

        private Palette GetPalette(int paletteIndex, bool padWithTransparent)
        {
            int paletteOffset = Sar.GetOffset(paletteIndex);
            int subPaletteCount = Sar.GetSize(paletteIndex) / 32;
            var palette = GbaReader.ReadPalette(paletteOffset, subPaletteCount, 16);

            if (padWithTransparent)
            {
                var newPalette = new Palette(16, palette.ColorsPerSubPalette);
                for (int i = 0; i < palette.SubPaletteCount * palette.ColorsPerSubPalette; i++)
                {
                    newPalette[i] = palette[i];
                }
                palette = newPalette;
            }

            return palette;
        }

        private Mother3Sob GetSob(int sobIndex)
        {
            var sobOffset = Sar.GetOffset(sobIndex);
            return Reader.ReadSob(sobOffset);
        }

        private (Mother3Efc.FrameAnimation animation, int frameIndex)[]
            ResolveFrameAnimationSequence(IEnumerable<Mother3Efc.FrameAnimationSequenceEntry> entries)
        {
            // Resolution rules:
            // - The total duration of any frame is (frameStep.Duration + 1)
            // - When a new frame step begins, it overwrites an existing frame step that started on a previous frame
            // - If more than one frame step begin on the same frame, the latest one wins

            var animationEntries = entries.OfType<Mother3Efc.FrameAnimationEntry>().ToArray();

            if (animationEntries.Length == 0)
            {
                return new (Mother3Efc.FrameAnimation animation, int frameIndex)[0];
            }

            // First, derive a global frame sequence
            int globalFrameCount = animationEntries.Max(e =>
            {
                var animation = Efc.FrameAnimations[e.AnimationIndex];
                return animation.Steps.Sum(s => s.Duration + 1) + e.GlobalFrameIndex;
            });

            var globalSequence = new (Mother3Efc.FrameAnimation animation, int frameIndex)[globalFrameCount];

            // For each animation sequence, build an index of (global frame index that frame step begins on, frame step)
            var globalIndex = animationEntries.Select(e =>
            {
                var animation = Efc.FrameAnimations[e.AnimationIndex];
                int globalAnimationIndex = e.GlobalFrameIndex;
                var globalIndexLookup = new Dictionary<int, (Mother3Efc.FrameAnimation animation, int step)>();

                int i = 0;
                foreach (var step in animation.Steps)
                {
                    globalIndexLookup[globalAnimationIndex] = (animation, i++);
                    globalAnimationIndex += step.Duration + 1;
                }

                return globalIndexLookup;
            }).ToArray();

            // Then, for each global frame, see which one wins
            (Mother3Efc.FrameAnimation animation, int step) currentTuple = default;
            int framesLeft = 0;

            for (int i = 0; i < globalFrameCount; i++)
            {
                var lastStep = globalIndex.LastOrDefault(d => d.ContainsKey(i));
                if (lastStep != null)
                {
                    // At least one frame step begins on this global frame index; the last one in sequence wins
                    var tuple = lastStep[i];
                    globalSequence[i] = (tuple.animation, tuple.step);
                    currentTuple = (tuple.animation, tuple.step);
                    framesLeft = tuple.animation.Steps[tuple.step].Duration + 1;
                }
                else
                {
                    if (framesLeft > 0)
                    {
                        globalSequence[i] = currentTuple;
                        framesLeft--;
                    }
                }
            }

            return globalSequence;
        }
    }
}
