using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using NLog;
using ProductManagement.Framework.Foundation;
using Sxb.PCWeb.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sxb.PCWeb.Middleware
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggerMiddleware> _logger;
        private IHttpContextAccessor _accessor;


        public LoggerMiddleware(RequestDelegate next,
            IHttpContextAccessor accessor, ILoggerFactory loggerFactory)
        {
            _next = next;
            _accessor = accessor;
            _logger = loggerFactory.CreateLogger<LoggerMiddleware>();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Logger log = LogManager.GetCurrentClassLogger();

            //记录用户访问日志
            if (log.IsInfoEnabled)
            {
                var routes = httpContext.GetRouteData()?.Values ?? null;

                if (routes != null && routes.Count() > 1)
                {
                    var controller = routes["controller"]?.ToString();
                    var action = routes["action"]?.ToString();
                    var id = routes["id"]?.ToString();

                    var actionDesc = ActionExtension.Description(controller, action);

                    string path = "/" + controller + "/" + action;

                    var queryDic = httpContext.Request.Query.ToDictionary(kv => kv.Key, kv =>
                    {
                        var v = kv.Value.ToString();
                        if (Guid.TryParse(v, out var gid)) return gid.ToString("n");
                        return v;
                    });
                    if (!string.IsNullOrWhiteSpace(id) && !queryDic.ContainsKey("id"))
                    {
                        queryDic.Add("id", id);
                    }
                    var queryString = string.Join("&", queryDic.Select(q => q.Key + "=" + q.Value));

                    var cookie = httpContext.Request.Cookies;
                    var header = httpContext.Request.Headers;

                    Dictionary<string, string> headerDic = new Dictionary<string, string>();
                    foreach (var h in header)
                    {
                        headerDic.Add(h.Key, h.Value);
                    }

                    var platformMode = UserAgentUtils.GetPlatformMode(headerDic);

                    int platform = platformMode & 0x100;
                    int system = platformMode & 0x010;
                    int client = platformMode & 0x001;


                    //IP
                    string ipString = _accessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    //var ipString = IPUtil.GetIpAddr(httpContext.Request);

                    Regex regEx = new Regex(@"(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)\.(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)\.(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)\.(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)");
                    long ip = IPConvertUnit.Ip2Long("127.0.0.1");
                    if (regEx.IsMatch(ipString))
                    {
                        ip = IPConvertUnit.Ip2Long(ipString);
                    }

                    //用户ID
                    Guid? userId = null;
                    if (httpContext.User.Identity.IsAuthenticated)
                    {
                        var userInfo = httpContext.User.Identity.GetUserInfo();
                        userId = userInfo.UserId;
                    }

                    //设备ID
                    string deviceId = null;
                    if (cookie.ContainsKey("uuid"))
                    {
                        deviceId = cookie["uuid"];
                    }

                    //渠道
                    string fw = null;
                    if (cookie.ContainsKey("fw"))
                    {
                        fw = cookie["fw"];
                    }
                    else if (queryDic.ContainsKey("fw"))
                    {
                        fw = queryDic["fw"];
                    }

                    string fx = null;
                    if (cookie.ContainsKey("fx"))
                    {
                        fw = cookie["fx"];
                    }
                    else if (queryDic.ContainsKey("fx"))
                    {
                        fw = queryDic["fx"];
                    }

                    string adcode = httpContext.Request.GetCity("0").ToString();

                    string sessionid = null;
                    if (cookie.ContainsKey("Sessionid"))
                    {
                        sessionid = cookie["Sessionid"];
                    }
                    else if (cookie.ContainsKey("sessionid"))
                    {
                        sessionid = cookie["sessionid"];
                    }

                    //坐标
                    var latitude = httpContext.Request.GetLatitude("0");
                    var longitude = httpContext.Request.GetLongitude("0");

                    string Params = "";
                    if (httpContext.Request.Method.ToLower() == "get")
                    {
                        Params = queryString;
                    }
                    else if (httpContext.Request.Method.ToLower() == "post" && headerDic.ContainsKey("Content-Type") && !headerDic["Content-Type"].Contains("multipart/form-data"))
                    {
                        httpContext.Request.EnableBuffering();
                        using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                        {
                            Params = await reader.ReadToEndAsync();
                        }
                        httpContext.Request.Body.Position = 0;
                    }

                    LogEventInfo ei = new LogEventInfo();
                    ei.Properties["userId"] = userId;
                    ei.Properties["deviceId"] = deviceId;
                    ei.Properties["fw"] = fw;
                    ei.Properties["fx"] = fx;
                    ei.Properties["ip"] = ip;

                    ei.Properties["latitude"] = latitude;
                    ei.Properties["longitude"] = longitude;

                    ei.Properties["path"] = path;
                    ei.Properties["queryString"] = queryString;
                    ei.Properties["actionName"] = actionDesc;

                    ei.Properties["platform"] = platform == 0x100 ? 1 : 2;
                    ei.Properties["system"] = system == 0x100 ? 0 : system == 0x010 ? 1 : 2;
                    ei.Properties["client"] = client == 0x100 ? 0 : client == 0x001 ? 1 : client == 0x002 ? 2 : client == 0x003 ? 3 : 0;

                    ei.Properties["adcode"] = adcode;
                    ei.Properties["sessionid"] = sessionid;
                    ei.Properties["params"] = Params;

                    ei.Level = NLog.LogLevel.Info;

                    log.Info(ei);
                }
            }
            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class LoggerMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggerMiddleware>();
        }
    }
}
