using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ProductManagement.Framework.Cache.Redis.Configuration
{
    public class RedisCachingConfig : IRedisCachingConfiguration
    {

        public RedisCachingConfig(IOptions<RedisConfig> options,
            ISerializer serializer,
            ILogger<CacheRedisLog> logger)
        {
            RedisConfig = options.Value;
            Serializer = serializer;
            LogWriter = RedisConfig.HaveLog ? new CacheRedisLog(logger) : null;
        }

        public RedisConfig RedisConfig { get; }

        public ISerializer Serializer { get; }

        public TextWriter LogWriter { get; }
    }
}