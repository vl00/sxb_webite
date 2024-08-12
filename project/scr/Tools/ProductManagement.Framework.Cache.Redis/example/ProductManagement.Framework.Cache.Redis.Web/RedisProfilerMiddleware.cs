using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace ProductManagement.Framework.Cache.Redis.Web
{
    public class RedisProfilerMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ICacheRedisClient _cacheRedisClient;

        private readonly RedisProfiler _profiler;

        private readonly ILogger<RedisProfilerMiddleware> _logger;

        public RedisProfilerMiddleware(RequestDelegate next, 
            ICacheRedisClient cacheRedisClient, RedisProfiler profiler,
            ILogger<RedisProfilerMiddleware> logger)
        {
            _next = next;
            _cacheRedisClient = cacheRedisClient;

            _profiler = profiler;
            _logger = logger;
            _cacheRedisClient.Connection.RegisterProfiler(_profiler);
        }

        public async Task Invoke(HttpContext httpContext)
        {
            _profiler.HttpContext = httpContext;

            _cacheRedisClient.Connection.BeginProfiling(_profiler.CreateContextForCurrentRequest());

            await _next(httpContext);

            var timings = _cacheRedisClient.Connection.FinishProfiling(_profiler.GetContext());

            _logger.LogInformation(string.Join("\r\n", timings));

            //foreach (var timing in timings)
            //{
            //    Console.WriteLine(timing);
            //    Console.WriteLine();
            //}
        }
    }
}
