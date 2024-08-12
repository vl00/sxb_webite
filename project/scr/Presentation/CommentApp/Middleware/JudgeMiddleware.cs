using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Sxb.Web.Middleware
{
    public class JudgeMiddleware
    {
        private readonly RequestDelegate _next;

        public JudgeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            bool isMobile = httpContext.Request.Headers["User-Agent"].ToString().ToLower().Contains("mobile");
            var host = httpContext.Request.Host.Host;
            if (!isMobile && host.Contains("m.sxkid.com"))
            {
                host = host.Replace("m.sxkid.com", "www.sxkid.com");

                var redirectUrl = new UriBuilder
                {
                    Scheme = httpContext.Request.Scheme,
                    Host = host,
                    //Port = httpContext.Request.Host.Port??80,
                    Path = httpContext.Request.Path,
                    Query = httpContext.Request.QueryString.ToString()
                };

                httpContext.Response.Redirect(redirectUrl.ToString());
                httpContext.Response.StatusCode = 302;
                return;
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
