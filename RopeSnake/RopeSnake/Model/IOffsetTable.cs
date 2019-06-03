using System;
using System.Collections.Generic;
using System.Text;

namespace RopeSnake.Model
{
    public interface IOffsetTable
    {
        int Count { get; }
        int GetOffset(int index);
    }
}
