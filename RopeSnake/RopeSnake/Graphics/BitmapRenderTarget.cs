using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using RopeSnake.Helpers;

namespace RopeSnake.Graphics
{
    public class BitmapRenderTarget : IRenderTarget, IDisposable
    {
        public Bitmap Target { get; }
        public int Width { get; }
        public int Height { get; }

        private BitmapData _targetData;

        public BitmapRenderTarget(Bitmap target)
        {
            target.ThrowIfNull(nameof(target));

            if (target.PixelFormat != PixelFormat.Format32bppArgb)
                throw new FormatException("Only PixelFormat.Format32bppArgb is supported");

            Target = target;
            Width = target.Width;
            Height = target.Height;

            Lock();
        }

        public unsafe void SetPixel(int x, int y, Color color)
        {
            if (x < 0 || x >= Width)
                return;

            if (y < 0 || y >= Height)
                return;

            int* ptr = (int*)_targetData.Scan0 + x + (y * _targetData.Stride / 4);
            *ptr = color.Argb;
        }

        private void Lock()
        {
            _targetData = Target.LockBits(new System.Drawing.Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, Target.PixelFormat);
        }

        private void Unlock()
        {
            Target.UnlockBits(_targetData);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BitmapRenderTarget()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            Unlock();
        }

        public void Fill(Color color)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    SetPixel(x, y, color);
                }
            }
        }
    }
}
