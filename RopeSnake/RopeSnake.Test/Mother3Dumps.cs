using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using NUnit.Framework;
using RopeSnake.Serialization;
using RopeSnake.Graphics;
using RopeSnake.Gba.Serialization;
using RopeSnake.Mother3.Serialization;
using RopeSnake.Mother3.Model;
using RopeSnake.Mother3.Graphics;
using RopeSnake.Helpers;
using BinaryReader = RopeSnake.Serialization.BinaryReader;
using System;

namespace Tests
{
    public class Tests
    {
        ByteArraySource m3;

        [SetUp]
        public void Setup()
        {
            byte[] m3rom = File.ReadAllBytes(Path.Combine("Artifacts", "m3jp.gba"));
            m3 = new ByteArraySource(m3rom);
        }

        [Test, Ignore("Run this test to dump all battle animations (takes a few minutes!)")]
        public void DumpBattleAnimations()
        {
            var baseDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\..\.."));

            var frameAnimationDirectory = new DirectoryInfo(Path.Combine(baseDirectory.FullName, "frame-animations"));
            var frameAnimationSequencesDirectory = new DirectoryInfo(Path.Combine(baseDirectory.FullName, "frame-animation-sequences"));
            var spriteSequencesDirectory = new DirectoryInfo(Path.Combine(baseDirectory.FullName, "sprite-sequences"));

            if (!frameAnimationDirectory.Exists)
                frameAnimationDirectory.Create();

            if (!frameAnimationSequencesDirectory.Exists)
                frameAnimationSequencesDirectory.Create();

            if (!spriteSequencesDirectory.Exists)
                spriteSequencesDirectory.Create();

            var reader = new Mother3Reader(new GbaReader(new BinaryReader(m3)));

            var efc = reader.ReadEfc(0x1E4015C);
            var sar = reader.ReadSar(0x1E45C1C);
            var efcRenderer = new Mother3EfcRenderer(reader, efc, sar);

            // Frame animation sequences
            for (int sequenceIndex = 0; sequenceIndex < efc.FrameAnimationSequences.Length; sequenceIndex++)
            {
                foreach (var step in efcRenderer.RenderFrameAnimationSequence(sequenceIndex))
                {
                    var sequenceDirectory = new DirectoryInfo(Path.Combine(frameAnimationSequencesDirectory.FullName, sequenceIndex.ToString("D3")));
                    if (!sequenceDirectory.Exists)
                        sequenceDirectory.Create();

                    string fileName = Path.Combine(sequenceDirectory.FullName, $"{step.SequenceIndex:D3}.png");
                    RenderImage(fileName, step.Render);
                }
            }

            // Sprite sequences
            for (int sequenceIndex = 0; sequenceIndex < efc.SpriteSequenceHeaders.Length; sequenceIndex++)
            {
                var sequenceDirectory = new DirectoryInfo(Path.Combine(spriteSequencesDirectory.FullName, sequenceIndex.ToString("D3")));
                if (!sequenceDirectory.Exists)
                    sequenceDirectory.Create();

                int animationIndex = 0;
                foreach (var animation in efcRenderer.RenderSpriteSequence(sequenceIndex))
                {
                    var animationDirectory = new DirectoryInfo(Path.Combine(sequenceDirectory.FullName, animationIndex.ToString("D3")));
                    if (!animationDirectory.Exists)
                        animationDirectory.Create();

                    foreach (var animationStep in animation)
                    {
                        string fileName = Path.Combine(animationDirectory.FullName, $"{animationStep.SequenceIndex:D3}.png");
                        RenderImage(fileName, animationStep.Render);
                    }

                    animationIndex++;
                }
            }

            // Frame animations
            for (int sequenceIndex = 0; sequenceIndex < efc.FrameAnimations.Length; sequenceIndex++)
            {
                var sequenceDirectory = new DirectoryInfo(Path.Combine(frameAnimationDirectory.FullName, sequenceIndex.ToString("D3")));
                if (!sequenceDirectory.Exists)
                    sequenceDirectory.Create();

                foreach (var step in efcRenderer.RenderFrameAnimation(sequenceIndex))
                {
                    string fileName = Path.Combine(sequenceDirectory.FullName, $"{step.SequenceIndex:D3}.png");
                    RenderImage(fileName, step.Render);
                }
            }
        }

        private void RenderImage(string fileName, RenderInvoker render)
        {
            using (var bmp = new Bitmap(256, 256, PixelFormat.Format32bppArgb))
            using (var target = new BitmapRenderTarget(bmp))
            {
                target.Fill(new RopeSnake.Graphics.Color(0, 0, 0, 0));
                render(target);
                bmp.Save(fileName);
            }
        }
    }
}
