using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sxb.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ExceptionMiddleware>();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                var formString = BodyData(httpContext);
                _logger.LogError(new EventId(1), ex, "{0} {1}", formString, ex.Message);

                httpContext.Response.StatusCode = 505;
            }
        }

        private static string BodyData(HttpContext httpContext)
        {
            string bodyData;
            using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8))
            {
                httpContext.Request.Body.Position = 0;
                bodyData = reader.ReadToEnd();
            }
            byte[] requestData = Encoding.UTF8.GetBytes(bodyData);
            httpContext.Request.Body = new MemoryStream(requestData);
            return bodyData;
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
