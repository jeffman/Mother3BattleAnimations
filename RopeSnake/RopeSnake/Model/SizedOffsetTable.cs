using RopeSnake.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RopeSnake.Model
{
    public class SizedOffsetTable : OffsetTable, ISizedOffsetTable
    {
        private int[] _sizes;

        public SizedOffsetTable(IEnumerable<int> offsets, IEnumerable<int> sizes)
            : base(offsets)
        {
            sizes.ThrowIfNull(nameof(sizes));

            _sizes = sizes.ToArray();
            if (_sizes.Length != Count)
                throw new ArgumentException($"The length of offsets and sizes must match");
        }

        public virtual int GetSize(int index) => _sizes[index];
    }
}
