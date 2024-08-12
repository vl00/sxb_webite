using Microsoft.AspNetCore.Mvc.Filters;
using Sxb.PCWeb.Common;
using Sxb.PCWeb.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sxb.PCWeb.Filters
{
    public class CityChangeFilterAttribute : ActionFilterAttribute
    {
        const string cityKey = "city";
        const string localcityKey = "localcity";
        const string longitudeKey = "longitude";

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.Request.IsAjax())
            {
                return;
            }

            var cityCode = context.HttpContext.Request.Query[cityKey];
            if (string.IsNullOrEmpty(cityCode))
            {
                cityCode = context.HttpContext.Request.Cookies[localcityKey];
            }

            int? code = int.TryParse(cityCode, out int result) ? result :
                string.IsNullOrEmpty(context.HttpContext.Request.Cookies[longitudeKey]) ? (int?)null :
                context.HttpContext.Request.GetLocalCity();

            if (code != null)
            {
                context.HttpContext.Response.Cookies.Append(localcityKey, code.ToString(), new Microsoft.AspNetCore.Http.CookieOptions()
                {
                    Path = "/"
                });
            }

            base.OnActionExecuted(context);
        }

    }
}
