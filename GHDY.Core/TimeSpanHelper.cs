using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHDY.Core
{
    public static class TimeSpanHelper
    {
        public static bool Intersect(TimeSpan oneBegin, TimeSpan oneEnd, TimeSpan twoBegin, TimeSpan twoEnd)
        {
            double length = IntersectLength(oneBegin, oneEnd, twoBegin, twoEnd);

            if (length >= 0)
                return true;
            else
                return false;
        }

        public static double IntersectLength(TimeSpan oneBegin, TimeSpan oneEnd, TimeSpan twoBegin, TimeSpan twoEnd)
        {
            var begin = Math.Max(oneBegin.TotalSeconds, twoBegin.TotalSeconds);
            var end = Math.Min(oneEnd.TotalSeconds, twoEnd.TotalSeconds);

            var length = end - begin;
            return length;
        }
    }
}
