using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ProductManagement.Framework.Cache.Redis;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Middleware
{
    /// <summary>
    /// 检查已登录帐号是否封禁
    /// </summary>
    public class UserBlockCheckMiddleware
    {
        private readonly RequestDelegate _next;
        IEasyRedisClient _easyRedisClient;

        public UserBlockCheckMiddleware(RequestDelegate next, IEasyRedisClient easyRedisClient)
        {
            _easyRedisClient = easyRedisClient;
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var endpointMetaData = httpContext.GetEndpoint()?.Metadata;
            if (endpointMetaData == null)
            {
                await _next(httpContext);
                return;
            }

            var hasAuthorizeAttribute = endpointMetaData.Any(x => x is AuthorizeAttribute);
            if (!hasAuthorizeAttribute)
            {
                await _next(httpContext);
                return;
            }

            if (!httpContext.User.Identity.IsAuthenticated)
            {
                await _next(httpContext);
                return;
            }

            var blockResult = await _easyRedisClient.ExistsAsync($"UserBlocked:{httpContext.User.Identity.GetId()}");
            if (blockResult)
            {
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                httpContext.Response.Cookies.Append("userid", "", new CookieOptions { Domain = ".sxkid.com", SameSite = SameSiteMode.Lax });
                httpContext.Response.StatusCode = 403;
                return;
            }

            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class UserBlockCheckMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserBlockCheckMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserBlockCheckMiddleware>();
        }
    }
}
