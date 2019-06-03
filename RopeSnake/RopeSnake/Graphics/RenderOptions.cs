using System;
using System.Collections.Generic;
using System.Text;

namespace RopeSnake.Graphics
{
    public sealed class RenderOptions
    {
        public TransparentHandling TransparentHandling { get; set; }

        public RenderOptions(TransparentHandling transparentHandling = TransparentHandling.DrawSolid)
        {
            TransparentHandling = transparentHandling;
        }
    }
}
