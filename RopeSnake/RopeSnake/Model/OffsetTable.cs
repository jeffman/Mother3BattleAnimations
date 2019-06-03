using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using RopeSnake.Helpers;

namespace RopeSnake.Model
{
    public class OffsetTable : IOffsetTable
    {
        private int[] _offsets;

        public virtual int Count => _offsets.Length;

        public OffsetTable(IEnumerable<int> offsets)
        {
            offsets.ThrowIfNull(nameof(offsets));
            _offsets = offsets.ToArray();
        }

        public virtual int GetOffset(int index) => _offsets[index];
    }
}
