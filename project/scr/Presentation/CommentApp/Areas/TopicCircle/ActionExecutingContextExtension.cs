using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.TopicCircle
{
    public static class ActionExecutingContextExtension
    {
        /// <summary>
        /// 从form或者query中取出指定key的参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Param(this ActionExecutingContext context, string key)
        {
            try
            {
                StringValues value;
                if (context.HttpContext.Request.Query.TryGetValue(key, out value))
                {
                    return value;
                }
                if (context.HttpContext.Request.Form.TryGetValue(key, out value))
                {
                    return value;
                }
                return value;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
