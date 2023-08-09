using System;
using System.Collections.Generic;

namespace Xeno.Utilities
{
    public static partial class XenoUtilities
    {
        public static T GetLast<T>(this List<T> list)
        {
            return list[list.Count - 1];
        }
        
    }
}