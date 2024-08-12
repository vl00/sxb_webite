using System;
using ProductManagement.Framework.Cache.Redis.Configuration;
using ProductManagement.Framework.Cache.Redis.NumberCreater;
using ProductManagement.Framework.Cache.Redis.Serializer;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ProductManagement.Framework.Cache.Redis.Test
{
    public class RedisDiTest
    {
        private IServiceProvider BuildServiceProvider(Action<IServiceCollection> setupServices = null)
        {
            var services = new ServiceCollection();
            services.AddOptions();
            services.AddLogging();
            setupServices?.Invoke(services);
            return services.BuildServiceProvider();
        }

        [Fact]
        public void RedisDi_DefaultType_Test()
        {
            IServiceProvider serviceProvider = BuildServiceProvider(s=>{
                s.AddSingletonCacheRedis(o =>
                {
                    o.Database = 0;
                    o.RedisConnect =
                        "10.0.0.181:7111,ssl=false,abortConnect=false";
                });
            });

            var config = serviceProvider.GetService<IRedisCachingConfiguration>();
            Assert.IsType<RedisCachingConfig>(config);
            Assert.Null(config.LogWriter);

            var client = serviceProvider.GetService<ICacheRedisClient>();

            Assert.IsType<ProductManagementCacheClient>(client);
            Assert.IsType<NewtonsoftSerializer>(client.Serializer);
            Assert.NotNull(client.Database);

            var numberCreater = serviceProvider.GetService<INumberCreater>();

            Assert.IsType<NumberRedisCreater>(numberCreater);
        }

        [Fact]
        public void RedisDi_HaveLog_Test()
        {
            IServiceProvider serviceProvider = BuildServiceProvider(s => {
                s.AddSingletonCacheRedis(o =>
                {
                    o.Database = 0;
                    o.RedisConnect =
                        "10.0.0.181:7111,ssl=false,abortConnect=false";
                    o.HaveLog = true;
                });
            });

            var config = serviceProvider.GetService<IRedisCachingConfiguration>();
            Assert.IsType<RedisCachingConfig>(config);
            Assert.NotNull(config.LogWriter);

        }

        [Fact]
        public void RedisDi_NullRedisConfigOptions_Test()
        {
            BuildServiceProvider(s =>
            {
                Assert.Throws<ArgumentNullException>(() => { s.AddSingletonCacheRedis(null); });
            });
        }
    }
}
