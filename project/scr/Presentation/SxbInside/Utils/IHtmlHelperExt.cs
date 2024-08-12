using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class IHtmlHelperExt
    {
        public static HtmlString HtmlString(this IHtmlHelper htmlHelper, string html)
        {
            return new HtmlString(html);
        }

    }
}
