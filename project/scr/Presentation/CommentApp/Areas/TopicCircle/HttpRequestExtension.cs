using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.TopicCircle
{
    public static class HttpRequestExtension
    {
        /// <summary>
        /// 从form或者query中取出指定key的参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Param(this HttpRequest request,string key)
        {
            try
            {
                StringValues value;
                if (request.Query.TryGetValue(key, out value))
                {
                    return value;
                }
                if (request.Form.TryGetValue(key, out value))
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
