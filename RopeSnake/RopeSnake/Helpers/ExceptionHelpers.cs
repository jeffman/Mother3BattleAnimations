using System;
using System.Collections.Generic;
using System.Text;

namespace RopeSnake.Helpers
{
    public static class ExceptionHelpers
    {
        public static void ThrowIfNull(this object obj, string name)
        {
            if (ReferenceEquals(obj, null))
                throw new ArgumentNullException(name);
        }
    }
}
