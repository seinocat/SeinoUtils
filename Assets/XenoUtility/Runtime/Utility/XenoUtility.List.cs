using System;
using System.Collections.Generic;

namespace Xeno.Utilities
{
    public static partial class XenoUtility
    {
        public static T GetLast<T>(this List<T> list)
        {
            return list[list.Count - 1];
        }
        
    }
}