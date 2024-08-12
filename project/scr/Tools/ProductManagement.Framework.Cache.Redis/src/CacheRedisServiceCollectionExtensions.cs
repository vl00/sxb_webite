using System;
using ProductManagement.Framework.Cache.Redis.Configuration;
using ProductManagement.Framework.Cache.Redis.NumberCreater;
using ProductManagement.Framework.Cache.Redis.Serializer;
using Microsoft.Extensions.DependencyInjection;

namespace ProductManagement.Framework.Cache.Redis
{
    public static class CacheRedisServiceCollectionExtensions
    {
        public static IServiceCollection AddSingletonCacheRedis(this IServiceCollection services,
            Action<RedisConfig> redisConfigOptions, ISerializer serializer = null)
        {
            AddCacheRedis(services, redisConfigOptions, serializer);

            services.AddSingleton<ICacheRedisClient, ProductManagementCacheClient>();
            services.AddSingleton<IEasyRedisClient, EasyRedisClient>();

            return services;
        }

        private static void AddCacheRedis(IServiceCollection services,
            Action<RedisConfig> redisConfigOptions, ISerializer serializer)
        {
            if (redisConfigOptions == null)
            {
                throw new ArgumentNullException(nameof(redisConfigOptions));
            }

            services.Configure(redisConfigOptions);

            if (serializer == null)
            {
                services.AddSingleton<ISerializer, NewtonsoftSerializer>();
            }
            else
            {
                services.AddSingleton(s => serializer);
            }

            services.AddSingleton<IRedisCachingConfiguration, RedisCachingConfig>();

            services.AddSingleton<INumberCreater>(f => new NumberRedisCreater(f.GetService<ICacheRedisClient>()));
        }
    }
}
