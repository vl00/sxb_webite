using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle
{
    public class UtilsHelper
    {
        internal static string GetSubscribeCallBackHandlerUrl(SubscribeCallBackQueryRequest request, HttpRequest httpRequest)
        {
            string scheme = httpRequest.Scheme;
            if (httpRequest.Headers.TryGetValue("X-Forwarded-Proto", out StringValues proto))
            {
                scheme = proto.ToString();
            };
            string host = httpRequest.Host.ToString();
            return $"{scheme}://{host}/Common/WeChat/SubscribeCallBack?{request.ToQueryString()}";
        }
    }
}
