using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Sxb.PCWeb.Middleware
{
    public class JudgeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JudgeMiddleware> _logger;

        public JudgeMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<JudgeMiddleware>();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                bool isMobile = httpContext.Request.Headers["User-Agent"].ToString().ToLower().Contains("mobile");
                var host = httpContext.Request.Host.Host;

                if (isMobile && (host.Contains("www.sxkid.com") || host?.ToLower().Contains("test.sxkid.com") == true || host?.ToLower().Contains("www4.sxkid.com") == true))
                {
                    host = host.Replace("www.sxkid.com", "m.sxkid.com");

                    var redirectUrl = new UriBuilder
                    {
                        Scheme = httpContext.Request.Scheme,
                        Host = host,
                        //Port = httpContext.Request.Host.Port??80,
                        Path = httpContext.Request.Path,
                        Query = httpContext.Request.QueryString.ToString()
                    };

                    if (host.ToLower().Contains("test.sxkid.com"))
                    {
                        redirectUrl.Host = "test.sxkid.com";
                        redirectUrl.Port = 5001;
                    }
                    if (host.ToLower().Contains("www4.sxkid.com"))
                    {
                        redirectUrl.Host = "m4.sxkid.com";
                    }

                    httpContext.Response.Redirect(redirectUrl.ToString());
                    httpContext.Response.StatusCode = 302;
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("跳转中间件出错，ex:{0} {1}", httpContext.Request.Path, ex.Message);
            }
            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class JudgeMiddlewareExtensions
    {
        public static IApplicationBuilder UseJudgeMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JudgeMiddleware>();
        }
    }
}
