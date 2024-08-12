using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;

namespace ProductManagement.Framework.Cache.Redis.Web
{
    public class RedisProfiler : IProfiler
    {
        const string RequestContextKey = "RequestProfilingContext";

        private readonly IHttpContextAccessor _accessor;

        public RedisProfiler(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public HttpContext HttpContext { get; set; }

        public object GetContext()
        {
            var ctx = _accessor.HttpContext;

            return ctx?.Items[RequestContextKey];
        }

        public object CreateContextForCurrentRequest()
        {
            var ctx = _accessor.HttpContext;
            if (ctx == null) return null;

            object ret;
            ctx.Items[RequestContextKey] = ret = new object();

            return ret;
        }
    }
}
