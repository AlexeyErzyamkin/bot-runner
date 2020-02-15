using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public static class Ensures
    {
        public static bool Ensure(this bool self)
        {
            if (!self)
            {
                throw new Exception("Value is false");
            }

            return self;
        }
    }
}
