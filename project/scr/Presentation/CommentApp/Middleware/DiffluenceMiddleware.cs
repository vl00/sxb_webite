using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Sxb.Web.Middleware.Diffluence;

namespace Sxb.Web.Middleware
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
                await _next(httpContext);
                return;
            }

            var requestPath = new DefaultRequestPath(httpContext);
            var orientPath = requestPath.OrientPath;

            if (requestPath.OrientPath == "//school")
            {
                orientPath = "/school/list";
            }
            else if (requestPath.OrientPath == "//comment")
            {
                orientPath = "/comment/list";
            }
            else if (requestPath.OrientPath == "//question")
            {
                orientPath = "/question/list";
            }
            else if (requestPath.OrientPath == "//article")
            {
                orientPath = "/article/list";
            }

            httpContext.Request.Method = requestPath.Method;
			httpContext.Request.Path = orientPath;
            httpContext.Request.QueryString = requestPath.Query;


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
