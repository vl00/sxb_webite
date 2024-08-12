using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ProductManagement.Framework.Foundation
{
    public static class DecimalExtension
    {
        public static decimal ToStar(this decimal score)
        {
            //兼容5分制
            if (score <= 5)
            {
                score *= 20;
            }

            var star = Math.Ceiling(score / 20);
            return star;
        }

    }
}
