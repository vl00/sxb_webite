using System.Threading.Tasks;
using ProductManagement.Framework.Cache.Redis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ProductManagement.Framework.Cache.RedisProfiler
{
    public class RedisProfilerMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ICacheRedisClient _cacheRedisClient;

        private readonly MyRedisProfiler _profiler;

        private readonly ILogger<RedisProfilerMiddleware> _logger;

        public RedisProfilerMiddleware(RequestDelegate next, 
            ICacheRedisClient cacheRedisClient, MyRedisProfiler profiler,
            ILogger<RedisProfilerMiddleware> logger)
        {
            _next = next;
            _cacheRedisClient = cacheRedisClient;

            _profiler = profiler;
            _logger = logger;
            _cacheRedisClient.Connection.RegisterProfiler(() => _profiler.GetContext());
        }

        public async Task Invoke(HttpContext httpContext)
        {
            _profiler.HttpContext = httpContext;

            _profiler.CreateContextForCurrentRequest();

            await _next(httpContext);

            var session = _profiler.GetContext();
            if (session != null)
            {
                var timings = session.FinishProfiling();

                _logger.LogTrace("[{0}]_{1}", httpContext.Request.Path, string.Join("\r\n", timings));
            }
            
        }
    }
}
