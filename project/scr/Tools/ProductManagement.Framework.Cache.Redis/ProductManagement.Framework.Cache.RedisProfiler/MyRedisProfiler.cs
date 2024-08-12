using System;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using StackExchange.Redis.Profiling;

namespace ProductManagement.Framework.Cache.RedisProfiler
{
    public class MyRedisProfiler
    {
        const string RequestContextKey = "RequestProfilingContext";

        private readonly IHttpContextAccessor _accessor;

        public MyRedisProfiler(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public HttpContext HttpContext { get; set; }

        public ProfilingSession GetContext()
        {
            var ctx = _accessor.HttpContext;
            if (ctx == null) return null;
            return (ProfilingSession)ctx.Items[RequestContextKey];
        }

        public void CreateContextForCurrentRequest()
        {
            var ctx = _accessor.HttpContext;
            if (ctx != null)
            {
                ctx.Items[RequestContextKey] = new ProfilingSession();
            }
        }
    }
}
