using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductManagement.Infrastructure;
using ProductManagement.Infrastructure.Exceptions;
using Sxb.Web.Response;
using Sxb.Web.Utils;

namespace Sxb.Web.Middleware
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
                //var formString = BodyData(httpContext);
                //httpContext.Response.StatusCode = 505;
                HandleException(httpContext, ex);
            }
        }

        private void HandleException(HttpContext context, Exception e)
        {
            if (e == null) return;

            _logger.LogError(new EventId(1), e, "{0} {1}{2}\n{3}",
                context.Request.Path, context.Request.QueryString.ToString(), BodyData(context), e.Message);

            WriteException(context, e);
        }

        private static void WriteException(HttpContext context, Exception e)
        {
            switch (e)
            {
                case NotFoundException ex:
                    context.Response.StatusCode = 406;
                    break;
                case ResponseResultException ex:
                    context.Response.StatusCode = ex.Code;
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }
        }

        private static string BodyData(HttpContext httpContext)
        {
            try
            {
                string Params = "";
                if (httpContext.Request.Method.ToLower() == "get")
                {
                    Params = httpContext.Request.QueryString.ToString();
                }
                else
                {
                    httpContext.Request.EnableBuffering();
                    using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                    {
                        Params = reader.ReadToEndAsync().Result;
                    }
                    httpContext.Request.Body.Position = 0;
                }
                return Params;
            }
            catch
            {
                return "";
            }
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
