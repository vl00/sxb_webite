using System;
using System.Collections.Generic;
using System.Linq;

namespace iSchool
{
    public static partial class LinqExtension
    {
        public static bool In<T>(this T item, params T[] collection)
        {
            return collection.Contains(item);
        }
    }
}