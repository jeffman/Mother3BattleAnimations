using System;
using System.Collections.Generic;
using System.Text;

namespace RopeSnake.Graphics
{
    /// <summary>
    /// A <c>Palette</c> logically represents a collection of colors.
    /// 
    /// This implementation allows the collection to be represented as either 1D or 2D.
    /// </summary>
    public class Palette : IPalette
    {
        /// <summary>
        /// Gets or sets a color (1D).
        /// </summary>
        /// <param name="index">1D color index</param>
        /// <returns>color at the given index</returns>
        public Color this[int index]
        {
            get => _colors[index];
            set => _colors[index] = value;
        }

        /// <summary>
        /// Gets or sets a color (2D).
        /// </summary>
        /// <param name="subPalette">sub-palette index</param>
        /// <param name="index">color index within sub-palette</param>
        /// <returns>sub-palette color at the given index</returns>
        public Color this[int subPalette, int index]
        {
            get => _colors[Get1DIndex(subPalette, index)];
            set => _colors[Get1DIndex(subPalette, index)] = value;
        }

        /// <summary>
        /// Returns the number of sub-palettes (2D).
        /// </summary>
        public int SubPaletteCount { get; }

        /// <summary>
        /// Returns the number of colors per sub-palette (2D).
        /// </summary>
        public int ColorsPerSubPalette { get; }

        /// <summary>
        /// Returns the total number of colors (1D).
        /// </summary>
        public virtual int TotalColorCount => SubPaletteCount * ColorsPerSubPalette;

        private Color[] _colors;

        public Palette(int totalColorCount) : this(1, totalColorCount) { }

        public Palette(int subPaletteCount, int colorsPerSubPalette)
        {
            if (subPaletteCount < 1 || subPaletteCount > 16)
                throw new ArgumentOutOfRangeException(nameof(subPaletteCount));

            if (colorsPerSubPalette < 1 || colorsPerSubPalette > 256)
                throw new ArgumentOutOfRangeException(nameof(colorsPerSubPalette));

            _colors = new Color[subPaletteCount * colorsPerSubPalette];
            SubPaletteCount = subPaletteCount;
            ColorsPerSubPalette = colorsPerSubPalette;
        }

        protected int Get1DIndex(int subPalette, int index) => index + subPalette * ColorsPerSubPalette;
    }
}
