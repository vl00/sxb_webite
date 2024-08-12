using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ProductManagement.Framework.Foundation
{
    public static class DoubleUtil
    {
        public static string ToPerYearFee(this double? fee, string def = "")
        {
            if (fee == null)
            {
                return def;
            }

            var bas = 10000;

            if (fee < bas)
            {
                return $"{fee}元/年";
            }
            var parseFee = Math.Round(fee.Value / bas, 2);
            return $"{parseFee}万元/年";
        }

        public static string ToDistance(this double? distance, string def = "")
        {
            if (distance == null)
            {
                return def;
            }
            
            return Math.Round((decimal)(distance / 1000), 2) + "km";
        }
    }
}
