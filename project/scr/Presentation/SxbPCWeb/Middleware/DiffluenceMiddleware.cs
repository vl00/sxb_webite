using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ProductManagement.Framework.Foundation;
using Sxb.PCWeb.Middleware.Diffluence;

namespace Sxb.PCWeb.Middleware
{
    /// <summary>
    /// 解析请求参数，转发到对应的Action
    /// </summary>
    public class DiffluenceMiddleware
    {
        private readonly RequestDelegate _next;
        public DiffluenceMiddleware(RequestDelegate next, IServiceProvider service)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var interceptHandle = new InterceptHandle(httpContext);

            var path = httpContext.Request.Path.ToString();


            if (interceptHandle.PassIntercept)
            {
              if (path.ToLower().Contains("/question/school") && httpContext.Request.Query.ContainsKey("extid"))
                {
                    var redirectUrl = "/";
                    if (httpContext.Request.Query.ContainsKey("extid"))
                    {
                        redirectUrl = "/question/school/" + httpContext.Request.Query["extid"].ToString().ToLower().Replace("-", "");
                    }
                    else
                    {
                        redirectUrl = path.ToLower().Replace("-", "");
                    }
                    httpContext.Response.Redirect(redirectUrl);
                    httpContext.Response.StatusCode = 301;
                    return;
                }
                else if (path.ToLower().Contains("/comment/school") && httpContext.Request.Query.ContainsKey("extid"))
                {
                    var redirectUrl = "/";
                    if (httpContext.Request.Query.ContainsKey("extid"))
                    {
                        redirectUrl = "/comment/school/" + httpContext.Request.Query["extid"].ToString().ToLower().Replace("-", "");
                    }
                    else
                    {
                        redirectUrl = path.ToLower().Replace("-", "");
                    }
                    httpContext.Response.Redirect(redirectUrl);
                    httpContext.Response.StatusCode = 301;
                    return;
                }
                //else if (path.StartsWith("/c-"))
                //{
                //    var cityCode = path.Replace("/c-", "");
                //    if (string.IsNullOrWhiteSpace(cityCode)) cityCode = "0";
                //    httpContext.Response.Redirect($"/home/setregion/{cityCode}");
                //    httpContext.Response.StatusCode = 301;
                //    return;
                //}

                await _next(httpContext);
                return;
            }

            var requestPath = new DefaultRequestPath(httpContext);

            var orientPath = requestPath.OrientPath;


            httpContext.Request.Method = requestPath.Method;
            httpContext.Request.Path = orientPath;
            httpContext.Request.QueryString = requestPath.Query;

            if (path.ToLower().Contains("/school/detail") && path.Contains("-"))
            {
                var redirectUrl = "/";
                if (httpContext.Request.Query.ContainsKey("extid"))
                {
                    redirectUrl = "/school/detail/" + httpContext.Request.Query["extid"].ToString().ToLower().Replace("-", "");
                }
                httpContext.Response.StatusCode = 301;
                httpContext.Response.Headers.Add("Location", redirectUrl);
                return;
            }
            else if (path.ToLower().Contains("/school/eges") && path.Contains("-"))
            {
                var redirectUrl = "/";
                if (httpContext.Request.Query.ContainsKey("extid"))
                {
                    redirectUrl = "/school/eges/" + httpContext.Request.Query["extid"].ToString().ToLower().Replace("-", "");
                }
                httpContext.Response.StatusCode = 301;
                httpContext.Response.Headers.Add("Location", redirectUrl);
                return;
            }
            else if (path.ToLower().Contains("/school/extrecruit") && path.Contains("-"))
            {
                var redirectUrl = "/";
                if (httpContext.Request.Query.ContainsKey("extid"))
                {
                    redirectUrl = "/school/extrecruit/" + httpContext.Request.Query["extid"].ToString().ToLower().Replace("-", "");
                }
                httpContext.Response.StatusCode = 301;
                httpContext.Response.Headers.Add("Location", redirectUrl);
                return;
            }
            else if (path.ToLower().Contains("/school/extfraction") && path.Contains("-"))
            {
                var redirectUrl = "/";
                if (httpContext.Request.Query.ContainsKey("extid"))
                {
                    redirectUrl = "/school/extfraction/" + httpContext.Request.Query["extid"].ToString().ToLower().Replace("-", "");
                }
                httpContext.Response.StatusCode = 301;
                httpContext.Response.Headers.Add("Location", redirectUrl);
                return;
            }
            else if (path.ToLower().Contains("/article/detail") && path.Contains("-"))
            {
                var redirectUrl = "/";
                if (httpContext.Request.Query.ContainsKey("id"))
                {
                    redirectUrl = "/article/detail/" + httpContext.Request.Query["id"].ToString().ToLower().Replace("-", "");
                }
                httpContext.Response.StatusCode = 301;
                httpContext.Response.Headers.Add("Location", redirectUrl);
                return;
            }
            else if (path.ToLower().Contains("/question/school") && httpContext.Request.Query.ContainsKey("extid"))
            {
                var redirectUrl = "/";
                if (httpContext.Request.Query.ContainsKey("extid"))
                {
                    redirectUrl = "/question/school/" + httpContext.Request.Query["extid"].ToString().ToLower().Replace("-", "");
                }
                else
                {
                    redirectUrl = path.ToLower().Replace("-", "");
                }
                httpContext.Response.Redirect(redirectUrl);
                httpContext.Response.StatusCode = 301;
                return;
            }
            else if (path.ToLower().Contains("/comment/school") && httpContext.Request.Query.ContainsKey("extid"))
            {
                var redirectUrl = "/";
                if (httpContext.Request.Query.ContainsKey("extid"))
                {
                    redirectUrl = "/comment/school/" + httpContext.Request.Query["extid"].ToString().ToLower().Replace("-", "");
                }
                else
                {
                    redirectUrl = path.ToLower().Replace("-", "");
                }
                httpContext.Response.Redirect(redirectUrl);
                httpContext.Response.StatusCode = 301;
                return;
            }

            if (string.IsNullOrEmpty(requestPath.OrientPath))
            {
                httpContext.Response.StatusCode = 404;
                return;
            }
            //httpContext.Request.ContentType = "application/json";
            await _next(httpContext);
        }
    }
    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class DiffluenceMiddlewareExtensions
    {
        public static IApplicationBuilder UseDiffluenceMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DiffluenceMiddleware>();
        }
    }
}
