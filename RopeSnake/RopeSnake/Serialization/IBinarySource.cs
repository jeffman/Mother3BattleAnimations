using System;
using System.Collections.Generic;
using System.Text;

namespace RopeSnake.Serialization
{
    public interface IBinarySource
    {
        /// <summary>
        /// Gets or sets a byte at a given offset.
        /// </summary>
        /// <param name="offset">offset</param>
        /// <returns>value</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if <c>offset</c> is negative, or equal to or greater than <c>Length</c>.</exception>
        byte this[int offset] { get; set; }

        /// <summary>
        /// Returns the length of this binary source, in bytes.
        /// </summary>
        int Length { get; }
    }
}
