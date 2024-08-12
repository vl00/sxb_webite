using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductManagement.Framework.Cache.Redis;

namespace Sxb.Web.Middleware
{
    public class WeixinAutoLoginMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly ILogger _logger;

        public WeixinAutoLoginMiddleware(RequestDelegate next, IConfiguration config, IEasyRedisClient easyRedisClient, ILogger<WeixinAutoLoginMiddleware> logger)
        {
            _configuration = config;
            _easyRedisClient = easyRedisClient;
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            //禁止自动登陆
            if (!httpContext.Request.Cookies.ContainsKey("NotAllowAutoLogin"))
            {

                var endpoint = httpContext.GetEndpoint();
                var routes = httpContext.GetRouteData()?.Values ?? null;

                bool isHomeIndex = false;
                if (routes != null && routes.Count() > 0)
                {
                    var controller = routes["controller"]?.ToString();
                    var action = routes["action"]?.ToString();
                    isHomeIndex = controller.ToLower() == "home" && action.ToLower() == "index";
                }

                //_logger.LogError($"url:{httpContext.Request.Path},enpoint:{endpoint == null},metadata:{endpoint?.Metadata == null}");
                //静态页面或者需要授权的action需要走授权登录校验 或首页
                if (endpoint == null || endpoint.Metadata?.GetMetadata<AuthorizeAttribute>() != null || isHomeIndex)
                {
                    bool isAjax = httpContext.Request.Headers["X-Requested-With"].ToString() == "XMLHttpRequest";
                    bool isWeixin = httpContext.Request.Headers["User-Agent"].ToString().ToLower().Contains("micromessenger");

                    bool isJsonp = httpContext.Request.QueryString.Value.Contains("callback");

                    bool isWeixinSDKAPI = httpContext.Request.Path.Value.Contains("/WeChatSDK", StringComparison.CurrentCultureIgnoreCase);

                    if (!(isAjax || isJsonp || isWeixinSDKAPI) && isWeixin)
                    {
                        if (!httpContext.User.Identity.IsAuthenticated)
                        {
                            var redirectUrl = httpContext.Request.Scheme + "://" + httpContext.Request.Host + httpContext.Request.Path + httpContext.Request.QueryString;
                            string state = Guid.NewGuid().ToString("N");
                            await _easyRedisClient.AddAsync("wx_redirect-" + state, redirectUrl, new TimeSpan(0, 0, 0, 600));
                            string a = await _easyRedisClient.GetAsync<string>("wx_redirect-" + state);
                            httpContext.Response.Redirect(GetLoginURL(redirectUrl, state: state));
                            httpContext.Response.StatusCode = 302;
                            return;
                        }
                    }
                }
            }
            await _next(httpContext);
        }
        private string GetLoginURL(string url, string appid = null, string scope = "snsapi_base", string state = null)
        {
            string UserServerUrl = _configuration.GetSection("UserSystemConfig")?.GetSection("ServerUrl")?.Value;
            string DefaultWxAppId = _configuration.GetSection("Wx")?.GetSection("AppId")?.Value;
            string redirect_uri_wx = $"{UserServerUrl}/ApiLogin/WXAuth?ReturnUrl=" + Uri.EscapeDataString(url);
            return
                "https://open.weixin.qq.com/connect/oauth2/authorize?" +
                "appid=" + (appid ?? DefaultWxAppId) +
                "&redirect_uri="
                + Uri.EscapeDataString("https://weixin.sxkid.com/LoginAuth.aspx?redirect_uri="
                + Uri.EscapeDataString(redirect_uri_wx))
                + "&response_type=code&scope=" + scope + "&state=" + state + "#wechat_redirect";
        }
    }


    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class WeixinAutoLoginMiddlewareExtensions
    {
        public static IApplicationBuilder UseWeixinAutoLoginMiddleware(this IApplicationBuilder builder)
        {

            return builder.UseMiddleware<WeixinAutoLoginMiddleware>();
        }
    }
}
