using RopeSnake.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace RopeSnake.Graphics
{
    public class RenderSequenceStep
    {
        public RenderInvoker Render { get; }
        public int SequenceIndex { get; }

        public RenderSequenceStep(RenderInvoker render, int sequenceIndex)
        {
            render.ThrowIfNull(nameof(render));

            Render = render;
            SequenceIndex = sequenceIndex;
        }
    }
}
