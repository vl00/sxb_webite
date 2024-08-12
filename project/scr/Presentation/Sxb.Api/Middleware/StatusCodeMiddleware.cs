using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sxb.Api.Response;

namespace Sxb.Api.Middleware
{
public class StatusCodeMiddleware
    {
        private readonly RequestDelegate _next;

        public StatusCodeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            await _next(httpContext);

            var responseResult = new ResponseResult();

            switch (httpContext.Response.StatusCode)
            {
                case 200:
                    return;
                case 302:
                    return;
                case 400:
                    return;
                case 401:
                    responseResult.Msg = "你还未登录，请登录后再来吧";
                    responseResult.status = ResponseCode.NoLogin;
                    break;
                case 403:
                    break;
                case 503:
                    break;
                case 505:
                    responseResult.Msg = "出问题了，暂时无法提供内容，请稍后重试";
                    responseResult.status = ResponseCode.Error;
                    break;

                default:
                    responseResult.Msg = "没有这个位置，你找错地方了";
                    responseResult.status = ResponseCode.NoFound;
                    break;
            }

            var responseText = JsonConvert.SerializeObject(responseResult,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

            httpContext.Response.StatusCode = 200;
            httpContext.Response.ContentType = "application/json; charset=utf-8";

            await httpContext.Response.WriteAsync(responseText);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class StatusCodeMiddlewareExtensions
    {
        public static IApplicationBuilder UseStatusCodeMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<StatusCodeMiddleware>();
        }
    }
}
