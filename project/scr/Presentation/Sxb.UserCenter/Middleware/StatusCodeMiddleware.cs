using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sxb.UserCenter.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Sxb.UserCenter.Utils;

namespace Sxb.UserCenter.Middleware
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

            if (httpContext.Request.IsAjax())
            {
                var responseResult = new ResponseResult();

                var config = httpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
                var userServiceUrl = config.GetSection("UserSystemConfig").GetValue<string>("ServerUrl");


                switch (httpContext.Response.StatusCode)
                {
                    case 200:
                        return;

                    case 302:
                        return;

                    case 500:
                        responseResult.Msg = "出问题了，暂时无法提供内容，请稍后重试";
                        responseResult.status = ResponseCode.Error;
                        break;

                    case 401:
                        responseResult.Msg = "你还未登录，请登录后再来吧";
                        responseResult.status = ResponseCode.NoLogin;
                        string referer = httpContext.Request.Headers["Referer"];
                        if (!string.IsNullOrEmpty(referer))
                        {
                            referer = Uri.EscapeDataString(referer);
                        }
                        responseResult.Data = new { LoginUrl = userServiceUrl + "/login/?ReturnUrl=" + referer };
                        break;

                    case 403:
                        break;

                    case 503:
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
            //else
            //{
            //    switch (httpContext.Response.StatusCode)
            //    {
            //        case 404:
            //            httpContext.Response.Redirect("/errors/404");
            //            break;
            //        default:
            //            break;
            //    }
            //}
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
