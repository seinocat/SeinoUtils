using System;
using System.Collections.Generic;

namespace Seino.Utils
{
    public static partial class SeinoUtils
    {
        public static T GetLast<T>(this List<T> list)
        {
            return list[^1];
        }
        
    }
}