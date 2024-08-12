using System;
using Microsoft.AspNetCore.Http;

namespace Sxb.Web.Middleware.Diffluence
{
    public class InterceptHandle
    {
        private readonly HttpContext _httpContext;
        private readonly PathString path;
        public const string Method = "get";
        public InterceptHandle(HttpContext httpContext)
        {
            _httpContext = httpContext;
            path = httpContext.Request.Path;
        }

        /// <summary>
        /// 不进行拦截
        /// </summary>
        public bool PassIntercept => !IsIntercept;

        /// <summary>
        /// 是否进行拦截
        /// </summary>
        public bool IsIntercept => InterceptPath && InterceptMethod;


        /// <summary>
        /// 拦截的路径
        /// </summary>
        public bool InterceptPath => path.HasValue && path.Value.Contains("~", StringComparison.CurrentCultureIgnoreCase);

        /// <summary>
        /// 拦截的请求方法
        /// </summary>
        public bool InterceptMethod => Method.Equals(_httpContext.Request.Method, StringComparison.CurrentCultureIgnoreCase);
    }
}
