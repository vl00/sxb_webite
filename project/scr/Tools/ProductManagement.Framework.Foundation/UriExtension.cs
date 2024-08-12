using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    public static class UriExtension
    {
        public static StringValues QueryValues(this Uri uri,string key)
        {
            Regex regex = new Regex($@"(?<=(\?|&|){key}=)[^&=#]+");
            var mathches = regex.Matches(uri.Query);
            string[] values = new string[mathches.Count];
            for (int i = 0; i < mathches.Count; i++)
            {
                values[i] = mathches[i].Value;

            }
            return new StringValues(values);

        }
    }
}
