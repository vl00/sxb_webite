using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.Utils
{
    public class NumberParse
    {

        public static double DoubleParse(string s, double _default)
        {
            if (double.TryParse(s, out double result))
            {
                return result;
            }
            else
            {
                return _default;
            }

        }
    }
}
