using System;
using System.Collections.Generic;
using System.Text;

namespace RopeSnake.Model
{
    public interface ISizedOffsetTable : IOffsetTable
    {
        int GetSize(int index);
    }
}
