using System;
using Microsoft.AspNetCore.Http;

namespace Sxb.Inside.Utils
{
    public static class AjaxMethod
    {
        public static bool IsAjax(this HttpRequest req)
        {
            bool result = false;

            var xreq = req.Headers.ContainsKey("x-requested-with");
            if (xreq)
            {
                result = req.Headers["x-requested-with"] == "XMLHttpRequest";
            }

            return result; 
        }
    }
}
