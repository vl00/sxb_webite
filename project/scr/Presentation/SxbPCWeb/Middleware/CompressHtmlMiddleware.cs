using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sxb.PCWeb.Middleware
{
    /// <summary>
    /// 压缩HTML代码中间件
    /// </summary>
    public class CompressHtmlMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CompressHtmlMiddleware> _logger;
        Regex _htmlRegex = new Regex("\\s+(?=<)|\\s+$|(?<=>)\\s+");

        public CompressHtmlMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<CompressHtmlMiddleware>();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var routes = httpContext.GetRouteData()?.Values ?? null;

            if (routes != null && routes.Count() > 1)
            {
                var controller = routes["controller"]?.ToString() ?? string.Empty;
                var action = routes["action"]?.ToString() ?? string.Empty;

                if (controller == "Home" && action == "Index")
                {
                    var buffer = new MemoryStream();
                    var stream = httpContext.Response.Body;
                    httpContext.Response.Body = buffer;

                    await _next(httpContext);

                    if (httpContext.Response.StatusCode == 200 &&
                    httpContext.Response.ContentType.Equals("text/html; charset=utf-8", StringComparison.OrdinalIgnoreCase))
                    {
                        buffer.Seek(0, SeekOrigin.Begin);
                        var reader = new StreamReader(buffer);
                        string responseBody = await reader.ReadToEndAsync();
                        MemoryStream msNew = new MemoryStream();
                        using (StreamWriter wr = new StreamWriter(msNew))
                        {
                            wr.WriteLine(_htmlRegex.Replace(responseBody, ""));
                            wr.Flush();
                            msNew.Seek(0, SeekOrigin.Begin);
                            await msNew.CopyToAsync(stream);
                        }
                    }
                }
                else
                {
                    await _next(httpContext);
                }
            }
            else {
                await _next(httpContext);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CompressHtmlMiddlewareExtensions
    {
        public static IApplicationBuilder UseCompressHtmlMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CompressHtmlMiddleware>();
        }
    }
}
